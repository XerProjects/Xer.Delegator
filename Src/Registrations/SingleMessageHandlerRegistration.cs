using System;
using System.Reflection;
using Xer.Delegator.Resolvers;

namespace Xer.Delegator.Registrations
{
    /// <summary>
    /// Represents an object which only allows a single message handler delegate to be registered for a given message type.
    /// </summary>
    public class SingleMessageHandlerRegistration : IMessageHandlerRegistration
    {
        #region Declarations

        private readonly SingleMessageHandlerDelegateStore _messageHandlersByMessageType = new SingleMessageHandlerDelegateStore();

        #endregion Declarations

        #region IMessageHandlerRegistration Implementation
        
        /// <summary>
        /// Register an asynchronous message handler delegate for the specified message type.
        /// Duplicate message handlers for a single message type is not allowed.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <param name="messageHandlerDelegate">Asynchronous message handler delegate.</param>
        public void Register<TMessage>(MessageHandlerDelegate<TMessage> messageHandler) where TMessage : class
        {
            if (messageHandler == null)
            {
                throw new ArgumentNullException(nameof(messageHandler));
            }

            _messageHandlersByMessageType.Add<TMessage>(messageHandler);
        }

        /// <summary>
        /// Register a synchronous message handler delegate for the specified message type.
        /// Duplicate message handlers for a single message type is not allowed.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <param name="messageHandlerDelegate">Synchronous message handler delegate.</param>
        public void Register<TMessage>(Action<TMessage> messageHandler) where TMessage : class
        {
            if (messageHandler == null)
            {
                throw new ArgumentNullException(nameof(messageHandler));
            }
            
            // Convert to async delegate.
            Register<TMessage>((message, ct) =>
            {
                messageHandler.Invoke(message);
                return TaskUtility.CompletedTask;
            });
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
            return new SingleMessageHandlerResolver(_messageHandlersByMessageType);
        }

        #endregion Methods
    }
}