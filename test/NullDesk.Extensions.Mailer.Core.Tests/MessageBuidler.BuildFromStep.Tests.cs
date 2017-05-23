using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class BuldFromStepTests
    {
        [Theory]
        [InlineData("Mr Toast")]
        [InlineData("")]
        [InlineData(null)]
        [Trait("TestType", "Unit")]
        public void BuldFromStep_WithDisplayName(string name)
        {
            var stepBuilder = new MessageBuilder.BuildFromStep(new MailerMessage().From("toast@toast.com"));
            var subStep = stepBuilder.WithDisplayName(name);
            subStep
                .Should()
                .NotBeNull()
                .And.BeOfType<MessageBuilder.BuildFromStep.BuildFromWithDisplayStep>()
                .Which.As<IBuilderContext>()
                .Message.From
                .Should()
                .NotBeNull()
                .And.BeOfType<MessageSender>()
                .Which
                .DisplayName
                .Should()
                .Be(name)
                .And
                .BeEquivalentTo(
                    stepBuilder.As<IBuilderContext>()
                        ?
                        .Message?
                        .From?
                        .DisplayName);
        }

        [Fact]
        [Trait("TestType", "Unit")]
        public void BuldFromStep_And()
        {
            var stepBuilder = new MessageBuilder.BuildFromStep(new MailerMessage().From("toast@toast.com"));
            var subStep = stepBuilder.And;
            subStep
                .Should()
                .NotBeNull()
                .And.BeOfType<MessageBuilder.BuildSubjectStep>();
        }
    }
}