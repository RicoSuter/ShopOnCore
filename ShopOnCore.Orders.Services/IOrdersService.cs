using System.Collections.Generic;
using System.Threading.Tasks;
using ShopOnCore.Orders.Contract;

namespace ShopOnCore.Orders.Services
{
    public interface IOrdersService
    {
        Task SaveAsync(CreateOrderMessage order);

        Task<IEnumerable<CreateOrderMessage>> GetAllAsync();
    }
}