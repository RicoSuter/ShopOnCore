using System;

namespace ShopOnCore.Orders.Contract
{
    public class CreateOrderMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Product { get; set; }

        public int Amount { get; set; }
    }
}