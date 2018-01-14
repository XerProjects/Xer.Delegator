using System;
using Xer.Delegator.Exceptions;
using Xer.Delegator.Registrations;

namespace Xer.Delegator.Resolvers
{
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
        /// If no handler is found, a <see cref="Xer.Delegator.NullMessageHandlerDelegate{TMessage}.Value"/> is returned.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <returns>Message handler delegate.</returns>
        public MessageHandlerDelegate<TMessage> ResolveMessageHandler<TMessage>() where TMessage : class
        {
            if (_singleMessageHandlerDelegateStore.TryGetValue<TMessage>(out MessageHandlerDelegate<TMessage> messageHandler))
            {
                return messageHandler;
            }
            
            return NullMessageHandlerDelegate<TMessage>.Value;
        }

        #endregion IMessageHandlerResolver Implementation
    }
}