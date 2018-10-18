using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ShopOnCore.Common.Messaging.Interfaces;

namespace ShopOnCore.Common.Messaging.Extensions
{
    public static class MessageSenderExtensions
    {
        public static void AddMessageSender<TMessage>(this IServiceCollection serviceCollection, 
            Func<IServiceProvider, IMessageSender<TMessage>> factory)
        {
            serviceCollection.TryAddScoped(factory);
        }
    }
}