using System.Threading.Tasks;
using FluentAssertions;
using Xer.Delegator.Tests.Entities;
using Xunit;

namespace Xer.Delegator.Tests
{
    public class NullMessageHandlerDelegateTests
    {
        #region Value Field

        public class InstanceField
        {
            [Fact]
            public void ShouldBeEqualIfMessageTypeIsTheSame()
            {
                MessageHandlerDelegate handler1 = NullMessageHandlerDelegate.Instance;
                MessageHandlerDelegate handler2 = NullMessageHandlerDelegate.Instance;

                handler1.Should().BeSameAs(handler2);
            }

            [Fact]
            public void ShouldNotBeEqualToANewEmptyInstance()
            {
                MessageHandlerDelegate handler1 = NullMessageHandlerDelegate.Instance;
                MessageHandlerDelegate handler2 = (m, ct) => Task.CompletedTask;
                MessageHandlerDelegate handler3 = (m, ct) => Task.CompletedTask;

                handler1.Should().NotBeSameAs(handler2);
                handler2.Should().NotBeSameAs(handler3);
            }
        }

        #endregion Value Field
    }
}