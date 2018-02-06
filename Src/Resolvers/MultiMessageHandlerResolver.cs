using System;

namespace Xer.Delegator.Resolvers
{
    /// <summary>
    /// Represents an object that resolves multiple instances of message handler delegates 
    /// which are merged in a single <see cref="Xer.Delegator.MessageHandlerDelegate"/> instance.
    /// </summary>
    public class MultiMessageHandlerResolver : IMessageHandlerResolver
    {
        #region Declarations
            
        private readonly MultiMessageHandlerDelegateStore _multiMessageHandlerDelegateStore = new MultiMessageHandlerDelegateStore();

        #endregion Declarations

        #region Constructors
        
        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="multiMessageHandlerDelegateStore">Multi message handler delegate store.</param>
        internal MultiMessageHandlerResolver(MultiMessageHandlerDelegateStore multiMessageHandlerDelegateStore)
        {
            _multiMessageHandlerDelegateStore = multiMessageHandlerDelegateStore ?? throw new ArgumentNullException(nameof(multiMessageHandlerDelegateStore));
        }

        #endregion Constructors

        #region IMessageHandlerResolver Implementation

        /// <summary>
        /// Resolve a message handler delegate for the message type.
        /// If no handler is found, an instance of <see cref="Xer.Delegator.NullMessageHandlerDelegate.Instance" /> is returned.
        /// </summary>
        /// <param name="messageType">Type of message.</param>
        /// <returns>Message handler delegate which invokes all registered delegates.</returns>
        public MessageHandlerDelegate ResolveMessageHandler(Type messageType)
        {
            if (_multiMessageHandlerDelegateStore.TryGetMessageHandlerDelegate(messageType, out MessageHandlerDelegate messageHandlerDelegate))
            {
                return messageHandlerDelegate;
            }

            // Null message handler.
            return NullMessageHandlerDelegate.Instance;
        }

        #endregion IMessageHandlerResolver Implementation
    }
}