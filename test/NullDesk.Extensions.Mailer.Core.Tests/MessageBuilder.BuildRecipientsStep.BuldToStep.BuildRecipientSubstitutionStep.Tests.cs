using System;
using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class BuiltToWithDisplayStepTests
    {
        [Theory]
        [InlineData("toast@toast.com", "a", "b")]
        [InlineData("toast@toast.com", "", "d")]
        [InlineData("toast@toast.com", "e", "")]
        [InlineData("toast@toast.com", null, "g")]
        [InlineData("toast@toast.com", "h", null)]
        [Trait("TestType", "Unit")]
        public void BuiltToWithDisplayStep_WithPersonalizedSubstitution(string address, string token, string value)
        {
            var rec = new MessageRecipient().ToAddress(address);
            var toStep =
                new MessageBuilder.BuildRecipientsStep.BuildToStep.BuiltToWithDisplayStep(new MailerMessage().To(rec),
                    rec);
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
                    .And
                    .BeOfType<MessageBuilder.BuildRecipientsStep.BuildToStep.BuildRecipientWithDisplaySubstitutionStep
                    >()
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
        public void BuiltToWithDisplayStep_And()
        {
            var rec = new MessageRecipient().ToAddress("toast@toast.com");
            var toStep =
                new MessageBuilder.BuildRecipientsStep.BuildToStep.BuiltToWithDisplayStep(new MailerMessage().To(rec),
                    rec);
            toStep.And
                .Should()
                .NotBeNull()
                .And.BeOfType<MessageBuilder.BuildContentStep>();
        }
    }
}