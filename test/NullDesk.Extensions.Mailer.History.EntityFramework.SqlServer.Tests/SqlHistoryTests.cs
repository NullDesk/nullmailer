﻿using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.Core.Fluent;
using NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Tests.Infrastructure;
using NullDesk.Extensions.Mailer.Tests.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

// ReSharper disable PossibleMultipleEnumeration
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Tests
{
    public class SqlHistoryTests : IClassFixture<SqlIntegrationFixture>
    {
        public SqlHistoryTests(SqlIntegrationFixture fixture)
        {
            ReplacementVars.Add("%name%", "Mr. Toast");

            Fixture = fixture;
        }

        private Dictionary<string, string> ReplacementVars { get; } = new Dictionary<string, string>();

        private const string Subject = "xunit Test run: no template - history";

        private SqlIntegrationFixture Fixture { get; }

        [Theory]
        [Trait("TestType", "Integration")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task Ef_Sql_History_AddWithCustomSourceApplicationName(string html, string text, string[] attachments)
        {
            var store = Fixture.ServiceProvider.GetService<IHistoryStore>();

            var message = new MessageBuilder().From("xunit@test.com")
                .And
                .Subject($"xunit Test run: content body")
                .And.To("noone@toast.com")
                .WithDisplayName("No One Important")
                .And.ForBody()
                .WithHtml(html)
                .AndPlainText(text)
                .And.WithSubstitutions(ReplacementVars)
                .Build();

            var deliveries = message.Recipients.Select(recipient => new DeliveryItem(message, recipient)).ToList();
            foreach (var item in deliveries)
            {
                item.SourceApplicationName = "testUpdatedValue";
                var id = await store.AddAsync(item);
                var savedItem = await store.GetAsync(id);
                savedItem.SourceApplicationName.Should().Be("testUpdatedValue");
                var body = savedItem.Body.Should().BeOfType<ContentBody>();
                body.Which.HtmlContent.Should().Be(html);
                body.Which.PlainTextContent.Should().Be(text);
            }
        }

        [Theory]
        [Trait("TestType", "Integration")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task Ef_Sql_History_SerializeAttachments_SendMail(string html, string text, string[] attachments)
        {
            attachments = attachments?.Select(a => Path.Combine(AppContext.BaseDirectory, a)).ToArray();

            var mailer = Fixture.ServiceProvider.GetService<IMailer>();
            ((EntityHistoryStore<TestSqlHistoryContext>)((Mailer<NullMailerSettings>)mailer).HistoryStore).Settings
                .StoreAttachmentContents = true;
            var deliveryItems =
                mailer.CreateMessage(b => b
                    .Subject(Subject)
                    .And.To("noone@toast.com")
                    .WithDisplayName("No One Important")
                    .And.ForBody()
                    .WithHtml(html)
                    .AndPlainText(text)
                    .And.WithSubstitutions(ReplacementVars)
                    .And.WithAttachments(attachments)
                    .Build());

            var result = await mailer.SendAllAsync(CancellationToken.None);
            result
                .Should()
                .NotBeNull()
                .And.AllBeOfType<DeliveryItem>()
                .And.HaveSameCount(deliveryItems)
                .And.OnlyContain(i => i.IsSuccess);

            var store = Fixture.ServiceProvider.GetService<IHistoryStore>();

            store.Should().BeOfType<EntityHistoryStore<TestSqlHistoryContext>>();

            var item = await store.GetAsync(result.First().Id, CancellationToken.None);
            item.SourceApplicationName.Should().Be("xunit");

            var m = item
                .Should()
                .NotBeNull()
                .And.BeOfType<DeliveryItem>();

            m.Which.Body.Should().NotBeNull();
            m.Which.Subject.Should().Be(Subject);
            m.Which.Attachments.Any(a => a.Value.Length == 0).Should().BeFalse();
        }

        [Theory]
        [Trait("TestType", "Integration")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task Ef_Sql_History_NoSerializeAttachments_SendMail(string html, string text, string[] attachments)
        {
            attachments = attachments?.Select(a => Path.Combine(AppContext.BaseDirectory, a)).ToArray();

            var mailer = Fixture.ServiceProvider.GetService<IMailer>();
            ((EntityHistoryStore<TestSqlHistoryContext>)((Mailer<NullMailerSettings>)mailer).HistoryStore).Settings
                .StoreAttachmentContents = false;
            var deliveryItems =
                mailer.CreateMessage(b => b
                    .Subject(Subject)
                    .And.To("noone@toast.com")
                    .WithDisplayName("No One Important")
                    .And.ForBody()
                    .WithHtml(html)
                    .AndPlainText(text)
                    .And.WithSubstitutions(ReplacementVars)
                    .And.WithAttachments(attachments)
                    .Build());

            var result = await mailer.SendAllAsync(CancellationToken.None);
            result
                .Should()
                .NotBeNull()
                .And.AllBeOfType<DeliveryItem>()
                .And.HaveSameCount(deliveryItems)
                .And.OnlyContain(i => i.IsSuccess);

            var store = Fixture.ServiceProvider.GetService<IHistoryStore>();

            store.Should().BeOfType<EntityHistoryStore<TestSqlHistoryContext>>();

            var item = await store.GetAsync(result.First().Id, CancellationToken.None);

            var m = item
                .Should()
                .NotBeNull()
                .And.BeOfType<DeliveryItem>();

            m.Which.Body.Should().NotBeNull();
            m.Which.Subject.Should().Be(Subject);
            m.Which.Attachments.All(a => a.Value == null).Should().BeTrue();
        }

        [Fact]
        [Trait("TestType", "Integration")]
        public async Task Ef_Sql_History_List()
        {
            var store = Fixture.ServiceProvider.GetService<IHistoryStore>();

            store.Should().BeOfType<EntityHistoryStore<TestSqlHistoryContext>>();

            for (var x = 0; x < 15; x++)
            {
                await store.AddAsync(new DeliveryItem(MailerMessage.Create(), new MessageRecipient())
                {
                    CreatedDate = DateTimeOffset.Now,
                    ToDisplayName = x.ToString(),
                    Subject = x.ToString(),
                    ToEmailAddress = $"{x}@toast.com",
                    DeliveryProvider = "xunit",
                    Id = Guid.NewGuid(),
                    IsSuccess = true,
                    ExceptionMessage = null,
                    Body = new ContentBody { PlainTextContent = "content" },
                    FromDisplayName = "noone",
                    FromEmailAddress = "noone@nowhere.com"
                });
            }

            var items = await store.GetAsync(0, 10, CancellationToken.None);

            items.Should()
                .HaveCount(10)
                .And.AllBeOfType<DeliverySummary>()
                .And.OnlyContain(m => m.SourceApplicationName == "xunit");

            var secondPageitems = await store.GetAsync(10, 5, CancellationToken.None);

            secondPageitems.Should().HaveCount(5).And.NotBeSameAs(items);
        }

        [Fact]
        [Trait("TestType", "Integration")]
        public async Task Ef_Sql_History_Search()
        {
            var history = Fixture.ServiceProvider.GetService<IHistoryStore>();
            await history.AddAsync(new DeliveryItem(MailerMessage.Create(), new MessageRecipient())
            {
                CreatedDate = DateTimeOffset.Now,
                ToDisplayName = "No One",
                Subject = "test message",
                ToEmailAddress = "noone@toast.com",
                DeliveryProvider = "xunit",
                SourceApplicationName = "xunit",
                Id = Guid.NewGuid(),
                IsSuccess = true,
                ExceptionMessage = null,
                Body = new ContentBody { PlainTextContent = "content" },
                FromDisplayName = "noone",
                FromEmailAddress = "noone@nowhere.com"
            });

            await history.AddAsync(new DeliveryItem(MailerMessage.Create(), new MessageRecipient())
            {
                CreatedDate = DateTimeOffset.Now,
                ToDisplayName = "Some One",
                Subject = "test message",
                ToEmailAddress = "someoneelse@toast.com",
                DeliveryProvider = "xunit",
                SourceApplicationName = "xunit",
                Id = Guid.NewGuid(),
                IsSuccess = true,
                ExceptionMessage = null,
                Body = new ContentBody { PlainTextContent = "content" },
                FromDisplayName = "noone",
                FromEmailAddress = "noone@nowhere.com"
            });

            var searchA = await history.SearchAsync("else");
            searchA.Should().NotBeNull().And.OnlyContain(i => i.SourceApplicationName == "xunit");
            searchA.Should().NotBeNull().And.OnlyContain(i => i.ToEmailAddress == "someoneelse@toast.com");

            var searchB = await history.SearchAsync("else", sourceApplicationName: "noxunit");
            searchB.Should().BeEmpty();

            var searchC = await history.SearchAsync("else", startDate: DateTimeOffset.UtcNow.AddHours(-1),
                endDate: DateTimeOffset.UtcNow);
            searchC.Should().NotBeNull().And.OnlyContain(i => i.SourceApplicationName == "xunit");
            searchC.Should().NotBeNull().And.OnlyContain(i => i.ToEmailAddress == "someoneelse@toast.com");

            var searchD = await history.SearchAsync("else", startDate: DateTimeOffset.UtcNow.AddHours(-2),
                endDate: DateTimeOffset.UtcNow.AddHours(-1));
            searchD.Should().BeEmpty();
        }
    }
}
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
