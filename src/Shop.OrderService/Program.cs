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
                    //SeedData.Initialize(service);
                    //service.AddHostedService<InitDataService>();
                    var rabbitHost = context.Configuration.GetValue<string>("RabbitMQ:Host");
                    var rabbitUser = context.Configuration.GetValue<string>("RabbitMQ:UserName");
                    var rabbitPassword = context.Configuration.GetValue<string>("RabbitMQ:Password");
                    var capConn = context.Configuration.GetConnectionString("DefaultConnection");
                    //注意: 注入的服务需要在 `services.AddCap()` 之前
                    service.AddTransient<ISubscriberService, OrderSubscriberService>();
                    service.AddCap(builder =>
                    {
                        builder.UseMySql(opt =>
                        {
                            opt.ConnectionString = capConn;
                        });
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
                });
            await hostBuilder.RunConsoleAsync();
        }
    }
}
