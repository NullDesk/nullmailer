using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class BuildTextBodyStepTests
    {
        [Theory]
        [InlineData("<tag>content</tag>")]
        [InlineData("")]
        [InlineData(null)]
        [Trait("TestType", "Unit")]
        public void BuildTextBody_AndHtml(string html)
        {
            var body = new ContentBody().WithPlainText("some text");
            var contentStep =
                new MessageBuilder.BuildContentStep.BuildBodyStep.BuildTextBodyStep(
                    MailerMessage.Create().WithBody(body), body);
            var stepBuilder = contentStep.AndHtml(html);
            stepBuilder
                .Should()
                .NotBeNull()
                .And.BeOfType<MessageBuilder.BuildContentStep.BuildBodyStep.BuildBodyCompleteStep>()
                .Which.As<IBuilderContext>()
                .Message.Body
                .As<ContentBody>()
                .ShouldBeEquivalentTo
                (
                    new ContentBody
                    {
                        HtmlContent = html,
                        PlainTextContent = "some text"
                    },
                    config => config
                        .Including(b => b.HtmlContent)
                        .Including(b => b.PlainTextContent));
        }

        [Fact]
        [Trait("TestType", "Unit")]
        public void BuildTextBody_And()
        {
            var contentStep =
                new MessageBuilder.BuildContentStep.BuildBodyStep.BuildTextBodyStep(MailerMessage.Create(),
                    ContentBody.Create());
            contentStep.And
                .Should()
                .NotBeNull()
                .And.BeOfType<MessageBuilder.BuildPostContentStep>();
        }

        [Fact]
        [Trait("TestType", "Unit")]
        public void BuildTextBody_Build()
        {
            var body = new ContentBody().WithPlainText("some text");
            var contentStep =
                new MessageBuilder.BuildContentStep.BuildBodyStep.BuildTextBodyStep(
                    MailerMessage.Create().WithBody(body), body);

            var message = contentStep.Build();
            message
                .Should()
                .NotBeNull()
                .And.BeOfType<MailerMessage>()
                .Which.Body.As<ContentBody>()
                .PlainTextContent
                .Should()
                .NotBeNullOrEmpty()
                .And.Be("some text");
        }
    }
}