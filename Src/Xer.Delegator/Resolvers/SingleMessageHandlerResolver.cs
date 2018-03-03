using System;

namespace Xer.Delegator.Resolvers
{
    /// <summary>
    /// Represents an object that resolves a single instance of <see cref="Xer.Delegator.MessageHandlerDelegate"/>.
    /// </summary>
    public class SingleMessageHandlerResolver : IMessageHandlerResolver
    {
        #region Declarations
            
        private readonly SingleMessageHandlerDelegateStore _singleMessageHandlerDelegateStore = new SingleMessageHandlerDelegateStore();

        #endregion Declarations

        #region Constructors
        
        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="singleMessageHandlerDelegateStore">Single message handler delegate store.</param>
        internal SingleMessageHandlerResolver(SingleMessageHandlerDelegateStore singleMessageHandlerDelegateStore)
        {
            _singleMessageHandlerDelegateStore = singleMessageHandlerDelegateStore ?? throw new ArgumentNullException(nameof(singleMessageHandlerDelegateStore));
        }

        #endregion Constructors
        
        #region IMessageHandlerResolver Implementation

        /// <summary>
        /// Resolve a message handler delegate for the message type.
        /// If no handler is found, an instance of <see cref="Xer.Delegator.NullMessageHandlerDelegate.Instance"/> is returned.
        /// </summary>
        /// <param name="messageType">Type of message.</param>
        /// <returns>Message handler delegate.</returns>
        public MessageHandlerDelegate ResolveMessageHandler(Type messageType)
        {
            if (_singleMessageHandlerDelegateStore.TryGetMessageHandlerDelegate(messageType, out MessageHandlerDelegate messageHandlerDelegate))
            {
                return messageHandlerDelegate;
            }
            
            // Null message handler.
            return NullMessageHandlerDelegate.Instance;
        }

        #endregion IMessageHandlerResolver Implementation
    }
}