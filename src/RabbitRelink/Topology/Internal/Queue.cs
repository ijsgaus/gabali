#region Usings

#endregion

namespace RabbitRelink.Topology.Internal
{
    internal class Queue : IQueue
    {
        public Queue(string name, bool isExclusive)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
            IsExclusive = isExclusive;
        }

        public string Name { get; }

        public bool IsExclusive { get; }
    }
}
