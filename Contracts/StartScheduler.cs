using NServiceBus;

namespace Contracts
{
    public class StartScheduler : ICommand
    {
        public string LegacySystemId => "MyLegacySystemNameGoesHere";
    }
}