using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class BuildBodyStep
    {
        [Theory]
        [InlineData("<tag>content</tag>")]
        [InlineData("")]
        [InlineData(null)]
        public void WithHtml(string html)
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
        public void WithPlainText(string text)
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