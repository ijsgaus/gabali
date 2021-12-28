namespace RabbitRelink.Serialization.Abstractions;

public interface ITypeToDiscriminator
{
    string GetDiscriminator(Type type);
}
