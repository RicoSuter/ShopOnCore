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
        private readonly IOrdersService _ordersService;
        private readonly IMessageSender<CreateOrderMessage> _messageSender;

        public OrdersController(IOrdersService ordersService, IMessageSender<CreateOrderMessage> messageSender)
        {
            _ordersService = ordersService;
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

            // Publish the message to the rabbitmq or in-memory queue
            await _messageSender.SendMessageAsync(message);
        }

        [HttpGet]
        public async Task<IEnumerable<CreateOrderMessage>> GetOrders()
        {
            return await _ordersService.GetAllAsync();
        }
    }
}
