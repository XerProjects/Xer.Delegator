using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xer.Delegator.Registrations;
using Xer.Delegator.Tests.Entities;
using Xunit;

namespace Xer.Delegator.Tests.Registration
{
    public static class SingleMessageHandlerRegistrationTests
    {
        #region Register Method
            
        public class RegisterMethod
        {
            [Fact]
            public void ShouldOnlyAllowOneAsyncDelegatePerMessageType()
            {
                // Given
                Action action = () =>
                {
                    SingleMessageHandlerRegistration registration = CreateSingleHandlerRegistration();
                    // When
                    registration.Register<TestMessage>((message, ct) => Task.CompletedTask);
                    registration.Register<TestMessage>((message, ct) => Task.CompletedTask);
                };

                // Then
                action.ShouldThrow<InvalidOperationException>("only one message handler delegate per message type should be allowed.");
            }

            [Fact]
            public void ShouldOnlyAllowOneSyncDelegatePerMessageType()
            {
                // Given
                Action action = () =>
                {
                    SingleMessageHandlerRegistration registration = CreateSingleHandlerRegistration();

                    // Stub
                    Action<TestMessage> doNothingAction = (message) => {};

                    // When
                    registration.Register<TestMessage>(doNothingAction);
                    registration.Register<TestMessage>(doNothingAction);
                };

                // Then
                action.ShouldThrow<InvalidOperationException>("only one message handler delegate per message type should be allowed.");
            }

            [Fact]
            public void ShouldNotAllowNullWhenRegisteringAsyncHandler()
            {
                // Given
                Action action = () =>
                {
                    SingleMessageHandlerRegistration registration = CreateSingleHandlerRegistration();
                    // When
                    registration.Register<TestMessage>(null);
                };

                // Then
                action.ShouldThrow<ArgumentNullException>("null is not allowed.");
            }

            [Fact]
            public void ShouldNotAllowNullWhenRegisteringSyncHandler()
            {
                // Given
                Action action = () =>
                {
                    SingleMessageHandlerRegistration registration = CreateSingleHandlerRegistration();
                    // When
                    registration.Register((Action<TestMessage>)null);
                };

                // Then
                action.ShouldThrow<ArgumentNullException>("null is not allowed.");
            }
        }

        #endregion Register Method

        #region Common Methods

        public static SingleMessageHandlerRegistration CreateSingleHandlerRegistration()
        {
            return new SingleMessageHandlerRegistration();
        }

        #endregion Common Methods
    }
}
