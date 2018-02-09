using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.Delegator
{
    public static class MessageDelegatorExtensions
    {       
        /// <summary>
        /// Send all messages to a delegate with one/many handlers.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <param name="messageDelegator">Message delegator.</param>
        /// <param name="messages">Messages to send.</param>
        /// <param name="cancellationToken">Optional cancellation token to be passed to handlers.</param>
        /// <returns>Asynchronous task which can be awaited for completion.</returns>
        public static Task SendAllAsync<TMessage>(this IMessageDelegator messageDelegator, 
                                                  IEnumerable<TMessage> messages, 
                                                  CancellationToken cancellationToken = default(CancellationToken)) 
                                                  where TMessage : class
        {
            if (messageDelegator == null)
            {
                throw new System.ArgumentNullException(nameof(messageDelegator));
            }

            if (messages == null)
            {
                throw new System.ArgumentNullException(nameof(messages));
            }

            // Convert to array.
            TMessage[] messageList = messages.ToArray();

            // Task list.
            Task[] sendTasks = new Task[messageList.Length];

            // Send each messages to start the tasks and add to task list.
            for (int i = 0; i < messageList.Length; i++)
                sendTasks[i] = messageDelegator.SendAsync(messageList[i], cancellationToken);

            // Wait for all messages to be sent.
            return Task.WhenAll(sendTasks);
        }
    }
}