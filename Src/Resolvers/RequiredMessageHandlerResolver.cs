using System;
using Xer.Delegator.Exceptions;

namespace Xer.Delegator.Resolvers
{
    public class RequiredMessageHandlerResolver : IMessageHandlerResolver
    {
        #region Declarations
            
        private readonly IMessageHandlerResolver _messageHandlerResolver;

        #endregion Declarations

        #region Constructors
        
        /// <summary>
        /// Multi message handler resolver instance to decorate.
        /// </summary>
        /// <param name="messageHandlerResolver">Message handler resolver.</param>
        public RequiredMessageHandlerResolver(IMessageHandlerResolver messageHandlerResolver)
        {
            _messageHandlerResolver = messageHandlerResolver ?? throw new ArgumentNullException(nameof(messageHandlerResolver));    
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
            try
            {
                MessageHandlerDelegate<TMessage> messageHandler = _messageHandlerResolver.ResolveMessageHandler<TMessage>();
                
                // Throw if resolved handler is a null message handler.
                if(messageHandler == null || messageHandler == NullMessageHandlerDelegate<TMessage>.Value)
                {
                    throw NoMessageHandlerResolvedException.FromMessageType(typeof(TMessage));
                }

                return messageHandler;
            }
            catch(Exception ex)
            {
                throw NoMessageHandlerResolvedException.FromMessageType(typeof(TMessage), ex);
            }
        }

        #endregion IMessageHandlerResolver Implementation
    }
}