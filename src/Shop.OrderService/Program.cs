using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Uragano.Core;
using Uragano.Logging.Exceptionless;
using Uragano.Consul;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Dapper.Extensions;
using Dapper.Extensions.MySql;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging;

namespace Shop.OrderService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder().ConfigureHostConfiguration(builder => { }).ConfigureAppConfiguration(
                    (context, builder) =>
                    {
                        builder.SetBasePath(Directory.GetCurrentDirectory());
                        builder.AddJsonFile("appsettings.json", true, true);
                        builder.AddJsonFile("uragano.json", true, true);
                        builder.AddJsonFile($"uragano.{context.HostingEnvironment.EnvironmentName}.json");
                        builder.AddCommandLine(args);
                    })
                .ConfigureServices((context, service) =>
                {
                    service.AddUragano(context.Configuration, builder =>
                    {
                        builder.AddServer();
                        builder.AddExceptionlessLogger();
                        builder.AddConsul();
                    });
                    service.AddScoped<IDapper, MySqlDapper>();

                }).ConfigureLogging((context, builder) =>
                {
                    builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                });
            await hostBuilder.RunConsoleAsync();
        }
    }
}
