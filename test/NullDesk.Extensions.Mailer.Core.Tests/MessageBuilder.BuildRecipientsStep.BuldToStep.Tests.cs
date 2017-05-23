using System;
using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class BuildToStepTests
    {
        [Theory]
        [InlineData("toast@toast.com", "Mr Toast")]
        [InlineData("toast@toast.com", "")]
        [InlineData("toast@toast.com", null)]
        [Trait("TestType", "Unit")]
        public void BuildToStep_WithDisplayName(string address, string display)
        {
            var toStep = new MessageBuilder.BuildRecipientsStep.BuildToStep(new MailerMessage(), address);
            var stepBuilder = toStep.WithDisplayName(display);

            stepBuilder
                .Should()
                .NotBeNull()
                .And.BeOfType<MessageBuilder.BuildRecipientsStep.BuildToStep.BuiltToWithDisplayStep>()
                .Which.As<IBuilderContext>()
                .Message.Recipients
                .Should()
                .NotBeEmpty()
                .And.AllBeAssignableTo<MessageRecipient>()
                .And.Contain(r => r.DisplayName == display);
        }

        [Theory]
        [InlineData("toast@toast.com", "a", "b")]
        [InlineData("toast@toast.com", "", "d")]
        [InlineData("toast@toast.com", "e", "")]
        [InlineData("toast@toast.com", null, "g")]
        [InlineData("toast@toast.com", "h", null)]
        [Trait("TestType", "Unit")]
        public void BuildToStep_WithPersonalizedSubstitution(string address, string token, string value)
        {
            var toStep = new MessageBuilder.BuildRecipientsStep.BuildToStep(new MailerMessage(), address);
            if (token == null)
            {
                toStep.Invoking(c => c.WithPersonalizedSubstitution(null, value)).ShouldThrow<ArgumentNullException>();
            }
            else
            {
                var stepBuilder = toStep.WithPersonalizedSubstitution(token, value);

                stepBuilder
                    .Should()
                    .NotBeNull()
                    .And.BeOfType<MessageBuilder.BuildRecipientsStep.BuildToStep.BuildRecipientSubstitutionStep>()
                    .Which.As<IBuilderContext>()
                    .Message.Recipients
                    .Should()
                    .NotBeEmpty()
                    .And.AllBeAssignableTo<MessageRecipient>()
                    .And.ContainSingle(r => r.EmailAddress == address)
                    .Which.PersonalizedSubstitutions.Should()
                    .ContainKey(token)
                    .WhichValue.Should()
                    .Be(value);
            }
        }

        [Fact]
        [Trait("TestType", "Unit")]
        public void BuildToStep_And()
        {
            var stepBuilder =
                new MessageBuilder.BuildRecipientsStep.BuildToStep(new MailerMessage(), "toast@toast.com");
            var subStep = stepBuilder.And;
            subStep
                .Should()
                .NotBeNull()
                .And.BeOfType<MessageBuilder.BuildContentStep>();
        }
    }
}