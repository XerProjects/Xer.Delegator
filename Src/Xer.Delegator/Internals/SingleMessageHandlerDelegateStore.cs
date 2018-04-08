using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.Delegator
{
    /// <summary>
    /// Represents an object that stores a single <see cref="Xer.Delegator.MessageHandlerDelegate"/> that is mapped to a message type.
    /// </summary>
    internal class SingleMessageHandlerDelegateStore
    {
        #region Declarations
            
        public readonly Dictionary<Type, MessageHandlerDelegate> _messageHandlersByMessageType = new Dictionary<Type, MessageHandlerDelegate>();

        #endregion Declarations

        #region Methods
        
        /// <summary>
        /// Add message handler delegate for the specified message type.
        /// Duplicate message handlers for a single message type is not allowed.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <param name="messageHandlerDelegate">Asynchronous message handler delegate.</param>
        public void Add<TMessage>(Func<TMessage, CancellationToken, Task> messageHandlerDelegate) where TMessage : class
        {
            if (messageHandlerDelegate == null)
            {
                throw new ArgumentNullException(nameof(messageHandlerDelegate));
            }

            Type messageType = typeof(TMessage);

            // Check if a handler already exists.
            if (TryGetMessageHandlerDelegate(messageType, out MessageHandlerDelegate existingMessageHandler))
            {
                throw new InvalidOperationException($"Duplicate message handler registered for {messageType.Name}.");
            }

            _messageHandlersByMessageType.Add(messageType, BuildMessageHandlerDelegate(messageHandlerDelegate));
        }

        /// <summary>
        /// Try to retrieve a single message handler delegate for the specified message type. 
        /// If no message handler delegate is found, an instance of <see cref="Xer.Delegator.NullMessageHandlerDelegate.Instance"/> is returned.
        /// </summary>
        /// <param name="messageType">Type of message.</param>
        /// <param name="messageHandlerDelegate">Message handler delegate.</param>
        /// <returns>True if a message handler delegate is found. Otherwise, false.</returns>
        public bool TryGetMessageHandlerDelegate(Type messageType, out MessageHandlerDelegate messageHandlerDelegate)
        {
            if (_messageHandlersByMessageType.TryGetValue(messageType, out MessageHandlerDelegate storedMessageHandler))
            {
                messageHandlerDelegate = storedMessageHandler;
                return true;
            }

            messageHandlerDelegate = NullMessageHandlerDelegate.Instance;
            return false;
        }

        #endregion Methods

        #region Functions
        
        /// <summary>
        /// Build message handler delegate.
        /// </summary>
        /// <param name="messageHandlerDelegate">Type safe message handler delegate.</param>
        /// <returns>Message handler delegate.</returns>
        private static MessageHandlerDelegate BuildMessageHandlerDelegate<TMessage>(Func<TMessage, CancellationToken, Task> messageHandlerDelegate) 
            where TMessage : class
        {
            return (message, cancellationToken) =>
            {
                if (message == null) throw new ArgumentNullException(nameof(message));

                // Only invoke if object is of correct message type.
                if (message is TMessage typedMessage)
                {
                    return messageHandlerDelegate.Invoke(typedMessage, cancellationToken);
                }

                throw new ArgumentException($"Message argument does not match expected message type: {typeof(TMessage).Name}.", nameof(message));
            };
        }

        #endregion Functions
    }
}