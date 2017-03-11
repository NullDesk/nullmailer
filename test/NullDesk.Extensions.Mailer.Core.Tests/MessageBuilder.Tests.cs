using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class MessageBuilderTests
    {
        [Theory]
        [InlineData("toast@toast.com")]
        [InlineData("")]
        [InlineData(null)]
        [Trait("TestType", "Unit")]
        public void MessageBuilder_From(string address)
        {
            var messageBuilder = new MessageBuilder();
            var stepBuilder = messageBuilder.From(address);

            stepBuilder
                .Should().NotBeNull()
                .And.BeOfType<MessageBuilder.BuildFromStep>()
                .Which.As<IBuilderContext>().Message.From
                .Should().NotBeNull()
                .And.BeOfType<MessageSender>().Which
                .EmailAddress
                .Should().Be(address).And
                .BeEquivalentTo(
                    messageBuilder.As<IBuilderContext>()?
                        .Message?
                        .From?
                        .EmailAddress);
        }
    }
}