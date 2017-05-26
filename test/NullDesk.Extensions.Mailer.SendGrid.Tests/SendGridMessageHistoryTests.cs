using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.SendGrid.Tests.Infrastructure;
using NullDesk.Extensions.Mailer.Tests.Common;
using Xunit;

namespace NullDesk.Extensions.Mailer.SendGrid.Tests
{
    public class SendGridMessageHistoryTests : IClassFixture<HistoryMailFixture>
    {
        public SendGridMessageHistoryTests(HistoryMailFixture fixture)
        {
            ReplacementVars.Add("%name%", "Mr. Toast");
            Fixture = fixture;
        }

        private HistoryMailFixture Fixture { get; }
        private Dictionary<string, string> ReplacementVars { get; } = new Dictionary<string, string>();


        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task SendGrid_History_SerializedAttachments_ReSendMail(string html, string text,
            string[] attachments)
        {
            attachments = attachments?.Select(a => Path.Combine(AppContext.BaseDirectory, a)).ToArray() ??
                          new string[0];

            var mailer = Fixture.MailerFactoryForHistoryWithSerializableAttachments.GetMailer();

            mailer.CreateMessage(b => b
                .Subject($"xunit Test run: content body")
                .And.To("noone@toast.com")
                .WithDisplayName("No One Important")
                .And.ForBody()
                .WithHtml(html)
                .AndPlainText(text)
                .And.WithSubstitutions(ReplacementVars)
                .And.WithAttachments(attachments)
                .Build());

            var result = await mailer.SendAllAsync(CancellationToken.None);
            var items = result as DeliveryItem[] ?? result.ToArray();
            items
                .Should()
                .HaveCount(1)
                .And.AllBeOfType<DeliveryItem>();
            var history =
                await Fixture.StoreWithSerializableAttachments.GetAsync(items.First().Id, CancellationToken.None);
            var m = history.Should().NotBeNull().And.BeOfType<DeliveryItem>().Which;
            m.IsSuccess.Should().BeTrue();
            m.DeliveryProvider.Should().NotBeNullOrEmpty();


            var di = await mailer.ReSendAsync(m.Id, CancellationToken.None);
            di.IsSuccess.Should().BeTrue();
        }

        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task SendGrid_History_NoSerializedAttachments_ReSendMail(string html, string text,
            string[] attachments)
        {
            attachments = attachments?.Select(a => Path.Combine(AppContext.BaseDirectory, a)).ToArray() ??
                          new string[0];

            var mailer = Fixture.MailerFactoryForHistoryWithoutSerializableAttachments.GetMailer();

            mailer.CreateMessage(b => b
                .Subject($"xunit Test run: content body")
                .And.To("noone@toast.com")
                .WithDisplayName("No One Important")
                .And.ForBody()
                .WithHtml(html)
                .AndPlainText(text)
                .And.WithSubstitutions(ReplacementVars)
                .And.WithAttachments(attachments)
                .Build());

            var result = await mailer.SendAllAsync(CancellationToken.None);
            var items = result as DeliveryItem[] ?? result.ToArray();
            items
                .Should()
                .HaveCount(1)
                .And.AllBeOfType<DeliveryItem>();
            var history =
                await Fixture.StoreWithoutSerializableAttachments.GetAsync(items.First().Id, CancellationToken.None);
            var m = history.Should().NotBeNull().And.BeOfType<DeliveryItem>().Which;
            m.IsSuccess.Should().BeTrue();
            m.DeliveryProvider.Should().NotBeNullOrEmpty();


            if (attachments.Any())
            {
                Func<Task> asyncFunction = () => mailer.ReSendAsync(m.Id, CancellationToken.None);

                asyncFunction.ShouldThrow<InvalidOperationException>();
            }
            else
            {
                var di = await mailer.ReSendAsync(m.Id, CancellationToken.None);
                di.Should().BeOfType<DeliveryItem>().Which.IsSuccess.Should().BeTrue();
            }
        }
    }
}