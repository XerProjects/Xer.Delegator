using System;

namespace Xer.Delegator
{
    /// <summary>
    /// Represents an object that resolves an instance of <see cref="Xer.Delegator.MessageHandlerDelegate"/> for a given message type.
    /// </summary>
    public interface IMessageHandlerResolver
    {
        /// <summary>
        /// Resolve a message handler delegate for the message.
        /// </summary>
        /// <param name="messageType">Type of message.</param>
        /// <returns>Message handler delegate.</returns>
        MessageHandlerDelegate ResolveMessageHandler(Type messageType);
    }
}