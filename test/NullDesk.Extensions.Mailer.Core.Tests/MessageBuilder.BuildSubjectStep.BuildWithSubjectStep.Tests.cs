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
    public class BuildWithSubjectStepTests
    {

        [Fact]
        public void BuldFromStepTests_And()
        {
            var stepBuilder = new MessageBuilder.BuildSubjectStep.BuildWithSubjectStep(new MailerMessage());
            var subStep = stepBuilder.And;
            subStep
                .Should().NotBeNull()
                .And.BeOfType<MessageBuilder.BuildRecipientsStep>();
        }

    }
}
