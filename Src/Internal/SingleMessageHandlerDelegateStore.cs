using System;
using System.Collections.Generic;

namespace Xer.Delegator
{
    /// <summary>
    /// Represents an object that stores a single <see cref="Xer.Delegator.MessageHandlerDelegate{TMessage}"/> that is mapped to a message type.
    /// </summary>
    internal class SingleMessageHandlerDelegateStore
    {
        #region Declarations
            
        public readonly Dictionary<Type, Delegate> _messageHandlersByMessageType = new Dictionary<Type, Delegate>();

        #endregion Declarations

        #region Methods
        
        /// <summary>
        /// Add message handler delegate for the specified message type.
        /// Duplicate message handlers for a single message type is not allowed.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <param name="messageHandlerDelegate">Message handler delegate.</param>
        public void Add<TMessage>(MessageHandlerDelegate<TMessage> messageHandlerDelegate) where TMessage : class
        {
            if (messageHandlerDelegate == null)
            {   
                throw new ArgumentNullException(nameof(messageHandlerDelegate));
            }

            Type messageType = typeof(TMessage);

            // Check if a handler already exists.
            if (TryGetValue<TMessage>(out MessageHandlerDelegate<TMessage> existingMessageHandler))
            {
                throw new InvalidOperationException($"Duplicate message handler registered for {messageType.Name}.");
            }

            _messageHandlersByMessageType.Add(messageType, messageHandlerDelegate);
        }

        /// <summary>
        /// Try to retrieve a single message handler delegate for the specified message type. 
        /// If no message handler delegate is found, an instance of <see cref="Xer.Delegator.NullMessageHandlerDelegate{TMessage}"/> is returned.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <param name="messageHandlerDelegates">Message handler delegate.</param>
        /// <returns>True if a message handler delegate is found. Otherwise, false.</returns>
        public bool TryGetValue<TMessage>(out MessageHandlerDelegate<TMessage> messageHandlerDelegate) where TMessage : class
        {
            if (_messageHandlersByMessageType.TryGetValue(typeof(TMessage), out Delegate storedMessageHandler))
            {
                messageHandlerDelegate = (MessageHandlerDelegate<TMessage>)storedMessageHandler;
                return true;
            }

            messageHandlerDelegate = NullMessageHandlerDelegate<TMessage>.Value;
            return false;
        }

        #endregion Methods
    }
}