using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class BuildBodyStepTests
    {
        [Theory]
        [InlineData("<tag>content</tag>")]
        [InlineData("")]
        [InlineData(null)]
        [Trait("TestType", "Unit")]
        public void BuildBodyStep_WithHtml(string html)
        {
            var contentStep = new MessageBuilder.BuildContentStep.BuildBodyStep(MailerMessage.Create());
            var stepBuilder = contentStep.WithHtml(html);
            stepBuilder
                .Should().NotBeNull()
                .And.BeOfType<MessageBuilder.BuildContentStep.BuildBodyStep.BuildHtmlBodyStep>()
                .Which.As<IBuilderContext>()
                .Message.Body
                .As<ContentBody>().HtmlContent
                .Should().Be(html);
        }

        [Theory]
        [InlineData("text content")]
        [InlineData("")]
        [InlineData(null)]
        [Trait("TestType", "Unit")]
        public void BuildBodyStep_WithPlainText(string text)
        {
            var contentStep = new MessageBuilder.BuildContentStep.BuildBodyStep(MailerMessage.Create());
            var stepBuilder = contentStep.WithPlainText(text);
            stepBuilder
                .Should().NotBeNull()
                .And.BeOfType<MessageBuilder.BuildContentStep.BuildBodyStep.BuildTextBodyStep>()
                .Which.As<IBuilderContext>()
                .Message.Body
                .As<ContentBody>().PlainTextContent
                .Should().Be(text);
        }
    }
}