using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

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

            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionString(connectionString);

            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.ConnectionBuilder(() => new SqlConnection(connectionString));

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }

        
    }
}
