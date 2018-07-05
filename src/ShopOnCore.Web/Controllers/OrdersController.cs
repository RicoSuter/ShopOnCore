using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShopOnCore.Common.Messaging;
using ShopOnCore.Orders.Contract;
using ShopOnCore.Orders.Services;

namespace ShopOnCore.Web.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        private readonly IMessageSender<CreateOrderMessage> _messageSender;
        private readonly IOrdersService _ordersService;

        public OrdersController(IMessageSender<CreateOrderMessage> messageSender, IOrdersService ordersService)
        {
            _messageSender = messageSender;
            _ordersService = ordersService;
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
            return await _ordersService.GetAllAsync();
        }
    }
}
