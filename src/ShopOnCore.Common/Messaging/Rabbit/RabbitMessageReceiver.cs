using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ShopOnCore.Common.Messaging.Rabbit
{
    public class RabbitMessageReceiver<TMessage> : MessageReceiver<TMessage, BasicDeliverEventArgs>
    {
        private readonly RabbitConfiguration _configuration;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMessageReceiver(RabbitConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
            : base(serviceScopeFactory)
        {
            _configuration = configuration;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration.Host,
                UserName = _configuration.Username,
                Password = _configuration.Password,
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();

            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(_configuration.ExchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(_configuration.QueueName, true, false, false, null);
            _channel.QueueBind(_configuration.QueueName, _configuration.ExchangeName, _configuration.Routingkey, null);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (o, a) =>
            {
                var result = await HandleMessageAsync(a, CancellationToken.None);
                if (result)
                {
                    _channel.BasicAck(a.DeliveryTag, false);
                }
                else
                {
                    _channel.BasicReject(a.DeliveryTag, true);
                }
            };

            _channel.BasicConsume(_configuration.QueueName, false, consumer);

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Dispose();
            _connection?.Dispose();

            return Task.CompletedTask;
        }

        protected override TMessage DeserializeMessage(BasicDeliverEventArgs rawMessage)
        {
            var json = Encoding.UTF8.GetString(rawMessage.Body);
            return JsonConvert.DeserializeObject<TMessage>(json);
        }
    }
}