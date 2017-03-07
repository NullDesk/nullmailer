using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class BuildRecipientsStepTests
    {
        [Theory]
        [InlineData("toast@toast.com")]
        [InlineData("")]
        [InlineData(null)]
        public void To(string address)
        {
            var recipientStep = new MessageBuilder.BuildRecipientsStep(new MailerMessage());
            var stepBuilder = recipientStep.To(address);

            stepBuilder
                .Should().NotBeNull()
                .And.BeOfType<MessageBuilder.BuildRecipientsStep.BuildToStep>()
                .Which.As<IBuilderContext>().Message.Recipients
                .Should().NotBeNull()
                .And.NotBeEmpty()
                .And.AllBeAssignableTo<MessageRecipient>()
                .And.Contain(r => r.EmailAddress == address);
        }
    }
}