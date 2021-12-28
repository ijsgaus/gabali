namespace RabbitRelink.Middlewares;

/// <summary>
/// Interface to serialize outgoing message
/// </summary>
/// <typeparam name="T">serialized type</typeparam>
public interface ISerializer<T>
{
    /// <summary>
    /// Method to serialize outgoing message
    /// </summary>
    /// <param name="next">next handler in chain<</param>
    /// <returns>this handler in chain</returns>
    DoPublish<T> Serialize(DoPublish<byte[]> next);

    /// <summary>
    /// Convert to function representation
    /// </summary>
    /// <returns>Function to serialize outgoing message</returns>
    public Serialize<T> AsDelegate() => Serialize;
}
