using System.Threading;
using System.Threading.Tasks;

namespace Xer.Delegator
{
    /// <summary>
    /// Delegate to handle messages.
    /// </summary>
    /// <param name="message">Message to handle.</param>
    /// <param name="cancellationToken">Optional cancellation token to support cancellation.<param>
    /// <returns>Asynchronous task which can be awaited for completion.</returns>
    public delegate Task MessageHandlerDelegate(object message, CancellationToken cancellationToken = default(CancellationToken));

    /// <summary>
    /// Null message handler delegate.
    /// </summary>
    public static class NullMessageHandlerDelegate
    {
        /// <summary>
        /// Message handler delegate instance that does nothing.
        /// </summary>
        public static readonly MessageHandlerDelegate Instance = (m, ct) => TaskUtility.CompletedTask;
    }
}