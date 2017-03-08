using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class BuildFromWithDisplayStepTests
    {
        [Fact]
        [Trait("TestType", "Unit")]
        public void And()
        {
            var stepBuilder = new MessageBuilder.BuildFromStep.BuildFromWithDisplayStep(new MailerMessage());
            var subStep = stepBuilder.And;
            subStep
                .Should().NotBeNull()
                .And.BeOfType<MessageBuilder.BuildSubjectStep>();
        }
    }
}