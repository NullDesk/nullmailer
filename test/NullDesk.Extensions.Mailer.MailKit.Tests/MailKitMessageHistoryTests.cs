using FluentAssertions;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;
using NullDesk.Extensions.Mailer.MailKit.Tests.Infrastructure;
using NullDesk.Extensions.Mailer.Tests.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

// ReSharper disable PossibleMultipleEnumeration

namespace NullDesk.Extensions.Mailer.MailKit.Tests
{
    public class MailKitMessageHistoryTests : IClassFixture<HistoryMailFixture>
    {
        public MailKitMessageHistoryTests(HistoryMailFixture fixture)
        {
            ReplacementVars.Add("%name%", "Mr. Toast");
            Fixture = fixture;
        }


        private HistoryMailFixture Fixture { get; }
        private Dictionary<string, string> ReplacementVars { get; } = new Dictionary<string, string>();


        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task MailKit__History_SerializedAttachments_ReSendMail(string html, string text,
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
        public async Task MailKit_History_NoSerializedAttachments_ReSendMail(string html, string text,
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

                await asyncFunction
                    .Should().ThrowAsync<InvalidOperationException>();
            }
            else
            {
                var di = await mailer.ReSendAsync(m.Id, CancellationToken.None);
                di.Should().BeOfType<DeliveryItem>().Which.IsSuccess.Should().BeTrue();
            }
        }

        [Fact]
        [Trait("TestType", "Unit")]
        public async Task MailKit_History_Search()
        {
            var mailer = Fixture.MailerFactoryForHistoryWithoutSerializableAttachments.GetMailer();
            var ids = mailer.AddMessages(new List<MailerMessage>
            {
                new MailerMessage()
                    .To("noone@nowhere.com")
                    .From("someone@somewhere.com", "Some One")
                    .WithSubject("Some Topic")
                    .WithBody<ContentBody>(b => b.PlainTextContent = "something"),
                new MailerMessage()
                    .To("someoneelse@nowhere.com")
                    .From("someone@somewhere.com", "New Guy")
                    .WithSubject("Some Topic")
                    .WithBody<ContentBody>(b => b.PlainTextContent = "something")
            });
            await mailer.SendAllAsync(CancellationToken.None);
            var history = Fixture.StoreWithoutSerializableAttachments;

            var searchA = await history.SearchAsync("else");
            searchA.Should().NotBeNull().And.OnlyContain(i => i.SourceApplicationName == "xunit");
            searchA.Should().NotBeNull().And.OnlyContain(i => i.ToEmailAddress == "someoneelse@nowhere.com");

            var searchB = await history.SearchAsync("else", sourceApplicationName: "noxunit");
            searchB.Should().BeEmpty();

            var searchC = await history.SearchAsync("else", startDate: DateTimeOffset.UtcNow.AddHours(-1),
                endDate: DateTimeOffset.UtcNow);
            searchC.Should().NotBeNull().And.OnlyContain(i => i.SourceApplicationName == "xunit");
            searchC.Should().NotBeNull().And.OnlyContain(i => i.ToEmailAddress == "someoneelse@nowhere.com");

            var searchD = await history.SearchAsync("else", startDate: DateTimeOffset.UtcNow.AddHours(-2),
                endDate: DateTimeOffset.UtcNow.AddHours(-1));
            searchD.Should().BeEmpty();
        }
    }
}