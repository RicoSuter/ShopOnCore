using System.Threading;
using System.Threading.Tasks;

namespace ShopOnCore.Common.Messaging
{
    public interface IMessageHandler<in TMessage>
    {
        Task HandleAsync(TMessage message, CancellationToken cancellationToken);
    }
}