using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShopOnCore.Common.Messaging;
using ShopOnCore.Orders.Contract;

namespace ShopOnCore.Web.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        private readonly IMessageSender<CreateOrderMessage> _messageSender;

        public OrdersController(IMessageSender<CreateOrderMessage> messageSender)
        {
            _messageSender = messageSender;
        }

        [HttpPost]
        public async Task CreateOrder(string product, int amount)
        {
            var message = new CreateOrderMessage
            {
                Product = product,
                Amount = amount
            };

            await _messageSender.SendMessageAsync(message);
        }

        [HttpGet]
        public async Task<IEnumerable<CreateOrderMessage>> GetOrders()
        {
            return await CreateOrderMessage.LoadAllAsync();
        }
    }
}
