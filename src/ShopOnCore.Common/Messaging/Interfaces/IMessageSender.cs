using System.Threading;
using System.Threading.Tasks;

namespace ShopOnCore.Common.Messaging.Interfaces
{
    public interface IMessageSender<in TMessage>
    {
        Task SendMessageAsync(TMessage message, CancellationToken cancellationToken);
    }
}
