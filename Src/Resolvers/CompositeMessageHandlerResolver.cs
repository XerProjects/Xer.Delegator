using System;
using System.Collections.Generic;
using Xer.Delegator.Exceptions;

namespace Xer.Delegator.Resolvers
{
    public class CompositeMessageHandlerResolver : IMessageHandlerResolver
    {
        #region Declarations
            
        private readonly IEnumerable<IMessageHandlerResolver> _resolvers;

        #endregion Declarations

        #region Properties

        /// <summary>
        /// Determines whether this resolver will throw if no message handler delegate is found from all its sources.
        /// </summary>
        public bool ThrowIfNoHandlerIsFound { get; }

        #endregion Properties

        #region Constructors
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="singleMessageHandlerResolvers">List of single message handler resolvers.</param>
        public CompositeMessageHandlerResolver(IEnumerable<SingleMessageHandlerResolver> singleMessageHandlerResolvers, bool throwIfNoHandlerIsFound = true)
        {
            ThrowIfNoHandlerIsFound = throwIfNoHandlerIsFound;
            _resolvers = singleMessageHandlerResolvers;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="multiMessageHandlerResolvers">List of multi message handler resolvers.</param>
        public CompositeMessageHandlerResolver(IEnumerable<MultiMessageHandlerResolver> multiMessageHandlerResolvers, bool throwIfNoHandlerIsFound = true)
        {
            ThrowIfNoHandlerIsFound = throwIfNoHandlerIsFound;
            _resolvers = multiMessageHandlerResolvers;
        }

        #endregion Constructors

        #region IMessageHandlerResolver Implementation
        
        /// <summary>
        /// Resolve a message handler delegate for the message type from multiple sources.
        /// This will try resolving a message handler delegate from all sources until a handler
        /// who is either not null or not equal to <see cref="Xer.Delegator.NullMessageHandlerDelegate{TMessage}.Value"/> is found.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <returns>Message handler delegate.</returns>
        public MessageHandlerDelegate<TMessage> ResolveMessageHandler<TMessage>() where TMessage : class
        {
            try
            {
                // Resolvers can either be a list of SingleMessageHandlerResolver or MultiMessageHandlerResolver.
                foreach (IMessageHandlerResolver resolver in _resolvers)
                {
                    MessageHandlerDelegate<TMessage> messageHandlerDelegate = resolver.ResolveMessageHandler<TMessage>();
                    
                    if (messageHandlerDelegate != null && 
                        messageHandlerDelegate != NullMessageHandlerDelegate<TMessage>.Value)
                    {
                        return messageHandlerDelegate;
                    }
                }
                
                if(ThrowIfNoHandlerIsFound)
                {
                    // Throw if no handler is found from all sources.
                    throw NoMessageHandlerResolvedException.FromMessageType(typeof(TMessage));
                }

                // Return null handler that does nothing.
                return NullMessageHandlerDelegate<TMessage>.Value;
            }
            catch(Exception ex)
            {
                throw NoMessageHandlerResolvedException.FromMessageType(typeof(TMessage), ex);
            }
        }

        #endregion IMessageHandlerResolver Implementation
    }
}