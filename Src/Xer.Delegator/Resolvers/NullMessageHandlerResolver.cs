using System;

namespace Xer.Delegator.Resolvers
{
    /// <summary>
    /// Represents an object that always returns a message handler delegate instance from 
    /// <see cref="Xer.Delegator.NullMessageHandlerDelegate.Instance"/>.
    /// </summary>
    public class NullMessageHandlerResolver : IMessageHandlerResolver
    {
        #region Declarations

        private static readonly Lazy<NullMessageHandlerResolver> _instance = new Lazy<NullMessageHandlerResolver>(() => new NullMessageHandlerResolver());
        
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static readonly NullMessageHandlerResolver Instance = _instance.Value;

        #endregion Declarations

        #region Constructor
            
        /// <summary>
        /// Private constructor.
        /// </summary>
        private NullMessageHandlerResolver() { }

        #endregion Constructor

        #region IMessageHandlerResolver Implementation

        /// <summary>
        /// Resolve message handler delegate instance from <see cref="Xer.Delegator.NullMessageHandlerDelegate.Instance"/>.
        /// </summary>
        /// <param name="messageType">Type of message.</param>
        /// <returns>Message handler delegate instance from <see cref="Xer.Delegator.NullMessageHandlerDelegate.Instance"/>.</returns>
        public MessageHandlerDelegate ResolveMessageHandler(Type messageType)
        {
            return NullMessageHandlerDelegate.Instance;
        }

        #endregion IMessageHandlerResolver Implementation
    }
}