using System.Threading;
using System.Threading.Tasks;

namespace ShopOnCore.Common.Messaging.Interfaces
{
    public interface IMessageHandler<in TMessage>
    {
        Task HandleAsync(TMessage message, CancellationToken cancellationToken);
    }
}