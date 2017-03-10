using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class BuildContentStepTests
    {
        [Theory]
        [InlineData("toast@toast.com")]
        [InlineData("")]
        [InlineData(null)]
        [Trait("TestType", "Unit")]
        public void BuildContentStep_To(string address)
        {
            var contentStep = new MessageBuilder.BuildContentStep(MailerMessage.Create());
            var stepBuilder = contentStep.To(address);

            stepBuilder
                .Should().NotBeNull()
                .And.BeOfType<MessageBuilder.BuildRecipientsStep.BuildToStep>()
                .Which.As<IBuilderContext>().Message.Recipients
                .Should().NotBeNull()
                .And.NotBeEmpty()
                .And.AllBeAssignableTo<MessageRecipient>()
                .And.Contain(r => r.EmailAddress == address);
        }

        [Theory]
        [InlineData("toast")]
        [InlineData("")]
        [InlineData(null)]
        [Trait("TestType", "Unit")]
        public void BuildContentStep_ForTemplate(string templateName)
        {
            var contentStep = new MessageBuilder.BuildContentStep(MailerMessage.Create());
            var stepBuilder = contentStep.ForTemplate(templateName);
            stepBuilder
                .Should().NotBeNull()
                .And.BeOfType<MessageBuilder.BuildContentStep.BuildContentTemplateStep>()
                .Which.As<IBuilderContext>()
                .Message.Body.As<TemplateBody>().TemplateName
                .Should().Be(templateName);
        }

        [Fact]
        [Trait("TestType", "Unit")]
        public void BuildContentStep_ForBody()
        {
            var contentStep = new MessageBuilder.BuildContentStep(MailerMessage.Create());
            var stepBuilder = contentStep.ForBody();
            stepBuilder
                .Should().NotBeNull()
                .And.BeOfType<MessageBuilder.BuildContentStep.BuildBodyStep>();
        }
    }
}