using System;
using System.Threading;
using System.Threading.Tasks;
using Xer.Delegator.Exceptions;

namespace Xer.Delegator
{
    /// <summary>
    /// Represents an object that delegates messages to one or more message handlers.
    /// </summary>
    public class MessageDelegator : IMessageDelegator
    {
        #region Declarations
            
        private readonly IMessageHandlerResolver _messageHandlerResolver;

        #endregion Declarations

        #region Constructors
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="messageHandlerResolver">Message handler resolver.</param>
        public MessageDelegator(IMessageHandlerResolver messageHandlerResolver)
        {
            _messageHandlerResolver = messageHandlerResolver ?? throw new ArgumentNullException(nameof(messageHandlerResolver));
        }

        #endregion Constructors

        #region IMessageDispatcher Implementation
        
        /// <summary>
        /// Send message to a delegate with one/many handlers.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Optional cancellation token to be passed to handlers.</param>
        /// <returns>Asynchronous task which can be awaited for completion.</returns>
        public Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default(CancellationToken)) where TMessage : class
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            
            Type messageType = message.GetType();

            MessageHandlerDelegate messageHandler = _messageHandlerResolver.ResolveMessageHandler(messageType);

            if(messageHandler == null)
            {
                throw NoMessageHandlerResolvedException.WithMessageType(messageType);
            }

            return messageHandler.Invoke(message, cancellationToken);
        }

        #endregion IMessageDispatcher Implementation
    }
}