using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xer.Delegator.Registrations;
using Xer.Delegator.Tests.Entities;
using Xunit;

namespace Xer.Delegator.Tests.Registration
{
    public static class MultiMessageHandlerRegistrationTests
    {
        #region Register Method
            
        public class RegisterMethod
        {
            [Fact]
            public void ShouldAllowMultipleAsyncDelegatesPerMessageType()
            {
                // Given
                Action action = () =>
                {
                    MultiMessageHandlerRegistration registration = CreateMultiHandlerRegistration();
                    // When
                    registration.Register<TestMessage>((message, ct) => Task.CompletedTask);
                    registration.Register<TestMessage>((message, ct) => Task.CompletedTask);
                    registration.Register<TestMessage>((message, ct) => Task.CompletedTask);
                };

                // Then
                action.ShouldNotThrow<InvalidOperationException>("multiple message handler delegates per message type should be allowed.");
            }

            [Fact]
            public void ShouldNotAllowNullInRegisterAsyncDelegate()
            {
                // Given
                Action action = () =>
                {
                    MultiMessageHandlerRegistration registration = CreateMultiHandlerRegistration();
                    // When
                    registration.Register<TestMessage>(null);
                };

                // Then
                action.ShouldThrow<ArgumentNullException>("null is not allowed.");
            }

            [Fact]
            public void ShouldAllowMultipleSyncDelegatesPerMessageType()
            {
                // Given
                Action action = () =>
                {
                    MultiMessageHandlerRegistration registration = CreateMultiHandlerRegistration();

                    // Stub
                    Action<TestMessage> doNothingAction = (message) => {};

                    // When
                    registration.Register<TestMessage>(doNothingAction);
                    registration.Register<TestMessage>(doNothingAction);
                    registration.Register<TestMessage>(doNothingAction);
                };

                // Then
                action.ShouldNotThrow<InvalidOperationException>("multiple message handler delegates per message type should be allowed.");
            }

            [Fact]
            public void ShouldNotAllowNullInRegisterSyncDelegate()
            {
                // Given
                Action action = () =>
                {
                    MultiMessageHandlerRegistration registration = CreateMultiHandlerRegistration();
                    // When
                    registration.Register((Action<TestMessage>)null);
                };

                // Then
                action.ShouldThrow<ArgumentNullException>("null is not allowed.");
            }
        }

        #endregion Register Method

        #region Common Methods

        public static MultiMessageHandlerRegistration CreateMultiHandlerRegistration()
        {
            return new MultiMessageHandlerRegistration();
        }

        #endregion Common Methods
    }
}