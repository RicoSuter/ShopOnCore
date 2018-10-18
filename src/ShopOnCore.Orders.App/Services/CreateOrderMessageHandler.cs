using System.Threading;
using System.Threading.Tasks;
using ShopOnCore.Common.Messaging.Interfaces;
using ShopOnCore.Orders.Contract;
using ShopOnCore.Orders.Services;

namespace ShopOnCore.Orders.App.Services
{
    public class CreateOrderMessageHandler : IMessageHandler<CreateOrderMessage>
    {
        private readonly IOrdersService _ordersService;

        public CreateOrderMessageHandler(IOrdersService ordersService)
        {
            // This handler is created by the message receiver which is registered in Program.cs
            _ordersService = ordersService;
        }

        public async Task HandleAsync(CreateOrderMessage message, CancellationToken cancellationToken)
        {
            await Task.Delay(1000);

            await _ordersService.SaveAsync(message);
        }
    }
}