using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xer.Delegator.Resolvers;

namespace Xer.Delegator.Registration
{
    /// <summary>
    /// Represents an object which only allows a single message handler delegate to be registered for a given message type.
    /// </summary>
    public class SingleMessageHandlerRegistration : IMessageHandlerRegistration, IMessageHandlerResolverBuilder
    {
        #region Declarations

        private readonly SingleMessageHandlerDelegateStore _singleMessageHandlerStore = new SingleMessageHandlerDelegateStore();

        #endregion Declarations

        #region IMessageHandlerRegistration Implementation
        
        /// <summary>
        /// Register an asynchronous message handler delegate for the specified message type.
        /// Duplicate message handlers for a single message type is not allowed.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <param name="messageHandler">Asynchronous message handler delegate.</param>
        public void Register<TMessage>(Func<TMessage, CancellationToken, Task> messageHandler) where TMessage : class
        {
            if (messageHandler == null)
            {
                throw new ArgumentNullException(nameof(messageHandler));
            }

            _singleMessageHandlerStore.Add<TMessage>(messageHandler);
        }

        #endregion IMessageHandlerRegistration Implementation

        #region Methods
        
        /// <summary>
        /// Build a message handler resolver containing all registered message handler delegates.
        /// </summary>
        /// <returns>
        /// Message handler resolver that returns a message handler delegate registered for a message type.
        /// </returns>
        public IMessageHandlerResolver BuildMessageHandlerResolver()
        {
            return new SingleMessageHandlerResolver(_singleMessageHandlerStore);
        }

        #endregion Methods
    }
}