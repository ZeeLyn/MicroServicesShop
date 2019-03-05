using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Uragano.Consul;
using Uragano.Core;
using Uragano.Logging.Exceptionless;

namespace Shop.GoodsService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder().ConfigureHostConfiguration(builder =>
                {
                }).ConfigureAppConfiguration((context, builder) =>
                {
                    Console.WriteLine("-------------------->" + context.HostingEnvironment.EnvironmentName);
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    var json = Path.Combine(Directory.GetCurrentDirectory(),
                        $"uragano.{context.HostingEnvironment.EnvironmentName}.json");
                    Console.WriteLine(json);
                    builder.AddJsonFile(json, true, true);
                    builder.AddCommandLine(args);
                })
                .ConfigureServices((context, service) =>
                {
                    Console.WriteLine("-------------------------------------------->" +
                                      context.Configuration.GetValue<string>("Uragano:ServiceDiscovery:Consul:Client:Address"));
                    service.AddUragano(context.Configuration, builder =>
                    {
                        builder.AddServer();
                        builder.AddExceptionlessLogger();
                        builder.AddConsul();
                    });
                }).ConfigureLogging((context, builder) =>
                {
                    builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                    builder.AddConsole();
                });
            await hostBuilder.RunConsoleAsync();
        }
    }
}
