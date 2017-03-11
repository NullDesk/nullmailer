﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NullDesk.Extensions.Mailer.MailKit.Tests.Infrastructure;
using NullDesk.Extensions.Mailer.Tests.Common;
using Xunit;
using FluentAssertions;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.MailKit.Tests
{
    public class MailKitMessageHistoryTests : IClassFixture<FactoryMailFixture>
    {


        private FactoryMailFixture Fixture { get; }

        public MailKitMessageHistoryTests(FactoryMailFixture fixture)
        {
            ReplacementVars.Add("%name%", "Mr. Toast");
            Fixture = fixture;
        }
        private Dictionary<string, string> ReplacementVars { get; } = new Dictionary<string, string>();


        [Theory]
        [Trait("TestType", "LocalOnly")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task MailKit_ReSendMail(string html, string text, string[] attachments)
        {
            attachments = attachments?.Select(a => Path.Combine(AppContext.BaseDirectory, a)).ToArray() ?? new string[0];

            var mailer = Fixture.Mail.GetMailer();
            ((Mailer<MkSmtpMailerSettings>)mailer).HistoryStore.SerializeAttachments = true;

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
            //setting this will not effect if the message can be resent, but will effect if the resend itself can be resent
            ((Mailer<MkSmtpMailerSettings>)resendMailer).HistoryStore.SerializeAttachments = false;
            var di = await resendMailer.ReSend(m.Id, CancellationToken.None);
            di.IsSuccess.Should().BeTrue();

            var secondResendMailer = (IHistoryMailer)Fixture.Mail.GetMailer();
            ((Mailer<MkSmtpMailerSettings>)secondResendMailer).HistoryStore.SerializeAttachments = false;


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

