using System;
using Xer.Delegator.Exceptions;

namespace Xer.Delegator.Resolvers
{
    /// <summary>
    /// Represents a decorator object that requires an instance of <see cref="Xer.Delegator.MessageHandlerDelegate"/>
    /// from an internal <see cref="Xer.Delegator.IMessageHandlerResolver"/> instance.
    /// </summary>
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
        /// <param name="messageType">Type of message.</param>
        /// <returns>Message handler delegate which invokes all registered handlers.</returns>
        /// <exception cref="Xer.Delegator.Exceptions.NoMessageHandlerResolvedException">Throws when no message handler delegate is found.</exception>
        public MessageHandlerDelegate ResolveMessageHandler(Type messageType)
        {
            try
            {
                MessageHandlerDelegate messageHandler = _messageHandlerResolver.ResolveMessageHandler(messageType);
                
                // Throw if resolved handler is a null message handler.
                if(messageHandler == null || messageHandler == NullMessageHandlerDelegate.Instance)
                {
                    throw NoMessageHandlerResolvedException.WithMessageType(messageType);
                }

                return messageHandler;
            }
            catch(Exception ex)
            {
                throw NoMessageHandlerResolvedException.WithMessageType(messageType, ex);
            }
        }

        #endregion IMessageHandlerResolver Implementation
    }
}