using System.Collections.Immutable;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitRelink.Internals;
using RabbitRelink.Internals.Actions;
using RabbitRelink.Internals.Async;
using RabbitRelink.Internals.Channels;
using RabbitRelink.Internals.Lens;
using RabbitRelink.Logging;

namespace RabbitRelink.Connection;

internal class RelinkConnection : AsyncStateMachine<RelinkConnectionState>, IRelinkConnection
{
    #region Fields

    public Uri Uri { get; }
    public IImmutableSet<string> Hosts { get; }

    public RelinkConfig Config { get; }
    private readonly IRelinkConnectionFactory _connectionFactory;

    private readonly CancellationToken _disposeCancellation;
    private readonly CancellationTokenSource _disposeCts;
    private readonly IRelinkLogger _logger;

    private readonly CompositeActionStorage<IConnection> _storage = new CompositeActionStorage<IConnection>(
        new CompositeChannel<ActionItem<IConnection>>(new LensChannel<ActionItem<IConnection>>())
    );

    private readonly object _sync = new object();

    private IConnection? _connection;
    private CancellationTokenSource? _connectionActiveCts;

    private Task? _loopTask;

    #endregion

    #region Ctor

    public RelinkConnection(Uri uri, RelinkConfig config, IImmutableSet<string> hosts) : base(RelinkConnectionState.Init)
    {
        Config = config;
        Uri = uri;
        Hosts = hosts;

        _logger = config.LoggerFactory.CreateLogger($"{GetType().Name}({Id:D})");

        _connectionFactory = new RelinkConnectionFactory(
            Config.ConnectionName,
            Config.AppId,
            Uri,
            Config.Timeout,
            Config.UseBackgroundThreadsForConnection,
            Hosts
        );

        _disposeCts = new CancellationTokenSource();
        _disposeCancellation = _disposeCts.Token;

        _logger.Debug($"Created ( name: {Config.ConnectionName})");
        if (Config.AutoStart)
        {
            Initialize();
        }
    }

    #endregion

    #region ILinkConnection Members

    public void Dispose()
    {
        if (State == RelinkConnectionState.Disposed)
            return;

        lock (_sync)
        {
            if (State == RelinkConnectionState.Disposed)
                return;

            _logger.Debug("Disposing");

            _disposeCts.Cancel();
            _disposeCts.Dispose();

            try
            {
                _loopTask?.Wait(CancellationToken.None);
            }
            catch
            {
                // no op
            }

            _storage.Dispose();

            ChangeState(RelinkConnectionState.Disposed);

            Disposed?.Invoke(this, EventArgs.Empty);

            _logger.Debug("Disposed");
        }
    }

    public event EventHandler? Disposed;
    public event EventHandler? Connected;
    public event EventHandler? Disconnected;

    public Guid Id { get; } = Guid.NewGuid();

    public string UserId => _connectionFactory.UserName;

    public void Initialize()
    {
        if (_disposeCancellation.IsCancellationRequested)
            throw new ObjectDisposedException(GetType().Name);

        if (State != RelinkConnectionState.Init)
            throw new InvalidOperationException("Already initialized");

        lock (_sync)
        {
            if (_disposeCancellation.IsCancellationRequested)
                throw new ObjectDisposedException(GetType().Name);

            if (State != RelinkConnectionState.Init)
                throw new InvalidOperationException("Already initialized");

            ChangeState(RelinkConnectionState.Opening);
            _loopTask = Task.Run(async () => await Loop().ConfigureAwait(false), _disposeCancellation);
        }
    }

    public Task<IModel> CreateModelAsync(CancellationToken cancellation)
    {
        return _storage.PutAsync(conn => conn.CreateModel(), cancellation)!;
    }

    #endregion


    protected override void OnStateChange(RelinkConnectionState newState)
    {
        _logger.Debug($"State change {State} -> {newState}");

        try
        {
            Config.StateHandler(State, newState);
        }
        catch (Exception ex)
        {
            _logger.Warning($"Exception in state handler: {ex}");
        }

        base.OnStateChange(newState);
    }

    private async Task Loop()
    {
        var newState = RelinkConnectionState.Opening;

        while (true)
        {
            if (_disposeCancellation.IsCancellationRequested)
            {
                newState = RelinkConnectionState.Stopping;
            }

            ChangeState(newState);

            try
            {
                switch (State)
                {
                    case RelinkConnectionState.Opening:
                    case RelinkConnectionState.Reopening:
                        newState = await OpenReopenAsync(State == RelinkConnectionState.Reopening)
                            .ConfigureAwait(false)
                            ? RelinkConnectionState.Active
                            : RelinkConnectionState.Stopping;
                        break;
                    case RelinkConnectionState.Active:
                        await AsyncHelper.RunAsync(Active)
                            .ConfigureAwait(false);
                        newState = RelinkConnectionState.Stopping;
                        break;
                    case RelinkConnectionState.Stopping:
                        await AsyncHelper.RunAsync(Stop)
                            .ConfigureAwait(false);
                        if (_disposeCancellation.IsCancellationRequested)
                        {
                            return;
                        }
                        newState = RelinkConnectionState.Reopening;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(State),State, $"Handler for state ${State} not implemented");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Unhandled exception: {ex}");
            }
        }
    }

    private async Task<bool> OpenReopenAsync(bool reopen)
    {
        using var yieldCts = new CancellationTokenSource();
        var connectTask = ConnectAsync(reopen, yieldCts);

        try
        {
            await _storage.YieldAsync(yieldCts.Token)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // No Op
        }

        return await connectTask
            .ConfigureAwait(false);
    }

    private async Task<bool> ConnectAsync(bool reopen, CancellationTokenSource cts)
    {
        try
        {
            if (_disposeCancellation.IsCancellationRequested)
                return false;

            if (reopen)
            {
                var timeout = Config.RecoveryInterval;
                _logger.Info($"Reopening in {timeout.TotalSeconds:0.###}s");

                try
                {
                    await Task.Delay(timeout, _disposeCancellation)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    return false;
                }
            }

            // start long-running task for synchronous connect
            if (await AsyncHelper.RunAsync(Connect)
                    .ConfigureAwait(false))
            {
                return true;
            }

            return false;
        }
        finally
        {
            cts.Cancel();
        }
    }

    private bool Connect()
    {
        if (_disposeCancellation.IsCancellationRequested)
            return false;

        _logger.Info("Connecting");

        try
        {
            _connection = _connectionFactory.GetConnection();
            _connectionActiveCts = new CancellationTokenSource();

            _connection.ConnectionShutdown += ConnectionOnConnectionShutdown;
            _connection.CallbackException += ConnectionOnCallbackException;
            _connection.ConnectionBlocked += ConnectionOnConnectionBlocked;
            _connection.ConnectionUnblocked += ConnectionOnConnectionUnblocked;
        }
        catch (Exception ex)
        {
            _logger.Error($"Cannot connect: {ex.Message}");
            return false;
        }

        _logger.Info(
            $"Connected (Host: {_connection.Endpoint.HostName}, Port: {_connection.Endpoint.Port}, LocalPort: {_connection.LocalPort})");

        return true;
    }

    private void Stop()
    {
        _connectionActiveCts?.Cancel();
        _connectionActiveCts?.Dispose();
        _connectionActiveCts = null;

        try
        {
            _connection?.Dispose();
        }
        catch (IOException)
        {
        }
        catch (Exception ex)
        {
            _logger.Warning($"Cleaning exception: {ex}");
        }
    }

    private void Active()
    {
        Connected?.Invoke(this, EventArgs.Empty);

        using (var cts =
               CancellationTokenSource.CreateLinkedTokenSource(_disposeCancellation, _connectionActiveCts?.Token
                   ?? throw new NullReferenceException($"{nameof(_connectionActiveCts)} not initialized")))
        {
            try
            {
                while (true)
                {
                    ActionItem<IConnection> item;

                    try
                    {
                        item = _storage.Wait(cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }

                    try
                    {
                        item.TrySetResult(item.Value(_connection ?? throw new NullReferenceException($"{nameof(_connection)} not initialized")));
                    }
                    catch (Exception ex)
                    {
                        _storage.PutRetry(new[] {item}, CancellationToken.None);
                        _logger.Error($"Cannot create model: {ex.Message}");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Debug($"Processing stopped: {ex}");
            }
        }

        Disconnected?.Invoke(this, EventArgs.Empty);
    }

    private void ConnectionOnConnectionUnblocked(object? sender, EventArgs e)
    {
        _logger.Debug("Unblocked");
    }

    private void ConnectionOnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
    {
        _logger.Debug($"Blocked, reason: {e.Reason}");
    }

    private void ConnectionOnCallbackException(object? sender, CallbackExceptionEventArgs e)
    {
        _logger.Error($"Callback exception: {e.Exception}");
    }

    private void ConnectionOnConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        _logger.Info($"Disconnected, Initiator: {e.Initiator}, Code: {e.ReplyCode}, Message: {e.ReplyText}");

        // if initialized by application, exit
        if (e.Initiator == ShutdownInitiator.Application) return;

        _connectionActiveCts?.Cancel();
    }
}
