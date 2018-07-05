using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShopOnCore.Orders.Contract
{
    public class CreateOrderMessage
    {
        private static string FileName { get; } = "abc.txt";

        public string Product { get; set; }

        public int Amount { get; set; }

        public Task StoreAsync()
        {
            lock (typeof(CreateOrderMessage))
            {
                if (!File.Exists(FileName))
                    File.Create(FileName);

                File.AppendAllText(FileName, Product + "\n" + Amount + "\n");
                return Task.CompletedTask;
            }
        }

        public static Task<IEnumerable<CreateOrderMessage>> LoadAllAsync()
        {
            lock (typeof(CreateOrderMessage))
            {
                if (File.Exists(FileName))
                {
                    var lines = File.ReadAllLines(FileName);
                    var index = 0;

                    return Task.FromResult(lines
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .GroupBy(s => (index++) / 2)
                        .Select(g => new CreateOrderMessage
                        {
                            Product = g.First(),
                            Amount = int.Parse(g.Last())
                        }));
                }

                return Task.FromResult<IEnumerable<CreateOrderMessage>>(new List<CreateOrderMessage>());
            }
        }
    }
}