using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xer.Delegator.Exceptions;
using Xer.Delegator.Registrations;

namespace Xer.Delegator.Resolvers
{
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
        /// If no handler is found, a <see cref="Xer.Delegator.NullMessageHandlerDelegate{TMessage}.Value" /> is returned.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <returns>Message handler delegate which invokes all registered delegates.</returns>
        public MessageHandlerDelegate<TMessage> ResolveMessageHandler<TMessage>() where TMessage : class
        {
            if (_multiMessageHandlerDelegateStore.TryGetValue<TMessage>(out IEnumerable<MessageHandlerDelegate<TMessage>> messageHandlers))
            {
                // Return a delegate that invokes all registered message handlers.
                return (message, cancellationToken) =>
                {
                    IEnumerable<Task> handleTasks = messageHandlers.Select(m => m.Invoke(message, cancellationToken));
                    return Task.WhenAll(handleTasks);
                };
            }

            // Null message handler.
            return NullMessageHandlerDelegate<TMessage>.Value;
        }

        #endregion IMessageHandlerResolver Implementation
    }
}