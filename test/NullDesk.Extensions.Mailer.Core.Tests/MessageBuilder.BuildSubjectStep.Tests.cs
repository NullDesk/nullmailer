using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class BuildSubjectStepTests
    {
        [Theory]
        [InlineData("Some Subject")]
        [InlineData("")]
        [InlineData(null)]
        [Trait("TestType", "Unit")]
        public void Subject(string subject)
        {
            var stepBuilder = new MessageBuilder.BuildSubjectStep(new MailerMessage());
            var subStep = stepBuilder.Subject(subject);

            subStep
                .Should().NotBeNull()
                .And.BeOfType<MessageBuilder.BuildSubjectStep.BuildWithSubjectStep>()
                .Which.As<IBuilderContext>().Message.Subject
                .Should().Be(subject);
        }

        [Fact]
        [Trait("TestType", "Unit")]
        public void WithoutSubject()
        {
            var stepBuilder = new MessageBuilder.BuildSubjectStep(new MailerMessage());
            var subStep = stepBuilder.WithOutSubject();
            subStep
                .Should().NotBeNull()
                .And.BeOfType<MessageBuilder.BuildSubjectStep.BuildWithSubjectStep>()
                .Which.As<IBuilderContext>().Message.Subject
                .Should().BeNull();
        }
    }
}