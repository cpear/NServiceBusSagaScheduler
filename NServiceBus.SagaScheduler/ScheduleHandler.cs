using System;
using System.Threading.Tasks;
using Contracts;

namespace NServiceBus.SagaScheduler
{
    public class ScheduleHandler : 
        Saga<ScheduleData>,
        IAmStartedByMessages<StartScheduler>,
        IHandleTimeouts<SchedulerTimeOut>
    {
        private static readonly TimeSpan TimeToCheckAgain = TimeSpan.FromSeconds(1);

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ScheduleData> mapper)
        {
            mapper.ConfigureMapping<StartScheduler>(message => message.LegacySystemId)
                .ToSaga(sagaData => sagaData.LegacySystemId);
        }

        public Task Handle(StartScheduler message, IMessageHandlerContext context)
        {
            Console.WriteLine("Received Start the Scheduler Message.");
            if (Data.IsStarted)
            {
                Console.WriteLine("The scheduler has been previously started\n\n");
                return Task.CompletedTask;
            }

            IsLegacyDataUpdated(context);

            Data.IsStarted = true;

            return RequestTimeout<SchedulerTimeOut>(context, TimeToCheckAgain);
        }

        public Task Timeout(SchedulerTimeOut state, IMessageHandlerContext context)
        {
            //Got a timeout message. Recheck legacy system.
            Console.WriteLine("Picked up a Timeout message");
            IsLegacyDataUpdated(context);
            
            //Send Timeout Request
            return RequestTimeout<SchedulerTimeOut>(context, TimeToCheckAgain);
        }


        private static void IsLegacyDataUpdated(IMessageHandlerContext context)
        {
            Console.WriteLine("Checking legacy system for new data\n");
            //----------------------------------------------------
            //Code to check if legacy system is updated goes here
            var dataUpdated = false;
            //----------------------------------------------------

            if (dataUpdated)
            {
                context.Send(new LegacyDataUpdated()).ConfigureAwait(false);
            }
        }
    }



    /// <summary>
    /// Send This Event to notify everyone that this Legacy System Has Updated Data
    /// </summary>
    public class LegacyDataUpdated : IEvent
    {
    }

    /// <summary>
    /// TimeOut message used by the saga
    /// </summary>
    public class SchedulerTimeOut : IMessage
    {
    }


    /// <summary>
    /// Data for the saga
    /// </summary>
    public class ScheduleData : IContainSagaData
    {
        public string LegacySystemId { get; set; }
        public bool IsStarted { get; set; }
        



        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }

    }
}