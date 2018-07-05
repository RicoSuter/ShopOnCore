using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace ShopOnCore.Common.Messaging.ServiceBus
{
    public class ServiceBusMessageProcessor<TMessage> : IHostedService
    {
        private readonly string _connectionString;
        private readonly string _entityPath;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private QueueClient _queueClient;

        public ServiceBusMessageProcessor(string connectionString, string entityPath, IServiceScopeFactory serviceScopeFactory)
        {
            _connectionString = connectionString;
            _entityPath = entityPath;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _queueClient = new QueueClient(_connectionString, _entityPath);
            var messageHandlerOptions = CreateMessageHandlerOptions();

            _queueClient.RegisterMessageHandler(async (message, token) =>
            {
                await HandleMessageAsync(message, _queueClient, token);
            }, messageHandlerOptions);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
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

        protected virtual async Task HandleMessageAsync(Message rawMessage, IServiceScope serviceScope, CancellationToken cancellationToken)
        {
            var message = DeserializeMessage(rawMessage);

            var handler = serviceScope.ServiceProvider.GetRequiredService<IMessageHandler<TMessage>>();
            await handler.HandleAsync(message, cancellationToken);
        }

        protected virtual TMessage DeserializeMessage(Message rawMessage)
        {
            var json = Encoding.UTF8.GetString(rawMessage.Body);
            return JsonConvert.DeserializeObject<TMessage>(json);
        }

        private async Task HandleMessageAsync(Message rawMessage, QueueClient queueClient, CancellationToken cancellationToken)
        {
            using (var serviceScope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    await HandleMessageAsync(rawMessage, serviceScope, cancellationToken);
                    await queueClient.CompleteAsync(rawMessage.SystemProperties.LockToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}