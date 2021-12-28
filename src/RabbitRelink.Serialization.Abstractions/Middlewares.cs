using RabbitRelink.Messaging;
using RabbitRelink.Middlewares;

namespace RabbitRelink.Serialization.Abstractions;

internal static class Middlewares
{
    public static ProducerMiddleware<byte[]?, T> DirectSerialize<T>(ISerializer<T> serializer, string? discriminator) where T : class? => next => async (body, properties, publish, cancellation) =>
        {
            var serialized = await serializer.SerializeAsync(body, cancellation);

            Properties Properties(Properties props)
            {
                props = (properties ?? (p => p))(props);
                return props with {Type = discriminator, ContentType = serializer.MediaType.ToString()};
            }

            await next(serialized, Properties, publish, cancellation);
        };

    public static ConsumerMiddleware<T?, byte[]?> DirectDeserialize<T>(IDeserializer<T> deserializer) where T : class? => next => async msg =>
        {
            var deserialized = await deserializer.DeserializeAsync(msg.Body, msg.Cancellation);
            var inner = new ConsumedMessage<T?>(deserialized, msg.Properties, msg.ReceiveProperties, msg.Cancellation);
            return await next(inner);
        };
}
