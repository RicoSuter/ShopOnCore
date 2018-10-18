using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using ShopOnCore.Common.Messaging.Interfaces;

namespace ShopOnCore.Common.Messaging.ServiceBus
{
    public class ServiceBusMessageSender<TMessage> : IMessageSender<TMessage>
    {
        private readonly QueueClient _queueClient;

        public ServiceBusMessageSender(string connectionString, string entityPath)
        {
            _queueClient = new QueueClient(connectionString, entityPath);
        }

        public async Task SendMessageAsync(TMessage message, CancellationToken cancellationToken)
        {
            var bytes = SerializeMessage(message);

            var rawMessage = new Message(bytes);
            await _queueClient.SendAsync(rawMessage);
        }

        protected virtual byte[] SerializeMessage(TMessage message)
        {
            var json = JsonConvert.SerializeObject(message, Formatting.Indented);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}