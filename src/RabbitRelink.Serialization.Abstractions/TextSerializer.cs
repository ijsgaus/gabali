using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;

namespace RabbitRelink.Serialization.Abstractions;

public class TextSerializer : ISerializer<string?>, IDeserializer<string?>
{
    public Task<byte[]?> SerializeAsync(string? data, CancellationToken token = default)
        => Task.FromResult(data == null
            ? null
            : Encoding.UTF8.GetBytes(data));

    public MediaTypeHeaderValue MediaType { get; } = new(MediaTypeNames.Text.Plain)
    {
        CharSet = Encoding.UTF8.WebName
    };

    public Task<string?> DeserializeAsync(byte[]? data, CancellationToken token = default)
        => Task.FromResult(data == null
            ? null
            : Encoding.UTF8.GetString(data));
}
