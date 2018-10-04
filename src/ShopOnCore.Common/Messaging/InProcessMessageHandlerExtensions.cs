using Microsoft.Extensions.DependencyInjection;

namespace ShopOnCore.Common.Messaging
{
    public static class InProcessMessageHandlerExtensions
    {
        /// <summary>
        /// Registers a non-blocking in-process message sender.
        /// </summary>
        public static void AddInProcessMessageSender<TMessage, TMessageHandler>(this IServiceCollection serviceCollection)
            where TMessageHandler : class, IMessageHandler<TMessage>
        {
            serviceCollection.AddScoped<IMessageSender<TMessage>>(provider =>
            {
                var handler = ActivatorUtilities.CreateInstance<TMessageHandler>(provider);
                return new InProcessMessageSender<TMessage, TMessageHandler>(handler);
            });
        }

        /// <summary>
        /// Registers a blocking in-process message sender (SendMessageAsync blocks until the message has been processed).
        /// </summary>
        public static void AddBlockingInProcessMessageSender<TMessage, TMessageHandler>(this IServiceCollection serviceCollection)
            where TMessageHandler : class, IMessageHandler<TMessage>
        {
            serviceCollection.AddScoped<IMessageSender<TMessage>>(provider =>
            {
                var handler = ActivatorUtilities.CreateInstance<TMessageHandler>(provider);
                return new InProcessMessageSender<TMessage, TMessageHandler>(handler, true);
            });
        }
    }
}