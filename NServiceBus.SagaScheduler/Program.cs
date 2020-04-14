using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Contracts;

namespace NServiceBus.SagaScheduler
{
    class Program
    {

        static async Task Main(string[] args)
        {
            const string connectionString = "Data Source=.\\SqlExpress;Database=NsbSamplesSql;Integrated Security=True";

            Console.Title = "NServiceBus.SagaScheduler";

            var endpointConfiguration = new EndpointConfiguration("NServiceBus.SagaScheduler");
            endpointConfiguration.EnableInstallers();

            // Configure Transport
            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionString(connectionString);

            // Configure message routing
            var routingSettings = transport.Routing();
            routingSettings.RouteToEndpoint(typeof(StartScheduler), "NServiceBus.SagaScheduler");

            //Configure Persistence
            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.ConnectionBuilder(() => new SqlConnection(connectionString));


            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            Console.WriteLine("\nPress Enter to exit...\n\n\n");

            //Start up scheduler
            //----------------------------------------------
            await endpointInstance.Send(new StartScheduler()).ConfigureAwait(false);
            Console.WriteLine("Published StartScheduler message\n");
            //----------------------------------------------

            Console.ReadLine();

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }

        
    }
}
