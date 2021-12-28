namespace RabbitRelink.Middlewares;

/// <summary>
/// Function to serialize outgoing message
/// </summary>
/// <typeparam name="T">serialized type</typeparam>
public delegate DoPublish<T> Serialize<T>(DoPublish<byte[]> next);
