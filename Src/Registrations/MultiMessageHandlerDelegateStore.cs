using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xer.Delegator.Registrations
{
    internal class MultiMessageHandlerDelegateStore
    {
        #region Declarations
            
        // IList value is a List<MessageHandlerDelegate<TMessage> where TMessage matches the Type in the dictionary key.
        private readonly IDictionary<Type, IList> _messageHandlersByMessageType = new Dictionary<Type, IList>();

        #endregion Declarations

        #region Methods

        /// <summary>
        /// Add message handler delegate for the specified message type. 
        /// This will add the message handler delegate to an internal collection of delegates.
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
            // Note: IList is a List<MessageHandlerDelegate<TMessage>>.
            if (_messageHandlersByMessageType.TryGetValue(messageType, out IList existingMessageHandlers))
            {
                // Add message handler to existing list.
                existingMessageHandlers.Add(messageHandlerDelegate);
            }
            else
            {
                // Instantiate new list and add to dictionary.
                _messageHandlersByMessageType.Add(messageType, new List<MessageHandlerDelegate<TMessage>>() { messageHandlerDelegate });
            }
        }

        /// <summary>
        /// Try to retrieve a collection of message handler delegates for the specified message type. 
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <param name="messageHandlerDelegates">Message handler delegates.</param>
        /// <returns>True if message handler delegates are found. Otherwise, false.</returns>
        public bool TryGetValue<TMessage>(out IReadOnlyList<MessageHandlerDelegate<TMessage>> messageHandlerDelegates) where TMessage : class
        {
            if (_messageHandlersByMessageType.TryGetValue(typeof(TMessage), out IList storedMessageHandlers))
            {
                // Cast object. This should throw if casting fails, but we are sure that IList is a List<MessageHandlerDelegate<TMessage>>.
                messageHandlerDelegates = (IReadOnlyList<MessageHandlerDelegate<TMessage>>)storedMessageHandlers;
                return true;
            }

            messageHandlerDelegates = MessageHandlerDelagatesCache<TMessage>.Empty;
            return false;
        }

        /// <summary>
        /// Used to cache empty message handler delegate collections.
        /// </summary>
        private static class MessageHandlerDelagatesCache<TMessage> where TMessage : class
        {
            /// <summary>
            /// Cached empty collection
            /// </summary>
            public static readonly IReadOnlyList<MessageHandlerDelegate<TMessage>> Empty = 
                new ReadOnlyCollection<MessageHandlerDelegate<TMessage>>(new MessageHandlerDelegate<TMessage>[0]);
        }

        #endregion Methods
    }
}