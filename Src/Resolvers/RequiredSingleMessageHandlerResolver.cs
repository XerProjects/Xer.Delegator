using System;
using Xer.Delegator.Internal;

namespace Xer.Delegator.Resolvers
{
    public class RequiredSingleMessageHandlerResolver : IMessageHandlerResolver
    {
        #region Declarations
            
        private readonly SingleMessageHandlerResolver _singleMessageHandlerResolver;

        #endregion Declarations

        #region Constructors

        /// <summary>
        /// Single message handler resolver instance to decorate.
        /// </summary>
        /// <param name="singleMessageHandlerResolver">Single message handler resolver.</param>
        public RequiredSingleMessageHandlerResolver(SingleMessageHandlerResolver singleMessageHandlerResolver)
        {
            _singleMessageHandlerResolver = singleMessageHandlerResolver ?? throw new ArgumentNullException(nameof(singleMessageHandlerResolver));
        }

        #endregion Constructors

        #region IMessageHandlerResolver Implementation
        
        /// <summary>
        /// Resolve a message handler delegate for the message type and throws an exception if no handler is found.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <returns>Message handler delegate.</returns>
        /// <exception cref="Xer.Delegator.Exceptions.NoMessageHandlerResolvedException">Throws when no message handler delegate is found.</exception>  
        public MessageHandlerDelegate<TMessage> ResolveMessageHandler<TMessage>() where TMessage : class
        {
            MessageHandlerDelegate<TMessage> messageHandler = _singleMessageHandlerResolver.ResolveMessageHandler<TMessage>();
            
            // Throw if resolved handler is a null message handler.
            if(messageHandler == null || messageHandler == NullMessageHandlerDelegate<TMessage>.Value)
            {
                throw ExceptionBuilder.NoMessageHandlerResolvedException(typeof(TMessage));
            }

            return messageHandler;
        }

        #endregion IMessageHandlerResolver Implementation
    }
}