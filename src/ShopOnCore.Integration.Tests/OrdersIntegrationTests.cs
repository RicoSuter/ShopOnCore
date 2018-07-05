using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ShopOnCore.Common.Messaging;
using ShopOnCore.Orders.App.Services;
using ShopOnCore.Orders.Contract;
using Xunit;

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
            serviceCollection.AddInProcessMessageHandler<CreateOrderMessageHandler, CreateOrderMessage>();
        }

        [Fact]
        public async Task Orders_CreateOrder_OrderIsStored()
        {
            // Arrange
            var guid = Guid.NewGuid().ToString().Replace("-", string.Empty);

            // Act
            await HttpClient.PostAsync("api/orders?product=" + guid + "&amount=3", new StringContent(string.Empty));

            // Assert
            await AwaitAsync(async () =>
            {
                var response = await HttpClient.GetAsync("api/orders");
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<CreateOrderMessage>>(json);
            }, n => n.Any(x => x.Product == guid), TimeSpan.FromSeconds(30));
        }
    }
}
