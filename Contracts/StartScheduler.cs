using NServiceBus;

namespace Contracts
{
    public class StartScheduler : ICommand
    {
        public string LegacySystemId => "Commodore 64 Legacy System";
    }
}