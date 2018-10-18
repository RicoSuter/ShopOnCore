using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using ShopOnCore.Common.Messaging.Interfaces;

namespace ShopOnCore.Common.Messaging.Rabbit
{
    public class RabbitMessageSender<TMessage> : IMessageSender<TMessage>
    {
        private readonly RabbitConfiguration _configuration;

        public RabbitMessageSender(RabbitConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendMessageAsync(TMessage message, CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration.Host,
                UserName = _configuration.Username,
                Password = _configuration.Password,
                DispatchConsumersAsync = true
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(_configuration.ExchangeName, ExchangeType.Direct);
                channel.QueueDeclare(_configuration.QueueName, true, false, false, null);
                channel.QueueBind(_configuration.QueueName, _configuration.ExchangeName, _configuration.Routingkey, null);

                channel.BasicPublish(
                    exchange: _configuration.ExchangeName,
                    routingKey: _configuration.Routingkey,
                    basicProperties: null,
                    body: SerializeMessage(message));
            }

            return Task.CompletedTask;
        }

        protected virtual byte[] SerializeMessage(TMessage message)
        {
            var json = JsonConvert.SerializeObject(message, Formatting.Indented);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}