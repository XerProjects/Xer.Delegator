using System;

namespace Xer.Delegator.Exceptions
{
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
    }
}