using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShopOnCore.Common.Messaging;
using ShopOnCore.Common.Messaging.Rabbit;
using ShopOnCore.Orders.App.Services;
using ShopOnCore.Orders.Contract;
using ShopOnCore.Orders.Services;

namespace ShopOnCore.Orders.App
{
    public class Program
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

                  // Register custom services
                  services.TryAddScoped<IOrdersService, OrdersService>();

                  // Register hosted service (entry point)
                  services.AddMessageReceiver((serviceProvider, scopeFactory) => new RabbitMessageReceiver<CreateOrderMessage, CreateOrderMessageHandler>(
                      new RabbitConfiguration { Host = "rabbit_queue" }, scopeFactory));
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
