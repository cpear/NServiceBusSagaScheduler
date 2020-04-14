using System;
using System.Threading.Tasks;
using Contracts;
using NServiceBus;

namespace Sender
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Sender";

            var endpointConfiguration = new EndpointConfiguration("Sender");
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.SendOnly();

            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionString("Data Source=.\\SqlExpress;Database=NsbSamplesSql;Integrated Security=True");

            // Configure message routing
            var routingSettings = transport.Routing();
            routingSettings.RouteToEndpoint(typeof(StartScheduler), "NServiceBus.SagaScheduler");

//            var endpointInstance = await Endpoint.Start(endpointConfiguration)
//                .ConfigureAwait(false);

            var startableEndpoint = Endpoint.Create(endpointConfiguration)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            var endpointInstance = startableEndpoint.Start()
                .ConfigureAwait(false).GetAwaiter().GetResult();


            Console.WriteLine("Press enter to send a message");
            Console.WriteLine("Press any key to exit");
            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();

                if (key.Key != ConsoleKey.Enter)
                {
                    break;
                }

                //Start up scheduler
                //----------------------------------------------
                await endpointInstance.Send(new StartScheduler()).ConfigureAwait(false);
                //----------------------------------------------

                Console.WriteLine("Published StartScheduler message\n");
            }
            

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
