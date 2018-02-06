using System;

namespace Xer.Delegator
{
    public static class MessageHandlerRegistrationExtensions
    {      
        /// <summary>
        /// Register a synchronous message handler delegate for the message type.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <param name="syncMessageHandler">Synchronous message handler delegate.</param>
        public static void Register<TMessage>(this IMessageHandlerRegistration registration, Action<TMessage> syncMessageHandler) 
            where TMessage : class
        {
            if (registration == null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            if (syncMessageHandler == null)
            {
                throw new ArgumentNullException(nameof(syncMessageHandler));
            }
            
            // Convert to async delegate.
            registration.Register<TMessage>((message, cancellationToken) =>
            {
                try
                {
                    syncMessageHandler.Invoke(message);
                    return TaskUtility.CompletedTask;
                }
                catch(Exception ex)
                {
                    return TaskUtility.FromException(ex);
                }
            });
        }
    }
}