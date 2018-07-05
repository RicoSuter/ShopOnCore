using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopOnCore.Orders.Contract;

namespace ShopOnCore.Orders.Services
{
    public class InMemoryOrdersService : IOrdersService
    {
        private static object _lock = new object();
        private static List<CreateOrderMessage> _list = new List<CreateOrderMessage>();

        public Task SaveAsync(CreateOrderMessage order)
        {
            lock (_lock)
            {
                if (_list.All(o => o.Id != order.Id))
                {
                    _list.Add(order);
                }
            }

            return Task.CompletedTask;
        }

        public Task<IEnumerable<CreateOrderMessage>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<CreateOrderMessage>>(_list.ToArray());
        }
    }
}