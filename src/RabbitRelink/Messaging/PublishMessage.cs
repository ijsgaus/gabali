namespace RabbitRelink.Messaging;

/// <summary>
/// Represents RabbitMQ message for publish
/// </summary>
/// <param name="Body">Message body</param>
/// <typeparam name="TBody">ype of message body</typeparam>
public sealed record PublishMessage<TBody>(TBody Body)
{
    /// <summary>
    /// Configure message properties
    /// </summary>
    public Apply<Properties> ConfigureProperties { get; init; } = Fn.Id;

    /// <summary>
    /// Configure publish properties
    /// </summary>
    public Apply<PublishProperties> ConfigurePublish { get; init; } = Fn.Id;


    /// <summary>
    /// Change message body type
    /// </summary>
    /// <param name="mapper">mapper function</param>
    /// <typeparam name="TOther">other body type</typeparam>
    /// <returns><see cref="PublishMessage{TBody}"/> with <see cref="TOther"/> type</returns>
    public PublishMessage<TOther> Map<TOther>(Func<TBody, TOther> mapper)
        => new PublishMessage<TOther>(mapper(Body))
        {
            ConfigureProperties = ConfigureProperties,
            ConfigurePublish = ConfigurePublish
        };
}
