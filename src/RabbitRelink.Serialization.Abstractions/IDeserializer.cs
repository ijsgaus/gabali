using System.Net.Http.Headers;

namespace RabbitRelink.Serialization.Abstractions;

public interface IDeserializer
{
    Task<object?> DeserializeAsync(byte[]? data, Type dataType, CancellationToken token = default);
    Task<T?> DeserializeAsync<T>(byte[]? data, CancellationToken token = default) where T : class;
    MediaTypeHeaderValue MediaType { get; }
}

public interface IDeserializer<T> where T : class?
{
    Task<T> DeserializeAsync(byte[]? data, CancellationToken token = default);
    MediaTypeHeaderValue MediaType { get; }
}
