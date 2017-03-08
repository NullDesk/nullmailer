using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class BuildBodyCompleteStep
    {
        [Fact]
        [Trait("TestType", "Unit")]
        public void And()
        {
            var contentStep =
                new MessageBuilder.BuildContentStep.BuildBodyStep.BuildBodyCompleteStep(MailerMessage.Create());
            contentStep.And
                .Should().NotBeNull()
                .And.BeOfType<MessageBuilder.BuildPostContentStep>();
        }

        [Fact]
        [Trait("TestType", "Unit")]
        public void Build()
        {
            var contentStep =
                new MessageBuilder.BuildContentStep.BuildBodyStep.BuildBodyCompleteStep(MailerMessage.Create());
            var message = contentStep.Build();
            message
                .Should().NotBeNull()
                .And.BeOfType<MailerMessage>();
        }
    }
}