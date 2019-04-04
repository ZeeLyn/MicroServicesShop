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
using System.Net;
using System.Net.Mail;

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
                    service.AddHostedService<InitDataService>();
                    service.AddUragano(context.Configuration, builder =>
                    {
                        builder.AddServer();
                        builder.AddClient();
                        builder.AddExceptionlessLogger();
                        builder.AddConsul();
                    });
                    service.AddScoped<IDapper, MySqlDapper>();

                    var rabbitHost = context.Configuration.GetValue<string>("RabbitMQ:Host");
                    var rabbitUser = context.Configuration.GetValue<string>("RabbitMQ:UserName");
                    var rabbitPassword = context.Configuration.GetValue<string>("RabbitMQ:Password");
                    var capConn = context.Configuration.GetConnectionString("DefaultConnection");
                    //注意: 注入的服务需要在 `services.AddCap()` 之前
                    service.AddTransient<ISubscriberService, OrderSubscriberService>();
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
                    //Email
                    var defaultFromEmail = context.Configuration.GetValue<string>("Email:DefaultFromEmail");
                    var emailHost = context.Configuration.GetValue<string>("Email:Host");
                    var emailPort = context.Configuration.GetValue<int>("Email:Port");
                    var emailUserName = context.Configuration.GetValue<string>("Email:UserName");
                    var emailPassword = context.Configuration.GetValue<string>("Email:Password");
                    service.AddFluentEmail(defaultFromEmail).AddSmtpSender(new SmtpClient
                    {
                        Host = emailHost,
                        Port = emailPort,
                        Credentials = new NetworkCredential(emailUserName, emailPassword)
                    });
                }).ConfigureLogging((context, builder) =>
                {
                    builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                });
            await hostBuilder.RunConsoleAsync();
        }
    }
}
