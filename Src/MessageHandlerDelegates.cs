using System.Threading;
using System.Threading.Tasks;

namespace Xer.Delegator
{
    /// <summary>
    /// Delegate to handle messages.
    /// </summary>
    /// <typeparam name="TMessage">The type of message.</typeparam>
    /// <param name="message">Message to handle.</param>
    /// <param name="cancellationToken">Optional cancellation token to support cancellation.<param>
    /// <returns>Asynchronous task which can be awaited for completion.</returns>
    public delegate Task MessageHandlerDelegate<TMessage>(TMessage message, CancellationToken cancellationToken = default(CancellationToken)) where TMessage : class;

    /// <summary>
    /// Null message handler delegate.
    /// </summary>
    public static class NullMessageHandlerDelegate<TMessage> where TMessage : class
    {
        /// <summary>
        /// Message handler delegate instance that does nothing.
        /// </summary>
        public static readonly MessageHandlerDelegate<TMessage> Value = (m, ct) => TaskUtility.CompletedTask;
    }
}