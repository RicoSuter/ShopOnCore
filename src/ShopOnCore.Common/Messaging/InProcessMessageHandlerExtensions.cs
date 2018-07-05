using Microsoft.Extensions.DependencyInjection;

namespace ShopOnCore.Common.Messaging
{
    public static class InProcessMessageHandlerExtensions
    {
        public static void AddInProcessMessageHandler<TMessageHandler, TMessage>(this IServiceCollection serviceCollection)
            where TMessageHandler : class, IMessageHandler<TMessage>
        {
            serviceCollection.AddScoped<TMessageHandler>();
            serviceCollection.AddScoped<IMessageSender<TMessage>>(provider =>
            {
                return new InProcessMessageSender<TMessage, TMessageHandler>(
                    provider.GetRequiredService<TMessageHandler>());
            });
        }
    }
}