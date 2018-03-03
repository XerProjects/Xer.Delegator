using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.Delegator
{
    /// <summary>
    /// Represents an object that handles registration of message handler delegates for given message types.
    /// </summary>
    public interface IMessageHandlerRegistration
    {
        /// <summary>
        /// Register an asynchronous message handler delegate for the message type.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <param name="messageHandler">Asynchronous message handler delegate.</param>
        void Register<TMessage>(Func<TMessage, CancellationToken, Task> messageHandler) where TMessage : class;
    }
}