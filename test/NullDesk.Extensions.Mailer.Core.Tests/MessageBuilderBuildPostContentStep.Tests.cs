using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class BuildPostContentStep
    {
        [Theory]
        [InlineData(@"..\..\..\..\TestData\attachments\testFile.1.txt")]
        [InlineData("")]
        [InlineData(null)]
        [Trait("TestType", "Unit")]
        public void WithAttachment(string fileName)
        {
            var contentStep = new MessageBuilder.BuildPostContentStep(MailerMessage.Create());
            if (string.IsNullOrEmpty(fileName))
            {
                contentStep.Invoking(s => s.WithAttachment(fileName)).ShouldThrow<FileNotFoundException>();
            }
            else
            {
                var path = Path.Combine(AppContext.BaseDirectory, fileName);

                var stepBuilder = contentStep.WithAttachment(path);

                stepBuilder
                    .Should().NotBeNull()
                    .And.BeOfType<MessageBuilder.BuildPostContentStep.BuildAttachmentOrSubstitutionStep>()
                    .Which.As<IBuilderContext>()
                    .Message.Attachments
                    .Should().NotBeEmpty()
                    .And.Subject.Keys.Select(Path.GetFileName)
                    .Should().Contain(Path.GetFileName(path));
            }
        }

        [Theory]
        [InlineData("a", "b")]
        [InlineData("", "d")]
        [InlineData("e", "")]
        [InlineData(null, "g")]
        [InlineData("h", null)]
        [Trait("TestType", "Unit")]
        public void WithSubstitution(string token, string value)
        {
            var contentStep = new MessageBuilder.BuildPostContentStep(MailerMessage.Create());
            if (token == null)
            {
                contentStep.Invoking(c => c.WithSubstitution(null, value)).ShouldThrow<ArgumentNullException>();
            }
            else
            {
                var stepBuilder = contentStep.WithSubstitution(token, value);

                stepBuilder
                    .Should().NotBeNull()
                    .And.BeOfType<MessageBuilder.BuildPostContentStep.BuildAttachmentOrSubstitutionStep>()
                    .Which.As<IBuilderContext>().Message.Substitutions
                    .Should().NotBeEmpty()
                    .And.ContainKey(token)
                    .WhichValue.Should().Be(value);
            }
        }

        [Fact]
        [Trait("TestType", "Unit")]
        public void Build()
        {
            var contentStep = new MessageBuilder.BuildPostContentStep(MailerMessage.Create());

            var message = contentStep.Build();

            message
                .Should().NotBeNull()
                .And.BeOfType<MailerMessage>();
        }
    }
}