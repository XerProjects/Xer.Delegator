using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xer.Delegator.Registrations;
using Xer.Delegator.Tests.Entities;
using Xunit;

namespace Xer.Delegator.Tests.Resolvers
{
    public static class MultiMessageHandlerResolverTests
    {
        #region ResolveMessageHandler Method
        
        public class ResolveMessageHandlerMethod
        {
            [Fact]
            public void ShouldResolveTheRegisteredMessageHandler()
            {
                // Given
                IMessageHandlerResolver resolver = CreateMultiMessageHandlerResolver(registration =>
                {
                    registration.Register<TestMessage>((message, ct) => Task.CompletedTask);
                    registration.Register<TestMessage>((message, ct) => Task.CompletedTask);
                    registration.Register<TestMessage>((message, ct) => Task.CompletedTask);
                });

                // When
                MessageHandlerDelegate<TestMessage> handler = resolver.ResolveMessageHandler<TestMessage>();
                
                // Then
                handler.Should().NotBe(NullMessageHandlerDelegate<TestMessage>.Value, "registered message handler delegate should be resolved.");
            }

            [Fact]
            public void ShouldReturnNullMessageHandlerWhenRegistrationIsEmpty()
            {
                // Given empty registration
                IMessageHandlerResolver resolver = CreateMultiMessageHandlerResolver();

                // When
                MessageHandlerDelegate<TestMessage> handler = resolver.ResolveMessageHandler<TestMessage>();
                
                // Then
                handler.ShouldBeEquivalentTo(NullMessageHandlerDelegate<TestMessage>.Value);
            }
        }

        #endregion ResolveMessageHandler Method

        #region Common Methods
        
        public static IMessageHandlerResolver CreateMultiMessageHandlerResolver(Action<MultiMessageHandlerRegistration> registrationAction = null)
        {
            var registration = new MultiMessageHandlerRegistration();

            if (registrationAction != null)
            {
                registrationAction(registration);
            }

            return registration.BuildMessageHandlerResolver();
        }

        #endregion Common Methods
    }
}