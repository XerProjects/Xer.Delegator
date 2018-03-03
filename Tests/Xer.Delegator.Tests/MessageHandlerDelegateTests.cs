using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xer.Delegator.Tests.Entities;
using Xunit;

using static Xer.Delegator.Tests.Resolvers.MultiMessageHandlerResolverTests;
using static Xer.Delegator.Tests.Resolvers.SingleMessageHandlerResolverTests;

namespace Xer.Delegator.Tests
{
    public class MessageHandlerDelegateTests
    {
        #region Null Testing
        
        [Fact]
        public void ShouldThrowIfNullIsPassedToTheMultiHandlerDelegate()
        {
            // Given
            IMessageHandlerResolver resolver = CreateMultiMessageHandlerResolver(registration =>
            {
                registration.Register<TestMessage>((message, ct) => Task.CompletedTask);
                registration.Register<TestMessage>((message, ct) => Task.CompletedTask);
                registration.Register<TestMessage>((message, ct) => Task.CompletedTask);
            });

            // When
            MessageHandlerDelegate handler = resolver.ResolveMessageHandler(typeof(TestMessage));

            // Expected TestMessage but was given a TestInvalidMessage.
            Action action = () => handler.Invoke(null);
            
            // Then
            action.ShouldThrow<ArgumentNullException>("because null was passed-in.");
        }
        
        [Fact]
        public void ShouldThrowIfNullIsPassedToTheSingleHandlerDelegate()
        {
            // Given
            IMessageHandlerResolver resolver = CreateSingleMessageHandlerResolver(registration =>
            {
                registration.Register<TestMessage>((message, ct) => Task.CompletedTask);
            });

            // When
            MessageHandlerDelegate handler = resolver.ResolveMessageHandler(typeof(TestMessage));

            // Expected TestMessage but was given a TestInvalidMessage.
            Action action = () => handler.Invoke(null);
            
            // Then
            action.ShouldThrow<ArgumentNullException>("because null was passed-in.");
        }

        #endregion Null Testing

        #region Invalid Message Testing

        [Fact]
        public void ShouldThrowIfUnexpectedMessageTypeIsPassedToTheMultiHandlerDelegate()
        {
            // Given
            IMessageHandlerResolver resolver = CreateMultiMessageHandlerResolver(registration =>
            {
                registration.Register<TestMessage>((message, ct) => Task.CompletedTask);
                registration.Register<TestMessage>((message, ct) => Task.CompletedTask);
                registration.Register<TestMessage>((message, ct) => Task.CompletedTask);
            });

            // When
            MessageHandlerDelegate handler = resolver.ResolveMessageHandler(typeof(TestMessage));

            // Expected TestMessage but was given a TestInvalidMessage.
            Action action = () => handler.Invoke(new TestInvalidMessage());
            
            // Then
            action.ShouldThrow<ArgumentException>("because MessageHandlerDelegate expects a TestMessage but was given a TestInvalidMessage.");
        }

        [Fact]
        public void ShouldThrowIfUnexpectedMessageTypeIsPassedToTheSingleHandlerDelegate()
        {
            // Given
            IMessageHandlerResolver resolver = CreateSingleMessageHandlerResolver(registration =>
            {
                registration.Register<TestMessage>((message, ct) => Task.CompletedTask);
            });

            // When
            MessageHandlerDelegate handler = resolver.ResolveMessageHandler(typeof(TestMessage));

            // Expected TestMessage but was given a TestInvalidMessage.
            Action action = () => handler.Invoke(new TestInvalidMessage());
            
            // Then
            action.ShouldThrow<ArgumentException>("because MessageHandlerDelegate expects a TestMessage but was given a TestInvalidMessage.");
        }

        #endregion Invalid Message Testing

        #region Inner Classes

        /// <summary>
        /// Sample invalid message.
        /// </summary>
        class TestInvalidMessage { }

        #endregion Inner Classes
    }
}