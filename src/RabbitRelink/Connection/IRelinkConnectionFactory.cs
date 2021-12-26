using RabbitMQ.Client;

namespace RabbitRelink.Connection;

internal interface IRelinkConnectionFactory
{
    IConnection GetConnection();

    #region Properties

    string UserName { get; }
    string Name { get; }

    #endregion
}
