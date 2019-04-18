using System;
using System.IO;
using System.Threading.Tasks;
using Dapper.Extensions;
using Dapper.Extensions.MySql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            var hostBuilder = new HostBuilder().ConfigureHostConfiguration(builder => { }).ConfigureAppConfiguration(
                    (context, builder) =>
                    {
                        builder.SetBasePath(Directory.GetCurrentDirectory());
                        builder.AddJsonFile($"appsettings.json", true, true);
                        builder.AddJsonFile("uragano.json", true, true);
                        builder.AddJsonFile($"uragano.{context.HostingEnvironment.EnvironmentName}.json", true, true);
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
                    service.AddHostedService<InitDataService>();
                    //DI service put before AddCap.
                    service.AddTransient<IGoodsSubscribeService, GoodsSubscriberService>();
                    var rabbitHost = context.Configuration.GetValue<string>("RabbitMQ:Host");
                    var rabbitUser = context.Configuration.GetValue<string>("RabbitMQ:UserName");
                    var rabbitPassword = context.Configuration.GetValue<string>("RabbitMQ:Password");
                    var capConn = context.Configuration.GetConnectionString("DefaultConnection");
                    service.AddCap(builder =>
                    {
                        builder.UseMySql(opt => { opt.ConnectionString = capConn; });
                        builder.UseRabbitMQ(r =>
                        {
                            r.HostName = rabbitHost;
                            r.UserName = rabbitUser;
                            r.Password = rabbitPassword;
                        });
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
