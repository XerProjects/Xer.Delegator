using System;
using System.Collections.Generic;
using Xer.Delegator.Exceptions;

namespace Xer.Delegator.Resolvers
{
    /// <summary>
    /// Represents a decorator object that resolves an instance of <see cref="Xer.Delegator.MessageHandlerDelegate"/>
    /// from an internal collection of <see cref="Xer.Delegator.IMessageHandlerResolver"/> instances.
    /// </summary>
    public class CompositeMessageHandlerResolver : IMessageHandlerResolver
    {
        #region Declarations
            
        private readonly IEnumerable<IMessageHandlerResolver> _resolvers;

        #endregion Declarations

        #region Constructors
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="messageHandlerResolvers">List of message handler resolvers.</param>
        public CompositeMessageHandlerResolver(IEnumerable<IMessageHandlerResolver> messageHandlerResolvers)
        {
            _resolvers = messageHandlerResolvers ?? throw new ArgumentNullException(nameof(messageHandlerResolvers));
        }

        #endregion Constructors

        #region IMessageHandlerResolver Implementation

        /// <summary>
        /// Resolve a message handler delegate for the message type from multiple sources.
        /// This will try resolving a message handler delegate from all sources until a handler
        /// who is either not null or not equal to <see cref="Xer.Delegator.NullMessageHandlerDelegate.Instance"/> is found.
        /// </summary>
        /// <remarks>
        /// If no handler is found, an instance of <see cref="Xer.Delegator.NullMessageHandlerDelegate.Instance"/> is returned.
        /// Any exceptions thrown by the internal resolvers will be propagated.
        /// </remarks>
        /// <param name="messageType">Type of message.</param>
        /// <returns>Message handler delegate.</returns>
        public MessageHandlerDelegate ResolveMessageHandler(Type messageType)
        {
            try
            {
                // Resolvers can either be a list of SingleMessageHandlerResolver or MultiMessageHandlerResolver.
                foreach (IMessageHandlerResolver resolver in _resolvers)
                {
                    MessageHandlerDelegate messageHandlerDelegate = resolver.ResolveMessageHandler(messageType);
                    
                    if (messageHandlerDelegate != null && 
                        messageHandlerDelegate != NullMessageHandlerDelegate.Instance)
                    {
                        return messageHandlerDelegate;
                    }
                }

                // Return null handler that does nothing.
                return NullMessageHandlerDelegate.Instance;
            }
            catch(NoMessageHandlerResolvedException)
            {
                // If a source has thrown this exception, just rethrow.
                throw;
            }
            catch(Exception ex)
            {
                throw NoMessageHandlerResolvedException.WithMessageType(messageType, ex);
            }
        }

        #endregion IMessageHandlerResolver Implementation

        #region Methods
        
        /// <summary>
        /// Create an instance of CompositeMessageHandlerResolver that is composed of all the provided message handler resolvers.
        /// </summary>
        /// <param name="messageHandlerResolvers">Message handler resolvers to combine.</param>
        /// <returns>Instance of CompositeMessageHandlerResolver that is composed of all the given message handler resolvers.</returns>
        public static CompositeMessageHandlerResolver Compose(params IMessageHandlerResolver[] messageHandlerResolvers)
        {
            return new CompositeMessageHandlerResolver(messageHandlerResolvers);
        }

        #endregion Methods
    }
}