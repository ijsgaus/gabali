#region Usings

using System.Collections.Immutable;
using System.Reflection;
using RabbitMQ.Client;

#endregion

namespace RabbitRelink.Connection
{
    internal class RelinkConnectionFactory : IRelinkConnectionFactory
    {
        #region Fields

        private readonly ConnectionFactory _factory;

        private readonly IImmutableSet<string> _hosts;

        #endregion

        #region Ctor

        public RelinkConnectionFactory(
            string name,
            string appId,
            Uri connectionString,
            TimeSpan timeout,
            bool useBackgroundThreads,
            IImmutableSet<string> hosts)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (timeout.TotalMilliseconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(timeout), timeout.TotalMilliseconds,
                    "Must be grater than 0 milliseconds");

            Name = name;

            _factory = new ConnectionFactory
            {
                Uri = connectionString ?? throw new ArgumentNullException(nameof(connectionString)),
                TopologyRecoveryEnabled = false,
                AutomaticRecoveryEnabled = false,
                UseBackgroundThreadsForIO = useBackgroundThreads,
                RequestedConnectionTimeout = timeout,
                ClientProperties =
                {
                    ["product"] = "RabbitRelink",
                    ["version"] = GetType()?.Assembly?.GetName()?.Version?.ToString(3) ?? "<unknown>",
                    ["copyright"] = "Copyright (c) 2021 RabbitRelink",
                    ["information"] = "https://github.com/ijsgaus/rabbit-relink",
                    ["app_id"] = appId
                }
            };

            _hosts = hosts;
        }

        #endregion

       #region ILinkConnectionFactory Members

        public string Name { get; }
        public string UserName => _factory.UserName;

        public IConnection GetConnection()
        {
            return _factory.CreateConnection(_hosts.ToList(), Name);
        }

        #endregion
    }
}
