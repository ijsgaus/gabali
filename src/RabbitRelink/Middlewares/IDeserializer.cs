using RabbitRelink.Consumer;
using RabbitRelink.Messaging;

namespace RabbitRelink.Middlewares;

/// <summary>
/// Interface to deserialize incoming message
/// </summary>
/// <typeparam name="T">deserialized type</typeparam>
public interface IDeserializer<T>
where T: class?
{
    /// <summary>
    /// Method to deserialize incoming message
    /// </summary>
    /// <param name="next">next handler in chain<</param>
    /// <returns>this handler in chain</returns>
    DoConsume<byte[]> Deserialize(DoConsume<T> next);

    /// <summary>
    /// Convert to function representation
    /// </summary>
    /// <returns>Function to deserialize incoming message</returns>
    Deserializer<T> AsDelegate() => Deserialize;

}
