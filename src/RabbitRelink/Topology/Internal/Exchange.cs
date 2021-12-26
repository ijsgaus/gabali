#region Usings

#endregion

namespace RabbitRelink.Topology.Internal
{
    internal class Exchange : IExchange
    {
        public Exchange(string name)
        {
            if (string.IsNullOrWhiteSpace(name) && name != "")
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        public string Name { get; }
    }
}
