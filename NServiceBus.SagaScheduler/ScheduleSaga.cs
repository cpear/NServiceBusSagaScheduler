using System;
using System.Threading.Tasks;
using Contracts;

namespace NServiceBus.SagaScheduler
{
    public class ScheduleSaga : 
        Saga<ScheduleData>,
        IAmStartedByMessages<StartScheduler>,
        IHandleTimeouts<SchedulerTimeOut>
    {
        //This represents how long the scheduler should wait to kick the job off again
        private static readonly TimeSpan TimeToCheckAgain = TimeSpan.FromSeconds(1);

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ScheduleData> mapper)
        {
            mapper
                .ConfigureMapping<StartScheduler>(message => message.LegacySystemId)
                .ToSaga(sagaData => sagaData.LegacySystemId);
        }

        public Task Handle(StartScheduler message, IMessageHandlerContext context)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Received Start the Scheduler Message.");
            if (Data.IsStarted)
            {
                Console.WriteLine("The scheduler has already been previously started. Standing by for scheduled tasks....\n\n");
                return Task.CompletedTask;
            }

            IsLegacyDataUpdated(context);

            Data.IsStarted = true;

            return RequestTimeout<SchedulerTimeOut>(context, TimeToCheckAgain);
        }

        public Task Timeout(SchedulerTimeOut state, IMessageHandlerContext context)
        {
            //Got a timeout message. Recheck legacy system.
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"[{DateTime.Now:T}] Running legacy system job. Timeout requested at {state.RequestedAt:T}");
            IsLegacyDataUpdated(context);
            
            //Send Timeout Request
            return RequestTimeout<SchedulerTimeOut>(context, TimeToCheckAgain);
        }

        /// <summary>
        /// This scheduled job checks a legacy system for updated data
        /// </summary>
        private void IsLegacyDataUpdated(IMessageHandlerContext context)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Checking for updated data in: {Data.LegacySystemId}\n");
            //----------------------------------------------------
            //Code to check if legacy system is updated goes here
            var isDataUpdated = false;
            //----------------------------------------------------

            if (isDataUpdated)
            {
                context.Send(new LegacyDataUpdated()).ConfigureAwait(false);
            }
        }
    }


    /// <summary>
    /// TimeOut message used by the saga
    /// </summary>
    public class SchedulerTimeOut : IMessage
    {
        public DateTime RequestedAt { get; set; } = DateTime.Now;
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