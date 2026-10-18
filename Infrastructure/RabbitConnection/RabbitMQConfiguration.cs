using RabbitMQ.Client;
using System.Text;
using MWSMan.Core.Model.RabbitModel;
using MWSMan.Core.Infrastructure.LoggManager.LoggManager;
using MWSMan.Core.Persistence.StaticConfiguration;

namespace RabbitMq_InfrastructureRabbitConnection
{
    public class RabbitMQConfiguration
    {
        private IChannel _channel;
        private IConnection connection;
        public VHostModel VHost;
        public UserModel user;
        private readonly IMinooConfiguration configuration;
        private readonly IMinooLogger minoologger;

        public RabbitMQConfiguration(UserModel user, VHostModel vHost, IMinooConfiguration configuration, IMinooLogger minoologger)
        {
            this.user = user;
            VHost = vHost;
            this.configuration = configuration;
            this.minoologger = minoologger;
        }

        public VHostModel GetVHost()
        {
            return VHost;
        }

        public async Task CreateQueueWithBinding(ExchangeModel exchange, QueueModel queue, BindingModel binding, string jsonBody, VHostModel vHost)
        {
            var rabbitConfigure = new RabbitConnectionManager(vHost, user,minoologger,configuration);
            connection = await rabbitConfigure.ConnectionGenerateAsync();
            _channel = await connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(
               exchange: exchange.Name,
               type: exchange.Type,
               durable: exchange.Durable,
               autoDelete: exchange.AutoDelete,
               arguments: exchange.Arguments
           );
            await _channel.QueueDeclareAsync(
               queue: queue.name,
               durable: queue.durable,
               exclusive: false,
               autoDelete: queue.autoDelete
           //arguments: queue.arguments
           );
            await _channel.QueueBindAsync(
                queue: queue.name,
                exchange: exchange.Name,
                routingKey: binding.RoutingKey ?? "",
                arguments: binding.Arguments
            );

            var ReadOnlyMemory = new ReadOnlyMemory<byte>(Encoding.ASCII.GetBytes(jsonBody));
            await _channel.BasicPublishAsync(exchange.Name, binding.RoutingKey ?? "", ReadOnlyMemory);

        }
    }
}
