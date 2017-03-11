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
        public async Task SendGrid_History_ReSendMail(string html, string text, string[] attachments)
        {
            attachments = attachments?.Select(a => Path.Combine(AppContext.BaseDirectory, a)).ToArray() ?? new string[0];

            var mailer = Fixture.Mail.GetMailer();
            ((Mailer<SendGridMailerSettings>)mailer).HistoryStore.SerializeAttachments = true;

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

            var resendMailer = (IHistoryMailer)Fixture.Mail.GetMailer();
            ((Mailer<SendGridMailerSettings>)resendMailer).HistoryStore.SerializeAttachments = false;

            //doesn't matter if we set the history store SerializeAttachments property now, but it does mean the resent item will not serialize it's attachments (the resent item will itself not be resendable if it had attachents)
            var di = await resendMailer.ReSend(m.Id, CancellationToken.None);
            di.IsSuccess.Should().BeTrue();


            var secondResendMailer = (IHistoryMailer)Fixture.Mail.GetMailer();
            ((Mailer<SendGridMailerSettings>)secondResendMailer).HistoryStore.SerializeAttachments = false;


            Func<Task> asyncFunction = () => secondResendMailer.ReSend(di.Id, CancellationToken.None);
            if (attachments.Any())
            {
               asyncFunction.ShouldThrow<InvalidOperationException>();
            }
            else
            {
                asyncFunction.ShouldNotThrow<InvalidOperationException>();
            }
        }
    }
}

