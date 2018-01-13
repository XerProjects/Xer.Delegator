using System;
using Xer.Delegator.Exceptions;

namespace Xer.Delegator.Internal
{
    internal static class ExceptionBuilder
    {
        #region Methods
        
        /// <summary>
        /// Build an exception to indicate that no message handler is registered for a message.
        /// </summary>
        /// <param name="messageType">Type of message.</param>
        /// <param name="ex">Inner exception, if available.</param>
        /// <returns>Instance of <see cref="Xer.Delegator.Exceptions.NoMessageHandlerResolvedException"/>.</returns>
        internal static NoMessageHandlerResolvedException NoMessageHandlerResolvedException(Type messageType, Exception ex = null)
        {
            if(ex != null)
            {
                return new NoMessageHandlerResolvedException($"Error occurred while trying to resolve message handler for { messageType.Name }.", messageType, ex);
            }
            
            return new NoMessageHandlerResolvedException($"Unable to resolve message handler for { messageType.Name }.", messageType, ex);
        }

        #endregion Methods
    }
}