using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace ShopOnCore.Common.Messaging.Extensions
{
    public static class MessageReceiverExtensions
    {
        public static void AddMessageReceiver<TReceiver>(this IServiceCollection serviceCollection, Func<IServiceProvider, IServiceScopeFactory, TReceiver> factory)
            where TReceiver : class, IHostedService
        {
            serviceCollection.AddSingleton<IHostedService, TReceiver>(serviceFactory => factory(serviceFactory, serviceFactory.GetRequiredService<IServiceScopeFactory>()));
        }
    }
}