using NServiceBus;

namespace Contracts
{
    /// <summary>
    /// Send This Event to notify everyone that this Legacy System Has Updated Data
    /// </summary>
    public class LegacyDataUpdated : IEvent
    {
    }
}