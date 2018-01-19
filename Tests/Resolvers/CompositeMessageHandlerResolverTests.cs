using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xer.Delegator.Resolvers;
using Xer.Delegator.Tests.Entities;
using Xunit;

// To access common methods.
using static Xer.Delegator.Tests.Resolvers.MultiMessageHandlerResolverTests;
using static Xer.Delegator.Tests.Resolvers.SingleMessageHandlerResolverTests;

namespace Xer.Delegator.Tests.Resolvers
{
    public static class CompositeMessageHandlerResolverTests
    {
        #region ResolveMessageHandler Method
        
        public class ResolveMessageHandlerMethod
        {
            [Fact]
            public void ShouldResolveTheFirstAvaibleMessageHandlerFromListOfResolvers()
            {
                // Given
                int expectedNumberOfHandlers = 3;
                int actualNumberOfHandlers = 0;

                IMessageHandlerResolver multiResolver = CreateMultiMessageHandlerResolver(multiRegistration =>
                {
                    // These should be invoked.
                    multiRegistration.Register<TestMessage>((message, ct) => { actualNumberOfHandlers++; return Task.CompletedTask; });
                    multiRegistration.Register<TestMessage>((message, ct) => { actualNumberOfHandlers++; return Task.CompletedTask; });
                    multiRegistration.Register<TestMessage>((message, ct) => { actualNumberOfHandlers++; return Task.CompletedTask; });
                });

                // This should not be invoked.
                IMessageHandlerResolver singleResolver = CreateSingleMessageHandlerResolver(singleRegistration => 
                {
                    singleRegistration.Register<TestMessage>((message, ct) => { actualNumberOfHandlers++; return Task.CompletedTask; });
                });

                CompositeMessageHandlerResolver compositeResolver = CreateCompositeMessageHandlerResolver(multiResolver, singleResolver);

                // When
                MessageHandlerDelegate<TestMessage> handler = compositeResolver.ResolveMessageHandler<TestMessage>();
                handler.Invoke(new TestMessage()); // Should invode the handlers registered in multi registration
                
                // Then
                actualNumberOfHandlers.ShouldBeEquivalentTo(expectedNumberOfHandlers);
            }

            [Fact]
            public void ShouldReturnNullMessageHandlerIfNoMessageHandlerIsFound()
            {
                // Given empty registration
                IMessageHandlerResolver multiRegistration = CreateMultiMessageHandlerResolver();
                IMessageHandlerResolver singleRegistration = CreateSingleMessageHandlerResolver();
                CompositeMessageHandlerResolver compositeResolver = CreateCompositeMessageHandlerResolver(multiRegistration, singleRegistration);

                // When
                MessageHandlerDelegate<TestMessage> handler = compositeResolver.ResolveMessageHandler<TestMessage>();
                
                // Then
                handler.ShouldBeEquivalentTo(NullMessageHandlerDelegate<TestMessage>.Value);
            }
        }

        #endregion ResolveMessageHandler Method

        #region Common Methods

        public static CompositeMessageHandlerResolver CreateCompositeMessageHandlerResolver(params IMessageHandlerResolver[] innerResolvers)
        {
            if (innerResolvers == null)
            {
                throw new ArgumentNullException(nameof(innerResolvers));
            }

            return new CompositeMessageHandlerResolver(innerResolvers);
        }

        #endregion Common Methods
    }
}