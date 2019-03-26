using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Uragano.Consul;
using Uragano.Core;
using Uragano.Logging.Exceptionless;

namespace Shop.BasketService
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
                        builder.AddClient();
                        builder.AddExceptionlessLogger();
                        builder.AddConsul();
                    });
                    var rabbitHost = context.Configuration.GetValue<string>("RabbitMQ:Host");
                    var rabbitUser = context.Configuration.GetValue<string>("RabbitMQ:UserName");
                    var rabbitPassword = context.Configuration.GetValue<string>("RabbitMQ:Password");
                    service.AddCap(builder =>
                    {
                        builder.UseMySql(context.Configuration.GetConnectionString("DefaultConnection"));
                        builder.UseRabbitMQ(r =>
                        {
                            r.HostName = rabbitHost;
                            r.UserName = rabbitUser;
                            r.Password = rabbitPassword;
                            //r.ExchangeName = "exchange.basket.checkout";
                        });
                    });
                    service.AddSingleton<IConnectionFactory>(s => new ConnectionFactory
                    {
                        HostName = rabbitHost,
                        UserName = rabbitUser,
                        Password = rabbitPassword
                    });
                    var csRedis = new CSRedis.CSRedisClient(context.Configuration.GetValue<string>("RedisConnection"));
                    RedisHelper.Initialization(csRedis);
                }).ConfigureLogging((context, builder) =>
                {
                    builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                });
            await hostBuilder.RunConsoleAsync();
        }
    }
}
