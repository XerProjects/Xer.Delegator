using System.Threading;
using System.Threading.Tasks;

namespace Xer.Delegator
{
    public interface IMessageDelegator
    {
        /// <summary>
        /// Send message to a delegate with one/many handlers.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Optional cancellation token to be passed to handlers.</param>
        /// <returns>Asynchronous task which can be awaited for completion.</returns>
        Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default(CancellationToken)) where TMessage : class;
    }
}