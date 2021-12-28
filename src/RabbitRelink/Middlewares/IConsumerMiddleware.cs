namespace RabbitRelink.Middlewares;


/// <summary>
/// Interface to operate with incoming message in middleware chain
/// </summary>
/// <typeparam name="T">message body type</typeparam>
public interface IConsumerMiddleware<T>
{
    /// <summary>
    /// Method to operate with incoming message in middleware chain
    /// </summary>
    /// <param name="next">next handler in chain</param>
    /// <returns>this handler in chain</returns>
    DoConsume<T> Next(DoConsume<T> next);

    /// <summary>
    /// Convert to function representation
    /// </summary>
    /// <returns>Function to operate with incoming message in middleware chain</returns>
    ConsumerMiddleware<T> AsConsumerMiddleware() => Next;

}
