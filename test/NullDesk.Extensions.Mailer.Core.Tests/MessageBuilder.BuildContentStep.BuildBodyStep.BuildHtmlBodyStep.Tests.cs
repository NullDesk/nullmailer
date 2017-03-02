using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class BuildHtmlBodyStep
    {


        [Theory]
        [InlineData("text content")]
        [InlineData("")]
        [InlineData(null)]
        public void AndPlainText(string text)
        {
            var body = new ContentBody().WithHtml("<tag>something</tag>");
            var contentStep = new MessageBuilder.BuildContentStep.BuildBodyStep
                .BuildHtmlBodyStep(MailerMessage.Create().WithBody(body), body);
            var stepBuilder = contentStep.AndPlainText(text);
            stepBuilder
                .Should().NotBeNull()
                .And.BeOfType<MessageBuilder.BuildContentStep.BuildBodyStep.BuildBodyCompleteStep>()
                .Which.As<IBuilderContext>()
                .Message.Body
                .As<ContentBody>().ShouldBeEquivalentTo
                (
                    new ContentBody
                    {
                        HtmlContent = "<tag>something</tag>",
                        PlainTextContent = text
                    },
                    config => config
                        .Including(b => b.HtmlContent)
                        .Including(b => b.PlainTextContent));
        }

        [Fact]
        public void And()
        {
            var contentStep = 
                new MessageBuilder
                    .BuildContentStep
                        .BuildBodyStep
                            .BuildHtmlBodyStep(MailerMessage.Create(), ContentBody.Create());
            contentStep.And
                .Should().NotBeNull()
                .And.BeOfType<MessageBuilder.BuildPostContentStep>();
        }

        [Fact]
        public void Build()
        {
            var body = new ContentBody().WithHtml("<tag>something</tag>");
            var contentStep = new MessageBuilder.BuildContentStep.BuildBodyStep
                .BuildHtmlBodyStep(MailerMessage.Create().WithBody(body), body);
            var message = contentStep.Build();
            message
                .Should().NotBeNull()
                .And.BeOfType<MailerMessage>()
                .Which.Body.As<ContentBody>().HtmlContent
                .Should().NotBeNullOrEmpty()
                .And.Be("<tag>something</tag>");
        }
    }
}
