using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private readonly object _lock = new object(); 

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

            lock (_lock)
            {
                // Check if a container already exists. 
                if (_messageHandlersByMessageType.TryGetValue(messageType, out MessageHandlerContainer container))
                {
                    // Replace message handler container with new instance that contains the updated handler list.
                    _messageHandlersByMessageType[messageType] = container.AppendNew(handler);
                }
                else
                {
                    // Instantiate new container and add to dictionary.
                    _messageHandlersByMessageType.Add(messageType, new MessageHandlerContainer(messageType, new[] { handler }));
                }
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
                messageHandlerDelegate = container.CombinedMessageHandler;
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

        #region Inner Struct
        
        private struct MessageHandlerContainer
        {
            #region Declarations

            private MessageHandlerDelegate[] _messageHandlerDelegates;

            #endregion Declarations

            #region Properties

            /// <summary>
            /// Type of message that the handlers are handling.
            /// </summary>
            public Type MessageType { get; }

            /// <summary>
            /// Provides a message handler delegate that invokes all the delegates in the message handler list.
            /// </summary>
            public MessageHandlerDelegate CombinedMessageHandler { get; }
            
            /// <summary>
            /// List of message handlers for the message type defined in the message type property.
            /// </summary>
            public IReadOnlyCollection<MessageHandlerDelegate> MessageHandlers => new ReadOnlyCollection<MessageHandlerDelegate>(_messageHandlerDelegates);

            #endregion Properties

            #region Constructors

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="messageType">Type of message.</param>
            /// <param name="messageHandlerDelegates">Message handler delegates.</param>
            public MessageHandlerContainer(Type messageType, MessageHandlerDelegate[] messageHandlerDelegates)
            {
                _messageHandlerDelegates = messageHandlerDelegates;
                CombinedMessageHandler = CombineMessageHandlers(_messageHandlerDelegates);
                MessageType = messageType;
            }

            #endregion Constructors

            #region Methods

            /// <summary>
            /// Add a message handler to the current list of <see cref="Xer.Delegator.MessageHandlerDelegate"/>
            /// and return it in a new instance of <see cref="Xer.Delegator.MultiMessageHandlerDelegateStore.MessageHandlerContainer"/>.
            /// </summary>
            /// <param name="messageHandlerDelegate">Message handler delegate.</param>
            public MessageHandlerContainer AppendNew(MessageHandlerDelegate messageHandlerDelegate)
            {
                int updatedLength = _messageHandlerDelegates.Length + 1;

                MessageHandlerDelegate[] updated = new MessageHandlerDelegate[updatedLength];
                
                // Copy and add new message handler delegate.
                _messageHandlerDelegates.CopyTo(updated, 0);
                
                // Add new message handler delegate.
                updated[updatedLength - 1] = messageHandlerDelegate;

                return new MessageHandlerContainer(MessageType, updated);
            }

            /// <summary>
            /// Combine all message handler delegates into a single delegate and assign to CombinedMessageHandler property.
            /// </summary>
            private static MessageHandlerDelegate CombineMessageHandlers(MessageHandlerDelegate[] messageHandlerDelegates)
            {
                if (messageHandlerDelegates.Length > 0)
                {
                    // Create a delegate that invokes all registered message handlers.
                    return (message, cancellationToken) =>
                    {
                        // Task list.
                        Task[] handleTasks = new Task[messageHandlerDelegates.Length];

                        // Invoke each message handler delegates to start the tasks and add to task list.
                        for (int i = 0; i < messageHandlerDelegates.Length; i++)
                            handleTasks[i] = messageHandlerDelegates[i].Invoke(message, cancellationToken);

                        // Wait for all tasks to complete.
                        return Task.WhenAll(handleTasks);
                    };
                }

                return NullMessageHandlerDelegate.Instance;
            }

            #endregion Methods
        }

        #endregion Inner Struct
    }
}