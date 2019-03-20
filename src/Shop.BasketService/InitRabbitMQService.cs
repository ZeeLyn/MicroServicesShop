using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Shop.BasketService
{
    public class InitRabbitMQService : IHostedService
    {
        private IConnectionFactory ConnectionFactory { get; }

        private IConnection Connection { get; }

        private IModel Model { get; }

        public InitRabbitMQService(IConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
            Connection = ConnectionFactory.CreateConnection();
            Model = Connection.CreateModel();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

            Model.ExchangeDeclare("cap.default.router", "topic", true);
            Model.QueueDeclare("queue.basket.checkout", true, false, false);
            Model.QueueBind("queue.basket.checkout", "cap.default.router", "route.basket.checkout");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Connection.Close();
            Model.Close();
        }
    }
}
