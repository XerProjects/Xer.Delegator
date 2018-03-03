namespace Xer.Delegator
{
    public static class MessageHandlerResolverExtensions
    {
        /// <summary>
        /// Resolve a message handler delegate for the message type.
        /// </summary>
        /// <typeparam name="TMessage">Type of message.</typeparam>
        /// <returns>Message handler delegate.</returns>
        public static MessageHandlerDelegate ResolveMessageHandler<TMessage>(this IMessageHandlerResolver resolver)
        {
            if (resolver == null)
            {
                throw new System.ArgumentNullException(nameof(resolver));
            }

            return resolver.ResolveMessageHandler(typeof(TMessage));
        }
    }
}