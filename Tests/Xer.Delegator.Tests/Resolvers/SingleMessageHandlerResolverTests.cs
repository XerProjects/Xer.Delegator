using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xer.Delegator.Registration;
using Xer.Delegator.Tests.Entities;
using Xunit;

namespace Xer.Delegator.Tests.Resolvers
{
    public static class SingleMessageHandlerResolverTests
    {
        #region ResolveMessageHandler Method
        
        public class ResolveMessageHandlerMethod
        {
            
            [Fact]
            public void ShouldResolveTheRegisteredMessageHandler()
            {
                // Given
                IMessageHandlerResolver resolver = CreateSingleMessageHandlerResolver(registration =>
                {
                   registration.Register<TestMessage>((message, ct) => Task.CompletedTask);
                });

                // When
                MessageHandlerDelegate handler = resolver.ResolveMessageHandler(typeof(TestMessage));
                
                // Then
                handler.Should().NotBe(NullMessageHandlerDelegate.Instance, "registered message handler delegate should be resolved.");
            }

            [Fact]
            public void ShouldResolveNullMessageHandlerIfRegistrationIsEmpty()
            {
                // Given empty registration
                IMessageHandlerResolver resolver = CreateSingleMessageHandlerResolver();

                // When
                MessageHandlerDelegate handler = resolver.ResolveMessageHandler(typeof(TestMessage));
                
                // Then
                handler.ShouldBeEquivalentTo(NullMessageHandlerDelegate.Instance);
            }
        }

        #endregion ResolveMessageHandler Method

        #region Common Methods

        public static IMessageHandlerResolver CreateSingleMessageHandlerResolver(Action<SingleMessageHandlerRegistration> registrationAction = null)
        {
            var registration = new SingleMessageHandlerRegistration();

            if (registrationAction != null)
            {
                registrationAction(registration);
            }

            return registration.BuildMessageHandlerResolver();
        }

        #endregion Common Methods
    }
}