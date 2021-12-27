#region Usings

#endregion

namespace RabbitRelink.Messaging;

/// <summary>
///     MessageId generator which uses <see cref="Guid.NewGuid" />
/// </summary>
public class GuidMessageIdGenerator : IMessageIdGenerator
{
    /// <inheritdoc cref="IMessageIdGenerator"/>
    public MessageProperties SetMessageId(byte[]? body, MessageProperties properties,
        PublishProperties publishProperties)
        => properties with {MessageId = Guid.NewGuid().ToString("D")};
}
