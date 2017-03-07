using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NullDesk.Extensions.Mailer.SendGrid.Tests.Infrastructure;
using NullDesk.Extensions.Mailer.Tests.Common;
using Xunit;
using FluentAssertions;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.SendGrid.Tests
{
    public class SendGridMessageHistoryTests : IClassFixture<FactoryMailFixture>
    {
        private FactoryMailFixture Fixture { get; }

        public SendGridMessageHistoryTests(FactoryMailFixture fixture)
        {
            ReplacementVars.Add("%name%", "Mr. Toast");
            Fixture = fixture;
        }
        private Dictionary<string, string> ReplacementVars { get; } = new Dictionary<string, string>();


        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task ReSendMail(string html, string text, string[] attachments)
        {
            attachments = attachments?.Select(a => Path.Combine(AppContext.BaseDirectory, a)).ToArray();

            var mailer = Fixture.Mail.Mailer;
          
            mailer.CreateMessage(b => b
                .Subject($"xunit Test run: content body")
                .And.To("noone@toast.com").WithDisplayName("No One Important")
                .And.ForBody().WithHtml(html).AndPlainText(text)
                .And.WithSubstitutions(ReplacementVars)
                .And.WithAttachments(attachments).Build());

            var result = await mailer.SendAllAsync(CancellationToken.None);
            var items = result as DeliveryItem[] ?? result.ToArray();
            items
                .Should().HaveCount(1)
                .And.AllBeOfType<DeliveryItem>();
            var history = await Fixture.Store.GetAsync(items.First().Id, CancellationToken.None);
            var m = history.Should().NotBeNull().And.BeOfType<DeliveryItem>().Which;
            m.IsSuccess.Should().BeTrue();
            m.DeliveryProvider.Should().NotBeNullOrEmpty();
        }
    }
}

