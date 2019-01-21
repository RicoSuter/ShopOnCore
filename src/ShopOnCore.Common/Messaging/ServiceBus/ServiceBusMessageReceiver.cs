using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ShopOnCore.Common.Messaging.Interfaces;

namespace ShopOnCore.Common.Messaging.ServiceBus
{
    public class ServiceBusMessageReceiver<TMessage, TMessageHandler> : MessageReceiver<TMessage, Message, TMessageHandler>
        where TMessageHandler : IMessageHandler<TMessage>
    {
        private readonly string _connectionString;
        private readonly string _entityPath;

        private QueueClient _queueClient;

        public ServiceBusMessageReceiver(string connectionString, string entityPath, IServiceScopeFactory serviceScopeFactory)
            : base(serviceScopeFactory)
        {
            _connectionString = connectionString;
            _entityPath = entityPath;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _queueClient = new QueueClient(_connectionString, _entityPath);
            var messageHandlerOptions = CreateMessageHandlerOptions();

            _queueClient.RegisterMessageHandler(async (message, token) =>
            {
                var result = await HandleMessageAsync(message, false, token);
                if (result)
                {
                    await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
                }
            }, messageHandlerOptions);

            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _queueClient.CloseAsync();
        }

        protected virtual MessageHandlerOptions CreateMessageHandlerOptions()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            return messageHandlerOptions;
        }

        protected override TMessage DeserializeMessage(Message rawMessage)
        {
            var json = Encoding.UTF8.GetString(rawMessage.Body);
            return JsonConvert.DeserializeObject<TMessage>(json);
        }

        private async Task ExceptionReceivedHandler(ExceptionReceivedEventArgs args)
        {
            await HandleExceptionAsync(args.Exception, CancellationToken.None);
        }
    }
}