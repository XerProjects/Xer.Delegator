using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xer.Delegator
{
    /// <summary>
    /// Represents an object which stores a collection of <see cref="Xer.Delegator.MessageHandlerDelegate{TMessage}"/>  that are mapped to a message type.
    /// </summary>
    internal class MultiMessageHandlerDelegateStore
    {
        #region Declarations
            
        // IList value is a List<MessageHandlerDelegate<TMessage> where TMessage matches the Type in the dictionary key.
        private readonly Dictionary<Type, IList> _messageHandlersByMessageType = new Dictionary<Type, IList>();

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
                _messageHandlersByMessageType.Add(messageType, new List<MessageHandlerDelegate<TMessage>>(1) { messageHandlerDelegate });
            }
        }

        /// <summary>
        /// Try to retrieve a message handler delegate for the specified message type.
        /// The message handler delegate that this method returns is actually a delegate that invokes
        /// a collection of internal message handler delegates that are registered/added for the specified message type.
        /// If no message handler delegate is found, an instance of <see cref="Xer.Delegator.NullMessageHandlerDelegate{TMessage}"/> is returned.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <param name="messageHandlerDelegates">Message handler delegates.</param>
        /// <returns>
        /// True if a message handler delegate is found, which means that there is atleast 
        /// one message handler delegate that is registered/added for the specified message type. Otherwise, false.
        /// </returns>
        public bool TryGetValue<TMessage>(out MessageHandlerDelegate<TMessage> messageHandlerDelegate) where TMessage : class
        {
            if (_messageHandlersByMessageType.TryGetValue(typeof(TMessage), out IList storedMessageHandlers))
            {
                // Build message handler delegate.
                // Cast object. This should throw if casting fails, but we are sure that IList is a List<MessageHandlerDelegate<TMessage>>.
                messageHandlerDelegate = BuildMessageHandlerDelegate((List<MessageHandlerDelegate<TMessage>>)storedMessageHandlers);
                return true;
            }

            messageHandlerDelegate = NullMessageHandlerDelegate<TMessage>.Value;
            return false;
        }

        #region Functions
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageHandlerDelegates"></param>
        /// <returns></returns>
        private static MessageHandlerDelegate<TMessage> BuildMessageHandlerDelegate<TMessage>(List<MessageHandlerDelegate<TMessage>> messageHandlerDelegates) where TMessage : class
        {
            // Return a delegate that invokes all registered message handlers.
            return (message, cancellationToken) =>
            {
                // Task list.
                var handleTasks = new Task[messageHandlerDelegates.Count];

                // Invoke each message handler delegates to start the tasks and add to task list.
                for (int i = 0; i < messageHandlerDelegates.Count; i++)
                    handleTasks[i] = messageHandlerDelegates[i].Invoke(message, cancellationToken);

                // Wait for all tasks to complete.
                return Task.WhenAll(handleTasks);
            };
        }

        #endregion Functions

        #endregion Methods
    }
}