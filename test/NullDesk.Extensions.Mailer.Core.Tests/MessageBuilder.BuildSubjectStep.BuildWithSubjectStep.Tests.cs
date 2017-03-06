using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class BuildWithSubjectStepTests
    {
        [Fact]
        public void And()
        {
            var stepBuilder = new MessageBuilder.BuildSubjectStep.BuildWithSubjectStep(new MailerMessage());
            var subStep = stepBuilder.And;
            subStep
                .Should().NotBeNull()
                .And.BeOfType<MessageBuilder.BuildRecipientsStep>();
        }
    }
}