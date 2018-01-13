using System.Threading;
using System.Threading.Tasks;

namespace Xer.Delegator
{
    public interface IMessageDispatcher
    {
        /// <summary>
        /// Dispatch message to handler.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <param name="message">Message to dispatch.</param>
        /// <param name="cancellationToken">Optional cancellation token to be passed to handlers.</param>
        /// <returns>Asynchronous task which can be awaited for completion.</returns>
        Task DispatchAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default(CancellationToken)) where TMessage : class;
    }
}