using System.IO;
using System.Threading.Tasks;
using Dapper.Extensions;
using Dapper.Extensions.MySql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ocelot.JwtAuthorize;
using Uragano.Consul;
using Uragano.Core;
using Uragano.Logging.Exceptionless;

namespace Shop.IdentityService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder().ConfigureHostConfiguration(builder =>
                {
                }).ConfigureAppConfiguration((context, builder) =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile($"appsettings.json", true, true);
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
                    service.AddTokenJwtAuthorize();
                    service.AddScoped<IDapper, MySqlDapper>();

                }).ConfigureLogging((context, builder) =>
                {
                    builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                });
            await hostBuilder.RunConsoleAsync();
        }
    }
}
