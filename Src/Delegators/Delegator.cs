using System;
using System.Threading;
using System.Threading.Tasks;
using Xer.Delegator.Exceptions;

namespace Xer.Delegator.Delegators
{
    public class Delegator : IDelegator
    {
        #region Declarations
            
        private readonly IMessageHandlerResolver _messageHandlerResolver;

        #endregion Declarations

        #region Constructors
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="messageHandlerResolver">Message handler resolver.</param>
        public Delegator(IMessageHandlerResolver messageHandlerResolver)
        {
            _messageHandlerResolver = messageHandlerResolver ?? throw new ArgumentNullException(nameof(messageHandlerResolver));
        }

        #endregion Constructors

        #region IMessageDispatcher Implementation
        
        /// <summary>
        /// Sends message to handler.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Optional cancellation token to be passed to handlers.</param>
        /// <returns>Asynchronous task which can be awaited for completion.</returns>
        public Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default(CancellationToken)) where TMessage : class
        {
            MessageHandlerDelegate<TMessage> messageHandler = _messageHandlerResolver.ResolveMessageHandler<TMessage>();

            if(messageHandler == null)
            {
                throw NoMessageHandlerResolvedException.FromMessageType(typeof(TMessage));
            }

            return messageHandler.Invoke(message, cancellationToken);
        }

        #endregion IMessageDispatcher Implementation
    }
}