using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xer.Delegator.Tests.Entities;
using Xunit;

// To access common methods.
using static Xer.Delegator.Tests.Resolvers.MultiMessageHandlerResolverTests;
using static Xer.Delegator.Tests.Resolvers.SingleMessageHandlerResolverTests;

namespace Xer.Delegator.Tests
{
    public class MessageDelegatorTests
    {
        #region Construction

        public class MessageDelegatorConstruction
        {
            [Fact]
            public void ShouldThrowWhenNullProvidedAsConstructorParameter()
            {
                // Given
                Action action = () =>
                {
                    // When
                    MessageDelegator messageDelegator = new MessageDelegator(null);
                };

                // Then
                action.ShouldThrow<ArgumentNullException>("because MessageDelegator requires an instance if IMessageHandlerResolver.");
            }
        }

        #endregion Construction

        #region SendAsync Method

        public class SendAsyncMethod
        {
            [Fact]
            public async Task ShouldSendMessageToMultipleMessageHandlers()
            {
                // Given
                int expectedNumberOfHandlers = 10;
                int actualMessageHandlerInvocationCount = 0;

                IMessageHandlerResolver resolver = CreateMultiMessageHandlerResolver(registration =>
                {
                    // Register 10 handlers.
                    for(int i = 0; i < expectedNumberOfHandlers; i++)
                        registration.Register<TestMessage>((message, ct) => { actualMessageHandlerInvocationCount++; return Task.CompletedTask; });
                });

                MessageDelegator messageDelegator = CreateMessageDelegator(resolver);

                // When
                await messageDelegator.SendAsync(new TestMessage());

                // Then
                actualMessageHandlerInvocationCount.Should().Be(expectedNumberOfHandlers);
            }

            [Fact]
            public async Task ShouldSendAllMessagesToMultipleMessageHandlers()
            {
                // Given
                int numberOfHandlers = 10;
                int expectedMessageHandlerInvocationCount = 5;
                int actualMessageHandlerInvocationCount = 0;

                IMessageHandlerResolver resolver = CreateMultiMessageHandlerResolver(registration =>
                {
                    // Register 10 handlers.
                    for(int i = 0; i < numberOfHandlers; i++)
                    {
                        registration.Register<TestMessage>((message, ct) => 
                        { 
                            actualMessageHandlerInvocationCount++; 
                            return Task.CompletedTask; 
                        });
                    }
                });               

                MessageDelegator messageDelegator = CreateMessageDelegator(resolver);

                // Instance messages.
                IEnumerable<TestMessage> messages = Enumerable.Range(0, expectedMessageHandlerInvocationCount).Select(i => new TestMessage());

                // When
                await messageDelegator.SendAllAsync(messages);

                // Then
                int totalInvocationCount = numberOfHandlers * expectedMessageHandlerInvocationCount;
                actualMessageHandlerInvocationCount.Should().Be(totalInvocationCount);
            }

            [Fact]
            public async Task ShouldSendMessageToASingleMessageHandler()
            {
                // Given
                bool messageHandlerWasInvoked = false;

                IMessageHandlerResolver resolver = CreateSingleMessageHandlerResolver(registration =>
                {
                    registration.Register<TestMessage>((message, ct) => 
                    { 
                        messageHandlerWasInvoked = true; 
                        return Task.CompletedTask; 
                    });
                });               

                MessageDelegator messageDelegator = CreateMessageDelegator(resolver);

                // When
                await messageDelegator.SendAsync(new TestMessage());

                // Then
                messageHandlerWasInvoked.Should().BeTrue();
            }

            [Fact]
            public async Task ShouldSendAllMessagesToASingleMessageHandler()
            {
                // Given
                int expectedMessageHandlerInvocationCount = 5;
                int actualMessageHandlerInvocationCount = 0;

                IMessageHandlerResolver resolver = CreateSingleMessageHandlerResolver(registration =>
                {
                    registration.Register<TestMessage>((message, ct) => 
                    { 
                        actualMessageHandlerInvocationCount++; 
                        return Task.CompletedTask; 
                    });
                });               

                MessageDelegator messageDelegator = CreateMessageDelegator(resolver);

                // Instance messages.
                IEnumerable<TestMessage> messages = Enumerable.Range(0, expectedMessageHandlerInvocationCount).Select(i => new TestMessage());

                // When
                await messageDelegator.SendAllAsync(messages);

                // Then
                actualMessageHandlerInvocationCount.Should().Be(expectedMessageHandlerInvocationCount);
            }

             [Fact]
            public void ShouldThrowWhenNullIsProvided()
            {
                // Given
                IMessageHandlerResolver resolver = CreateSingleMessageHandlerResolver();
                MessageDelegator messageDelegator = CreateMessageDelegator(resolver);

                Func<Task> action = () =>
                {
                    // When
                    return messageDelegator.SendAsync<TestMessage>(null);
                };

                // Then
                action.ShouldThrow<ArgumentNullException>();
            }
        }

        #endregion SyncAsync Method

        #region Common Methods
        
        public static MessageDelegator CreateMessageDelegator(IMessageHandlerResolver resolver)
        {
            if (resolver == null)
            {
                throw new System.ArgumentNullException(nameof(resolver));
            }

            return new MessageDelegator(resolver);
        }

        #endregion Common Methods
    }
}