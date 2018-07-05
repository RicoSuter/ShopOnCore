using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShopOnCore.Common.Messaging;
using ShopOnCore.Common.Messaging.ServiceBus;
using ShopOnCore.Orders.App.Services;
using ShopOnCore.Orders.Contract;

namespace ShopOnCore.Orders.App
{
    class Program
    {
        public static void Main(string[] args)
        {
            var builder = new HostBuilder()
              .ConfigureAppConfiguration((hostingContext, config) =>
              {
                  config.AddJsonFile("appsettings.json", optional: true);
                  config.AddEnvironmentVariables();

                  if (args != null)
                      config.AddCommandLine(args);
              })
              .ConfigureServices((hostContext, services) =>
              {
                  services.AddOptions();

                  // Register ServiceBusMessageProcessor
                  services.AddScoped<IMessageHandler<CreateOrderMessage>, CreateOrderMessageHandler>();
                  services.AddSingleton<IHostedService, ServiceBusMessageProcessor<CreateOrderMessage>>(
                      serviceProvider => new ServiceBusMessageProcessor<CreateOrderMessage>(
                          hostContext.Configuration.GetSection("ServiceBus")["ConnectionString"],
                          hostContext.Configuration.GetSection("ServiceBus")["EntityPath"],
                          serviceProvider.GetRequiredService<IServiceScopeFactory>()));
              })
              .ConfigureLogging((hostingContext, logging) =>
              {
                  logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                  logging.AddConsole();
              });

            builder.RunConsoleAsync().GetAwaiter().GetResult();
        }
    }
}
