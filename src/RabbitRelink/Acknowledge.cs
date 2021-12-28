namespace RabbitRelink;

/// <summary>
/// Consumer ACK strategies
/// </summary>
public abstract record Acknowledge()
{
    /// <summary>
    /// ACK acknowledge type
    /// </summary>
    private sealed record AckAcknowledge() : Acknowledge();

    /// <summary>
    /// NACK acknowledge type
    /// </summary>
    private sealed record NackAcknowledge() : Acknowledge();

    /// <summary>
    /// REQUEUE acknowledge type
    /// </summary>
    private sealed record RequeueAcknowledge() : Acknowledge();

    /// <summary>
    /// ACK acknowledge
    /// </summary>
    public static Acknowledge Ack { get; } = new AckAcknowledge();

    /// <summary>
    /// NACK acknowledge
    /// </summary>
    public static Acknowledge Nack { get; } = new NackAcknowledge();

    /// <summary>
    /// REQUEUE acknowledge
    /// </summary>
    public static Acknowledge Requeue { get; } = new RequeueAcknowledge();

    /// <summary>
    /// Execute action on ACK, NACK, REQUEUE result
    /// </summary>
    /// <param name="onAck">on ACC action</param>
    /// <param name="onNack">on NACK action</param>
    /// <param name="onRequeue">on REQUEUE action</param>
    /// <exception cref="ArgumentOutOfRangeException">If other acknowledge</exception>
    public void Match(Action onAck, Action onNack, Action onRequeue)
    {
        switch (this)
        {
            case AckAcknowledge ackAcknowledge:
                onAck();
                break;
            case NackAcknowledge nackAcknowledge:
                onNack();
                break;
            case RequeueAcknowledge requeueAcknowledge:
                onRequeue();
                break;
            default:
                Fn.ThrowUnknownCase<Acknowledge, ValueTuple>(this);
                break;
        }
    }

    /// <summary>
    /// Execute action on ACK, NACK, REQUEUE and OTHER result
    /// </summary>
    /// <param name="onAck">on ACC action</param>
    /// <param name="onNack">on NACK action</param>
    /// <param name="onRequeue">on REQUEUE action</param>
    /// <param name="onOther">ont OTHER action</param>
    public void Match(Action onAck, Action onNack, Action onRequeue, Action<Acknowledge> onOther)
    {
        switch (this)
        {
            case AckAcknowledge ackAcknowledge:
                onAck();
                break;
            case NackAcknowledge nackAcknowledge:
                onNack();
                break;
            case RequeueAcknowledge requeueAcknowledge:
                onRequeue();
                break;
            default:
                onOther(this);
                break;
        }
    }

    /// <summary>
    /// Execute function on ACK, NACK, REQUEUE result
    /// </summary>
    /// <typeparam name="T">function result type</typeparam>
    /// <param name="onAck">on ACC function</param>
    /// <param name="onNack">on NACK function</param>
    /// <param name="onRequeue">on REQUEUE function</param>
    /// <exception cref="ArgumentOutOfRangeException">If other acknowledge</exception>
    public T Match<T>(Func<T> onAck, Func<T> onNack, Func<T> onRequeue)
        => this switch
        {
            AckAcknowledge ackAcknowledge => onAck(),
            NackAcknowledge nackAcknowledge => onNack(),
            RequeueAcknowledge requeueAcknowledge => onRequeue(),
            _ => Fn.ThrowUnknownCase<Acknowledge, T>(this)
        };

    /// <summary>
    /// Execute function on ACK, NACK, REQUEUE and OTHER result
    /// </summary>
    /// <typeparam name="T">function result type</typeparam>
    /// <param name="onAck">on ACC function</param>
    /// <param name="onNack">on NACK function</param>
    /// <param name="onRequeue">on REQUEUE function</param>
    /// <param name="onOther">ont OTHER function</param>
    public T Match<T>(Func<T> onAck, Func<T> onNack, Func<T> onRequeue, Func<Acknowledge, T> onOther)
        => this switch
        {
            AckAcknowledge ackAcknowledge => onAck(),
            NackAcknowledge nackAcknowledge => onNack(),
            RequeueAcknowledge requeueAcknowledge => onRequeue(),
            _ => onOther(this)
        };
}
