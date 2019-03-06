using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dapper.Extensions;
using Dapper.Extensions.MySql;
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
                    builder.AddJsonFile($"appsettings.json", true, true);
                    builder.AddJsonFile($"uragano.{context.HostingEnvironment.EnvironmentName}.json", true, true);
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
                }).UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>(builder =>
                    {
                        builder.RegisterType<MySqlDapper>().As<IDapper>().WithParameter("connectionName", "mysql")
                            .PropertiesAutowired().InstancePerLifetimeScope();
                    }).ConfigureLogging((context, builder) =>
                {
                    builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                    builder.AddConsole();
                });
            await hostBuilder.RunConsoleAsync();
        }
    }
}
