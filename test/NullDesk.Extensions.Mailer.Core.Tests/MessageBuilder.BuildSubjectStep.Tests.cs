using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class BuildSubjectStepTests
    {
        [Theory]
        [InlineData("Some Subject")]
        [InlineData("")]
        [InlineData(null)]
        public void BuildSubjectStep_Subject(string subject)
        {
            var stepBuilder = new MessageBuilder.BuildSubjectStep(new MailerMessage());
            var subStep = stepBuilder.Subject(subject);

            subStep
                .Should().NotBeNull()
                .And.BeOfType<MessageBuilder.BuildSubjectStep.BuildWithSubjectStep>()
                .Which.As<IBuilderContext>().Message.Subject
                    .Should().Be(subject);
        }

        [Fact]
        public void BuildSubjectStep_WithoutSubject()
        {
            var stepBuilder = new MessageBuilder.BuildSubjectStep(new MailerMessage());
            var subStep = stepBuilder.WithOutSubject();
            subStep
               .Should().NotBeNull()
               .And.BeOfType<MessageBuilder.BuildSubjectStep.BuildWithSubjectStep>()
               .Which.As<IBuilderContext>().Message.Subject
                    .Should().BeNull();


        }

    }
}
