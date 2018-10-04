using Microsoft.Extensions.DependencyInjection;

namespace ShopOnCore.Common.Messaging
{
    public static class InProcessMessageHandlerExtensions
    {
        public static void AddInProcessMessageSender<TMessage, TMessageHandler>(this IServiceCollection serviceCollection)
            where TMessageHandler : class, IMessageHandler<TMessage>
        {
            serviceCollection.AddScoped<IMessageSender<TMessage>>(provider =>
            {
                var handler = ActivatorUtilities.CreateInstance<TMessageHandler>(provider);
                return new InProcessMessageSender<TMessage, TMessageHandler>(handler);
            });
        }
    }
}