using RabbitRelink.Consumer;
using RabbitRelink.Messaging;

namespace RabbitRelink;

/// <summary>
/// Function to receive message from consumer
/// </summary>
/// <typeparam name="T">Message body type</typeparam>
public delegate Task<Acknowledge> DoConsume<T>(ConsumedMessage<T> msg);
