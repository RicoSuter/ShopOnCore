﻿using System.Threading;
using System.Threading.Tasks;
using ShopOnCore.Common.Messaging;
using ShopOnCore.Orders.Contract;
using ShopOnCore.Orders.Services;

namespace ShopOnCore.Orders.App.Services
{
    public class CreateOrderMessageHandler : IMessageHandler<CreateOrderMessage>
    {
        private readonly IOrdersService _ordersService;

        public CreateOrderMessageHandler(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        public async Task HandleAsync(CreateOrderMessage message, CancellationToken cancellationToken)
        {
            await Task.Delay(1000);

            await _ordersService.SaveAsync(message);
        }
    }
}