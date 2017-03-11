using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class BuildAttachmentOrSubstitutionStepTests
    {
        [Fact]
        [Trait("TestType", "Unit")]

        public void BuildAttachmentOrSubstitutionStep_And()
        {
            var contentStep =
                new MessageBuilder.BuildPostContentStep.BuildAttachmentOrSubstitutionStep(MailerMessage.Create());
            contentStep.And
                .Should().NotBeNull()
                .And.BeOfType<MessageBuilder.BuildPostContentStep>();
        }

        [Fact]
        [Trait("TestType", "Unit")]

        public void BuildAttachmentOrSubstitutionStep_Build()
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