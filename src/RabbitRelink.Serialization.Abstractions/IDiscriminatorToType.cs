namespace RabbitRelink.Serialization.Abstractions;

public interface IDiscriminatorToType
{
    string GetType(string discriminator);
}
