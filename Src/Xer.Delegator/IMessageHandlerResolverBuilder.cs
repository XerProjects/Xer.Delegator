namespace Xer.Delegator
{
    /// <summary>
    /// Represents an object that build an instance of <see cref="Xer.Delegator.IMessageHandlerResolver"/>.
    /// </summary>
    public interface IMessageHandlerResolverBuilder
    {
        /// <summary>
        /// Build an instance of <see cref="Xer.Delegator.IMessageHandlerResolver"/>.
        /// </summary>
        /// <returns>Instance of <see cref="Xer.Delegator.IMessageHandlerResolver"/>.</returns>
        IMessageHandlerResolver BuildMessageHandlerResolver();
    }
}