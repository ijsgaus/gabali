using System.Net.Http.Headers;

namespace RabbitRelink.Serialization.Abstractions;

public interface ISerializer
{
    Task<byte[]?> SerializeAsync<T>(T? value, CancellationToken token = default) where T : class;
    Task<byte[]?> SerializeAsync(object? value, CancellationToken token = default);
    MediaTypeHeaderValue MediaType { get; }
}

public interface ISerializer<T> where T : class?
{
    Task<byte[]?> SerializeAsync(T data, CancellationToken token = default);
    MediaTypeHeaderValue MediaType { get; }
}
