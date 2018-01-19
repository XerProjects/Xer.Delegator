using System;
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
                int actualNumberOfHandlers = 0;

                IMessageHandlerResolver resolver = CreateMultiMessageHandlerResolver(registration =>
                {
                    // Register 10 handlers.
                    for(int i = 0; i < expectedNumberOfHandlers; i++)
                        registration.Register<TestMessage>((message, ct) => { actualNumberOfHandlers++; return Task.CompletedTask; });
                });

                MessageDelegator messageDelegator = CreateMessageDelegator(resolver);

                // When
                await messageDelegator.SendAsync(new TestMessage());

                // Then
                actualNumberOfHandlers.Should().Be(expectedNumberOfHandlers);
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