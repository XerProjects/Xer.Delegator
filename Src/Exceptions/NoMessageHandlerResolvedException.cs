using System;

namespace Xer.Delegator.Exceptions
{
    /// <summary>
    /// Exception that can be thrown when no instance of <see cref="Xer.Delegator.MessageHandlerDelegate{TMessage}"/> can be resolved.
    /// </summary>
    public class NoMessageHandlerResolvedException : Exception
    {
        #region Properties
        
        /// <summary>
        /// Type of message.
        /// </summary>
        public Type MessageType { get; }

        #endregion Properties

        #region Constructors
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="messageType">Type of message.</param>
        public NoMessageHandlerResolvedException(string message, Type messageType) : base(message)
        {
            MessageType = messageType;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="messageType">Type of message.</param>
        /// <param name="innerException">Inner exception.</param>
        public NoMessageHandlerResolvedException(string message, Type messageType, Exception innerException) : base(message, innerException)
        {
            MessageType = messageType;
        }

        #endregion Constructors

        #region Methods
        
        /// <summary>
        /// Create instance of NoMessageHandlerResolvedException with a default messagefor the message type.
        /// </summary>
        /// <param name="messageType">Type of message.</param>
        /// <param name="ex">Inner exception, if available.</param>
        /// <returns>Instance of NoMessageHandlerResolvedException.</returns>
        public static NoMessageHandlerResolvedException WithMessageType(Type messageType, Exception ex = null)
        {
            if (messageType == null)
            {
                throw new ArgumentNullException(nameof(messageType));
            }

            if (ex != null)
            {
                return new NoMessageHandlerResolvedException($"Error occurred while trying to resolve message handler for { messageType.Name }.", messageType, ex);
            }
            
            return new NoMessageHandlerResolvedException($"Unable to resolve message handler for { messageType.Name }.", messageType, ex);
        }

        #endregion Methods
    }
}