using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xer.Delegator.Resolvers;

namespace Xer.Delegator.Registration
{
    /// <summary>
    /// Represents an object which allows multiple message handler delegates to be registered for a given message type.
    /// </summary>
    public class MultiMessageHandlerRegistration : IMessageHandlerRegistration
    {
        #region Declarations

        private readonly MultiMessageHandlerDelegateStore _messageHandlersByMessageType = new MultiMessageHandlerDelegateStore();

        #endregion Declarations

        #region IMessageHandlerRegistration Implementation

        /// <summary>
        /// Register an asynchronous message handler delegate for the specified message type. 
        /// This will add the message handler delegate to an internal collection of delegates.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <param name="messageHandler">Asynchronous message handler delegate.</param>
        public void Register<TMessage>(Func<TMessage, CancellationToken, Task> messageHandler) where TMessage : class
        {
            if (messageHandler == null)
            {
                throw new ArgumentNullException(nameof(messageHandler));
            }

            _messageHandlersByMessageType.Add<TMessage>(messageHandler);
        }

        #endregion IMessageHandlerRegistration Implementation

        #region Methods
        
        /// <summary>
        /// Build a message handler resolver containing all registered message handler delegates.
        /// </summary>
        /// <returns>
        /// Message handler resolver that returns a message handler delegate 
        /// which invokes all stored delegates in the internal collection of delegates.
        /// </returns>
        public IMessageHandlerResolver BuildMessageHandlerResolver()
        {
            return new MultiMessageHandlerResolver(_messageHandlersByMessageType);
        }

        #endregion Methods
    }
}