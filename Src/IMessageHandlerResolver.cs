using System;

namespace Xer.Delegator
{
    /// <summary>
    /// Represents an object that resolves an instance of <see cref="Xer.Delegator.MessageHandlerDelegate{TMessage}"/> for a given message type.
    /// </summary>
    public interface IMessageHandlerResolver
    {
        /// <summary>
        /// Resolve a message handler delegate for the message.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <returns>Message handler delegate.</returns>
        MessageHandlerDelegate<TMessage> ResolveMessageHandler<TMessage>() where TMessage : class;
    }
}