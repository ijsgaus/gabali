using System.Collections.Immutable;

namespace RabbitRelink.Messaging;

/// <summary>
/// Represents message properties
/// </summary>
public sealed record Properties()
{
    #region Fields

    private readonly string? _appId;
    private readonly string? _clusterId;
    private readonly string? _contentEncoding;
    private readonly string? _contentType;
    private readonly string? _correlationId;
    private readonly string? _replyTo;
    private readonly TimeSpan? _expiration;
    private readonly string? _messageId;
    private readonly string? _type;
    private readonly string? _userId;

    #endregion

    /// <summary>
    /// Application Id
    /// </summary>
    public string? AppId
    {
        get => _appId;
        init => _appId = CheckShortString(nameof(value), value);
    }

    /// <summary>
    /// Cluster Id
    /// </summary>
    public string? ClusterId
    {
        get => _clusterId;
        init => _clusterId = NormalizeString(value);
    }

    /// <summary>
    /// Content encoding of body
    /// </summary>
    public string? ContentEncoding
    {
        get => _contentEncoding;
        init => _contentEncoding = CheckShortString(nameof(value), value);
    }

    /// <summary>
    /// Content type of body
    /// </summary>
    public string? ContentType
    {
        get => _contentType;
        init => _contentType = CheckShortString(nameof(value), value);
    }

    /// <summary>
    /// Request correlation id
    /// </summary>
    public string? CorrelationId
    {
        get => _correlationId;
        init => _correlationId = CheckShortString(nameof(value), value);
    }

    /// <summary>
    /// Message delivery mode
    /// </summary>
    public DeliveryMode DeliveryMode { get; init; } = DeliveryMode.Default;

    /// <summary>
    /// Reply to for request
    /// </summary>
    public string? ReplyTo
    {
        get => _replyTo;
        init => _replyTo = CheckShortString(nameof(value), value);
    }

    /// <summary>
    /// Message expiration (TTL)
    /// </summary>
    public TimeSpan? Expiration
    {
        get => _expiration;
        init
        {
            if (value?.TotalMilliseconds < 0 || value?.TotalMilliseconds > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(value),
                    "Must be greater or equal 0 and less than Int32.MaxValue");

            _expiration = value;
        }
    }

    /// <summary>
    /// Message Id
    /// </summary>
    public string? MessageId
    {
        get => _messageId;
        init => _messageId = CheckShortString(nameof(value), value);
    }

    /// <summary>
    /// Timestamp in UNIX time
    /// </summary>
    public long? TimeStamp { get; init; }

    /// <summary>
    /// Message type
    /// </summary>
    public string? Type
    {
        get => _type;
        init => _type = CheckShortString(nameof(value), value);
    }

    /// <summary>
    /// Id of sender
    /// </summary>
    public string? UserId
    {
        get => _userId;
        init => _userId = CheckShortString(nameof(value), value);
    }

    /// <summary>
    /// Priority of message
    /// </summary>
    public byte? Priority { get; init; }

    /// <summary>
    /// Message headers
    /// </summary>
    public IImmutableDictionary<string, object>? Headers { get; init; }

    #region Private methods

    private static string? CheckShortString(string name, string? input)
    {
        input = NormalizeString(input);

        if (input != null && input.Length > 255)
        {
            throw new ArgumentOutOfRangeException(name, "Must be less than 256 characters long");
        }

        return input;
    }

    private static string? NormalizeString(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        return input!.Trim();
    }

    #endregion
}
