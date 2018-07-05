using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ShopOnCore.Common.Messaging
{
    public static class MessageSenderExtensions
    {
        public static void AddMessageSender<TMessage>(this IServiceCollection serviceCollection, 
            Func<IServiceProvider, IMessageSender<TMessage>> factory)
        {
            serviceCollection.TryAddScoped(typeof(IMessageSender<TMessage>), factory);
        }
    }
}