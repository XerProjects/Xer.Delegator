using System;

namespace Xer.Delegator
{
    public interface IMessageHandlerRegistration
    {
        /// <summary>
        /// Register an asynchronous message handler delegate for the message type.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <param name="messageHandler">Asynchronous message handler delegate.</param>
        void Register<TMessage>(MessageHandlerDelegate<TMessage> messageHandler) where TMessage : class;

        /// <summary>
        /// Register a synchronous message handler delegate for the message type.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <param name="syncMessageHandler">Synchronous message handler delegate.</param>
        void Register<TMessage>(Action<TMessage> syncMessageHandler) where TMessage : class;
    }
}