namespace RabbitRelink.Middlewares;

/// <summary>
/// Interface to operate with outgoing message in middleware chain
/// </summary>
/// <typeparam name="T">message body type</typeparam>
public interface IProducerMiddleware<T>
{
    /// <summary>
    /// Method to operate with outgoing message in middleware chain
    /// </summary>
    /// <param name="next">next handler in chain</param>
    /// <returns>this handler in chain</returns>
    DoPublish<T> Next(DoPublish<T> next);

    /// <summary>
    /// Convert to function representation
    /// </summary>
    /// <returns>Function to operate with outgoing message in middleware chain</returns>
    ProducerMiddleware<T> AsProducerMiddleware() => Next;
}
