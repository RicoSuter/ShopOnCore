using System.Threading;
using System.Threading.Tasks;
using ShopOnCore.Common.Messaging;
using ShopOnCore.Orders.Contract;

namespace ShopOnCore.Orders.App.Services
{
    public class CreateOrderMessageHandler : IMessageHandler<CreateOrderMessage>
    {
        public async Task HandleAsync(CreateOrderMessage message, CancellationToken cancellationToken)
        {
            await Task.Delay(1000);

            await message.StoreAsync();
        }
    }
}