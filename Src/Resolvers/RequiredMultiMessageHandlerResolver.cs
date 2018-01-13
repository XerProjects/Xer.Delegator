using System;
using Xer.Delegator.Internal;

namespace Xer.Delegator.Resolvers
{
    public class RequiredMultiMessageHandlerResolver : IMessageHandlerResolver
    {
        #region Declarations
            
        private readonly MultiMessageHandlerResolver _multiMessageHandlerResolver;

        #endregion Declarations

        #region Constructors
        
        /// <summary>
        /// Multi message handler resolver instance to decorate.
        /// </summary>
        /// <param name="multiMessageHandlerResolver">Multi message handler resolver.</param>
        public RequiredMultiMessageHandlerResolver(MultiMessageHandlerResolver multiMessageHandlerResolver)
        {
            _multiMessageHandlerResolver = multiMessageHandlerResolver ?? throw new ArgumentNullException(nameof(multiMessageHandlerResolver));    
        }

        #endregion Constructors

        #region IMessageHandlerResolver Implementation
        
        /// <summary>
        /// Resolve a message handler delegate for the message type and throws an exception if no handler is found.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <returns>Message handler delegate which invokes all registered handlers.</returns>
        /// <exception cref="Xer.Delegator.Exceptions.NoMessageHandlerResolvedException">Throws when no message handler delegate is found.</exception>  
        public MessageHandlerDelegate<TMessage> ResolveMessageHandler<TMessage>() where TMessage : class
        {
            MessageHandlerDelegate<TMessage> messageHandler = _multiMessageHandlerResolver.ResolveMessageHandler<TMessage>();
            
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