using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.Delegator
{
    /// <summary>
    /// Represents an object which stores a collection of <see cref="Xer.Delegator.MessageHandlerDelegate"/>  that are mapped to a message type.
    /// </summary>
    internal class MultiMessageHandlerDelegateStore
    {
        #region Declarations

        // This dictionary contains all the message handler delegates that are registered for a message type.
        private readonly Dictionary<Type, MessageHandlerContainer> _messageHandlersByMessageType = new Dictionary<Type, MessageHandlerContainer>();
        
        #endregion Declarations

        #region Methods

        /// <summary>
        /// Add message handler delegate for the specified message type. 
        /// This will add the message handler delegate to an internal collection of delegates.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <param name="messageHandler">Asynchronous message handler delegate.</param>
        public void Add<TMessage>(Func<TMessage, CancellationToken, Task> messageHandlerDelegate) where TMessage : class
        {
            if (messageHandlerDelegate == null)
            {
                throw new ArgumentNullException(nameof(messageHandlerDelegate));
            }

            // Build message handler.
            MessageHandlerDelegate handler = BuildMessageHandlerDelegate(messageHandlerDelegate);

            Type messageType = typeof(TMessage);

            // Check if a container already exists. 
            if (_messageHandlersByMessageType.TryGetValue(messageType, out MessageHandlerContainer container))
            {
                // Add message handler to existing container list.
                container.AddMessageHandler(handler);
            }
            else
            {
                // Instantiate new container and add to dictionary.
                _messageHandlersByMessageType.Add(messageType, new MessageHandlerContainer(messageType).AddMessageHandler(handler));
            }
        }

        /// <summary>
        /// Try to retrieve a message handler delegate for the specified message type.
        /// The message handler delegate that this method returns is actually a delegate that invokes
        /// a collection of internal message handler delegates that are registered/added for the specified message type.
        /// If no message handler delegate is found, an instance of <see cref="Xer.Delegator.NullMessageHandlerDelegate.Instance"/> is returned.
        /// </summary>
        /// <param name="messageType">Type of message.</param>
        /// <param name="messageHandlerDelegate">Message handler delegate.</param>
        /// <returns>
        /// True if a message handler delegate is found, which means that there is atleast 
        /// one message handler delegate that is registered/added for the specified message type. Otherwise, false.
        /// </returns>
        public bool TryGetMessageHandlerDelegate(Type messageType, out MessageHandlerDelegate messageHandlerDelegate)
        {
            if (_messageHandlersByMessageType.TryGetValue(messageType, out MessageHandlerContainer container))
            {
                messageHandlerDelegate = container.MergedMessageHandler;
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

        #region Inner Class
        
        private class MessageHandlerContainer
        {
            #region Declarations

            private MessageHandlerDelegate _mergedMessageHandler = NullMessageHandlerDelegate.Instance;
            private List<MessageHandlerDelegate> _messageHandlerDelegates = new List<MessageHandlerDelegate>();

            #endregion Declarations

            #region Properties

            /// <summary>
            /// Type of message that the handlers are handling.
            /// </summary>
            public Type MessageType { get; }

            /// <summary>
            /// List of message handlers for the message type defined in the message type property.
            /// </summary>
            public IReadOnlyList<MessageHandlerDelegate> MessageHandlers => _messageHandlerDelegates;

            /// <summary>
            /// Provides a merged message handler delegate that invokes all the delegates in the message handler list.
            /// </summary>
            public MessageHandlerDelegate MergedMessageHandler => _mergedMessageHandler;

            #endregion Properties

            #region Constructors

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="messageType">Type of message.</param>
            public MessageHandlerContainer(Type messageType)
            {
                MessageType = messageType;
            }

            #endregion Constructors

            #region Methods

            /// <summary>
            /// Add a message handler to the list and rebuilds the MessageHandlerDelegate property 
            /// to contain the newly added message handler delegate.
            /// </summary>
            /// <param name="messageHandlerDelegate">Message handler delegate.</param>
            public MessageHandlerContainer AddMessageHandler(MessageHandlerDelegate messageHandlerDelegate)
            {
                _messageHandlerDelegates.Add(messageHandlerDelegate);

                // Re-merge to include newly added message handler.
                BuildMergedMessageHandler();

                return this;
            }

            /// <summary>
            /// Merge all message handler delegates into a single delegate and assign to MergedMessageHandler property.
            /// </summary>
            private void BuildMergedMessageHandler()
            {
                if(_messageHandlerDelegates.Count > 0)
                {
                    // Create a delegate that invokes all registered message handlers.
                    _mergedMessageHandler = (message, cancellationToken) =>
                    {
                        // Task list.
                        Task[] handleTasks = new Task[_messageHandlerDelegates.Count];

                        // Invoke each message handler delegates to start the tasks and add to task list.
                        for (int i = 0; i < _messageHandlerDelegates.Count; i++)
                            handleTasks[i] = _messageHandlerDelegates[i].Invoke(message, cancellationToken);

                        // Wait for all tasks to complete.
                        return Task.WhenAll(handleTasks);
                    };
                }
            }

            #endregion Methods
        }

        #endregion Inner Class
    }
}