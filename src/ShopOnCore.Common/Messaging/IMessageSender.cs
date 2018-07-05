using System.Threading.Tasks;

namespace ShopOnCore.Common.Messaging
{
    public interface IMessageSender<in TMessage>
    {
        Task SendMessageAsync(TMessage message);
    }
}
