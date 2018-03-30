using System;
using FluentAssertions;
using Xer.Delegator.Exceptions;
using Xer.Delegator.Registration;
using Xer.Delegator.Resolvers;
using Xer.Delegator.Tests.Entities;
using Xunit;

// To access common methods.
using static Xer.Delegator.Tests.Registration.MultiMessageHandlerRegistrationTests;
using static Xer.Delegator.Tests.Registration.SingleMessageHandlerRegistrationTests;
using static Xer.Delegator.Tests.Resolvers.CompositeMessageHandlerResolverTests;
using static Xer.Delegator.Tests.Resolvers.MultiMessageHandlerResolverTests;
using static Xer.Delegator.Tests.Resolvers.SingleMessageHandlerResolverTests;

namespace Xer.Delegator.Tests.Resolvers
{
    public static class RequiredMessageHandlerResolverTests
    {
        #region ResolveMessageHandler Method

        public class ResolveMessageHandlerMethod
        {
            [Fact]
            public void ShouldThrowWhenNoHandlerIsResolvedFromSingleMessageHandlerResolve()
            {
                // Given empty registration
                SingleMessageHandlerRegistration registration = CreateSingleHandlerRegistration();
                RequiredMessageHandlerResolver requiredResolver = CreateRequiredMessageHandlerResolver(registration.BuildMessageHandlerResolver());
                
                Action action = () =>
                {
                    // When
                    MessageHandlerDelegate handler = requiredResolver.ResolveMessageHandler(typeof(TestMessage));
                };
                
                // Then
                action.ShouldThrow<NoMessageHandlerResolvedException>("RequiredMessageHandlerResolver should require a message handler to be found.");
            }

            [Fact]
            public void ShouldThrowWhenNoHandlerIsResolvedFromMultiMessageHandlerResolve()
            {
                // Given empty registration
                MultiMessageHandlerRegistration registration = CreateMultiHandlerRegistration();
                RequiredMessageHandlerResolver requiredResolver = CreateRequiredMessageHandlerResolver(registration.BuildMessageHandlerResolver());
                
                Action action = () =>
                {
                    // When
                    MessageHandlerDelegate handler = requiredResolver.ResolveMessageHandler(typeof(TestMessage));
                };
                
                // Then
                action.ShouldThrow<NoMessageHandlerResolvedException>("RequiredMessageHandlerResolver should require a message handler to be found.");
            }

            [Fact]
            public void ShouldThrowWhenNoHandlerIsResolvedFromCompositeMessageHandlerResolver()
            {
                // Given empty registration
                IMessageHandlerResolver multiResolver = CreateMultiMessageHandlerResolver();
                IMessageHandlerResolver singleResolver = CreateSingleMessageHandlerResolver();
                CompositeMessageHandlerResolver compositeResolver = CreateCompositeMessageHandlerResolver(singleResolver, multiResolver);
                RequiredMessageHandlerResolver requiredResolver = CreateRequiredMessageHandlerResolver(compositeResolver);
                
                Action action = () =>
                {
                    // When
                    MessageHandlerDelegate handler = requiredResolver.ResolveMessageHandler(typeof(TestMessage));
                };
                
                // Then
                action.ShouldThrow<NoMessageHandlerResolvedException>("RequiredMessageHandlerResolver should require a message handler to be found.");
            }
        }

        #endregion ResolveMessageHandler Method

        #region Common Methods

        public static RequiredMessageHandlerResolver CreateRequiredMessageHandlerResolver(IMessageHandlerResolver inner)
        {
            if (inner == null)
            {
                throw new ArgumentNullException(nameof(inner));
            }

            return new RequiredMessageHandlerResolver(inner);
        }

        #endregion Common Methods
    }
}