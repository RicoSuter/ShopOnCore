using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShopOnCore.Common.Messaging.Interfaces;

namespace ShopOnCore.Common.Messaging
{
    public abstract class MessageReceiver<TMessage, TRawMessage, TMessageHandler> : IHostedService
        where TMessageHandler : IMessageHandler<TMessage>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        protected MessageReceiver(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public abstract Task StartAsync(CancellationToken cancellationToken);

        public abstract Task StopAsync(CancellationToken cancellationToken);

        protected async Task<bool> HandleMessageAsync(TRawMessage rawMessage, CancellationToken cancellationToken)
        {
            using (var serviceScope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    await HandleMessageAsync(rawMessage, serviceScope, cancellationToken);
                    return true;
                }
                catch (Exception e)
                {
                    await HandleExceptionAsync(e, cancellationToken);
                    return false;
                }
            }
        }

        protected virtual Task HandleExceptionAsync(Exception exception, CancellationToken cancellationToken)
        {
            Console.Write(exception.ToString());
            return Task.CompletedTask;
        }

        protected virtual async Task HandleMessageAsync(TRawMessage rawMessage, IServiceScope serviceScope, CancellationToken cancellationToken)
        {
            var message = DeserializeMessage(rawMessage);

            var handler = ActivatorUtilities.CreateInstance<TMessageHandler>(serviceScope.ServiceProvider);
            await handler.HandleAsync(message, cancellationToken);
        }

        protected abstract TMessage DeserializeMessage(TRawMessage rawMessage);
    }
}