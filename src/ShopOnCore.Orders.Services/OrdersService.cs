using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShopOnCore.Orders.Contract;
using StackExchange.Redis;

namespace ShopOnCore.Orders.Services
{
    public class OrdersService : IOrdersService
    {
        public async Task SaveAsync(CreateOrderMessage order)
        {
            using (var redis = await ConnectAsync())
            {
                var database = redis.GetDatabase();
                await database.StringSetAsync(order.Id, JsonConvert.SerializeObject(order));
            }
        }

        public async Task<IEnumerable<CreateOrderMessage>> GetAllAsync()
        {
            using (var redis = await ConnectAsync())
            {
                var keys = redis.GetServer("redis_db", 6379).Keys();
                var database = redis.GetDatabase();

                var list = new List<CreateOrderMessage>();
                foreach (var key in keys)
                {
                    list.Add(JsonConvert.DeserializeObject<CreateOrderMessage>(await database.StringGetAsync(key)));
                }

                return list;
            }
        }

        private async Task<ConnectionMultiplexer> ConnectAsync()
        {
            return await ConnectionMultiplexer.ConnectAsync("redis_db:6379");
        }
    }
}
