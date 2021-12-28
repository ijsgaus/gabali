#region Usings

#endregion

namespace RabbitRelink.Messaging;

/// <summary>
///     MessageId generator which uses <see cref="Guid.NewGuid" />
/// </summary>
public class GuidMessageIdGenerator : IMessageIdGenerator
{
    /// <inheritdoc cref="IMessageIdGenerator"/>
    public Properties SetMessageId(byte[]? body, Properties properties,
        PublishProperties publishProperties)
        => properties with {MessageId = Guid.NewGuid().ToString("D")};
}
