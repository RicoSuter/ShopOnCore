using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using ShopOnCore.Orders.App.Services;
using ShopOnCore.Orders.Contract;
using ShopOnCore.Orders.Services;
using Xunit;
using ShopOnCore.Common.Messaging.Extensions;

namespace ShopOnCore.Integration.Tests
{
    public abstract class OrdersIntegrationTests : IntegrationTestsBase
    {
        [Trait("Category", "E2E")]
        public class RemoteOrdersIntegrationTests : OrdersIntegrationTests
        {
            public RemoteOrdersIntegrationTests()
            {
                InitializeRemoteServer();
            }
        }

        [Trait("Category", "Integration")]
        public class LocalOrdersIntegrationTests : OrdersIntegrationTests
        {
            public LocalOrdersIntegrationTests()
            {
                InitializeTestServer();
            }
        }

        protected override void ConfigureTestServerServices(IServiceCollection serviceCollection)
        {
            // For local testing, we use a in-memory based orders service
            serviceCollection.TryAddScoped<IOrdersService, InMemoryOrdersService>();

            // Now we register a message sender which directly processes the message in-process 
            // for that, we need a reference to the message handler class (which would run in a separate process)
            serviceCollection.AddBlockingInProcessMessageSender<CreateOrderMessage, CreateOrderMessageHandler>();
        }

        [Fact]
        public async Task Orders_CreateOrder_OrderIsStored()
        {
            // Arrange
            var guid = Guid.NewGuid().ToString().Replace("-", string.Empty);

            // Act
            await HttpClient.PostAsync("api/orders?product=" + guid + "&amount=3", new StringContent(string.Empty));

            // Assert
            // Here we need to await until the message has been processed
            // when testing in-process with a blocking sender, we don't need this
            var order = await AwaitNotNullAsync(async () =>
            {
                var response = await HttpClient.GetAsync("api/orders");
                var json = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<IEnumerable<CreateOrderMessage>>(json);
                return orders.FirstOrDefault(o => o.Product == guid);
            }, TimeSpan.FromSeconds(30));

            Assert.Equal(guid, order.Product);
            Assert.Equal(3, order.Amount);
        }
    }
}
