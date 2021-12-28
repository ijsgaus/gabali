namespace RabbitRelink.Serialization.Abstractions;

public static class ProducerBuilderExtensions
{
    public static IProducerMiddlewareBuilder<T> UseSerializer<T>(this IProducerConfigBuilder builder,
        ISerializer<T> serializer, string? discriminator = null) where T : class?
        => builder.Middleware(Middlewares.DirectSerialize(serializer, discriminator));

    public static IProducerMiddlewareBuilder<T> UseSerializer<T>(this IProducerMiddlewareBuilder<byte[]?> builder,
        ISerializer<T> serializer, string? discriminator = null) where T : class?
        => builder.Middleware(Middlewares.DirectSerialize(serializer, discriminator));

    public static IConsumerConfigBuilder<T?> UseDeserializer<T>(this IConsumerConfigBuilder<byte[]?> builder,
        IDeserializer<T> deserializer) where T : class? => builder.Middleware(Middlewares.DirectDeserialize(deserializer));

    public static IConsumerHandlerBuilder<T?> UseDeserializer<T>(this IConsumerHandlerBuilder<byte[]?> builder,
        IDeserializer<T> deserializer) where T : class? => builder.Middleware(Middlewares.DirectDeserialize(deserializer));

    public static IProducerMiddlewareBuilder<string?> UsePlainText(this IProducerConfigBuilder builder)
        => builder.UseSerializer(new TextSerializer(), "string");

    public static IProducerMiddlewareBuilder<string?> UsePlainText(this IProducerMiddlewareBuilder<byte[]?> builder)
        => builder.UseSerializer(new TextSerializer(), "string");

    public static IConsumerConfigBuilder<string?> UsePlainText(this IConsumerConfigBuilder<byte[]?> builder)
        => builder.UseDeserializer(new TextSerializer());

    public static IConsumerHandlerBuilder<string?> UsePlainText(this IConsumerHandlerBuilder<byte[]?> builder)
        => builder.UseDeserializer(new TextSerializer());
}
