using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShopOnCore.Common.Messaging
{
    public class InProcessMessageSender<TMessage, TMessageHandler> : IMessageSender<TMessage>
        where TMessageHandler : IMessageHandler<TMessage>
    {
        private readonly TMessageHandler _handler;
        private readonly bool _blockUntilProcessed;

        public InProcessMessageSender(TMessageHandler handler, bool blockUntilProcessed = false)
        {
            _handler = handler;
            _blockUntilProcessed = blockUntilProcessed;
        }

        public async Task SendMessageAsync(TMessage message)
        {
            if (_blockUntilProcessed)
            {
                await _handler.HandleAsync(message, CancellationToken.None);
            }
            else
            {
                #pragma warning disable CS4014
                Task.Run(async () =>
                {
                    try
                    {
                        await _handler.HandleAsync(message, CancellationToken.None);
                    }
                    catch (Exception exception)
                    {
                        // TODO: Handle e
                        Console.WriteLine("In-process message processing failed: " + exception);
                    }
                });
            }
        }
    }
}