namespace RabbitRelink.Middlewares;

/// <summary>
/// Function to deserialize incoming message
/// </summary>
/// <typeparam name="T">deserialized type</typeparam>
public delegate DoConsume<byte[]> Deserializer<T>(DoConsume<T> next);
