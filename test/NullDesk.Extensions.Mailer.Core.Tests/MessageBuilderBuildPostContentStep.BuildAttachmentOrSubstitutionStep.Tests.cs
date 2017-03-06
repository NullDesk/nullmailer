using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class BuildAttachmentOrSubstitutionStep
    {
        [Fact]
        public void And()
        {
            var contentStep =
                new MessageBuilder.BuildPostContentStep.BuildAttachmentOrSubstitutionStep(MailerMessage.Create());
            contentStep.And
                .Should().NotBeNull()
                .And.BeOfType<MessageBuilder.BuildPostContentStep>();
        }

        [Fact]
        public void Build()
        {
            var contentStep =
                new MessageBuilder.BuildPostContentStep.BuildAttachmentOrSubstitutionStep(MailerMessage.Create());
            var message = contentStep.Build();
            message
                .Should().NotBeNull()
                .And.BeOfType<MailerMessage>();
        }
    }
}