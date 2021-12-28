namespace RabbitRelink.Middlewares;

/// <summary>
/// Function to operate with incoming message in middleware chain
/// </summary>
/// <typeparam name="T">message body type</typeparam>
public delegate DoConsume<T> ConsumerMiddleware<T>(DoConsume<T> next);
