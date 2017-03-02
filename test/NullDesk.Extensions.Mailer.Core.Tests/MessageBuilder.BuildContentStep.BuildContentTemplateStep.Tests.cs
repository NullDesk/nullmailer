using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class BuildContentTemplateStep
    {
        [Fact]
        public void ForBody()
        {
            var contentStep = new MessageBuilder.BuildContentStep.BuildContentTemplateStep(new MailerMessage().WithBody<TemplateBody>(b => b.TemplateName = "toast"));
            var stepBuilder = contentStep.And;
            stepBuilder
                .Should().NotBeNull()
                .And.BeOfType<MessageBuilder.BuildPostContentStep>();
        }

        [Fact]
        public void Build()
        {
            var contentStep = new MessageBuilder.BuildContentStep.BuildContentTemplateStep(new MailerMessage().WithBody<TemplateBody>(b => b.TemplateName = "toast"));
            var message = contentStep.Build();
            message
                .Should().NotBeNull()
                .And.BeOfType<MailerMessage>()
                .Which.Body.As<TemplateBody>().TemplateName
                .Should().NotBeNullOrEmpty()
                .And.Be("toast");
        }
    }
}
