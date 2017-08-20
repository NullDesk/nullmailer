using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NullDesk.Extensions.Mailer.Core.Extensions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using NullDesk.Extensions.Mailer.Core.Tests.Infrastructure;
using NullDesk.Extensions.Mailer.Tests.Common;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class HistorySerializationTests : IClassFixture<HistorySerializationFixture>
    {
        public HistorySerializationTests(HistorySerializationFixture fixture)
        {
            ReplacementVars.Add("%name%", "Mr. Toast");
            Fixture = fixture;
        }

        private HistorySerializationFixture Fixture { get; }

        private Dictionary<string, string> ReplacementVars { get; } = new Dictionary<string, string>();

        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task SerializeDeserialize_History_WithContentBody(string html, string text, string[] attachments)
        {
            var history = Fixture.ServiceProvider.GetService<IHistoryStore>();
            var message = new MessageBuilder().From("xunit@test.com")
                .And
                .Subject($"xunit Test run: content body")
                .And.To("noone@toast.com")
                .WithDisplayName("No One Important")
                .And.ForBody()
                .WithHtml(html)
                .AndPlainText(text)
                .And.WithSubstitutions(ReplacementVars)
                .And.WithAttachments(attachments)
                .Build();
            var preDeliveryBody = message.Body;
            var preDeliveryAttachments = message
                .Attachments
                .Select(a => new KeyValuePair<string, string>(a.Key, a.Value.ToBase64StringAsync().Result))
                .ToDictionary(k => k.Key, k => k.Value);
            var deliveries = message.Recipients.Select(recipient => new DeliveryItem(message, recipient)).ToList();
            foreach (var item in deliveries)
            {
                var id = await history.AddAsync(item);
                var savedItem = await history.GetAsync(id);
                savedItem.SourceApplicationName.Should().Be("xunit");
                var postDeliveryAttachments = savedItem
                    .Attachments
                    .Select(a => new KeyValuePair<string, string>(a.Key, a.Value.ToBase64StringAsync().Result))
                    .ToDictionary(k => k.Key, k => k.Value);
                var postDeliveryBody = savedItem.Body;

                preDeliveryBody.Should().BeOfType(postDeliveryBody.GetType());

                foreach (var postAttach in postDeliveryAttachments)
                {
                    preDeliveryAttachments.FirstOrDefault(preAttach => preAttach.Key == postAttach.Key)
                        .Value
                        .Should()
                        .Be(postAttach.Value);
                }
            }
        }

        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(TemplateMailerTestData))]
        public async Task SerializeDeserialize_History_WithTemplateBody(string template, string[] attachments)
        {
            var history = Fixture.ServiceProvider.GetService<IHistoryStore>();
            var message = new MessageBuilder().From("xunit@test.com")
                .And
                .Subject($"xunit Test run: %template%")
                .And.To("noone@toast.com")
                .WithDisplayName("No One Important")
                .And.ForTemplate(template)
                .And.WithSubstitutions(ReplacementVars)
                .And.WithSubstitution("%template%", template)
                .And.WithAttachments(attachments)
                .Build();
            var preDeliveryBody = message.Body;
            var preDeliveryAttachments = message
                .Attachments
                .Select(a => new KeyValuePair<string, string>(a.Key, a.Value.ToBase64StringAsync().Result))
                .ToDictionary(k => k.Key, k => k.Value);
            var deliveries = message.Recipients.Select(recipient => new DeliveryItem(message, recipient)).ToList();
            foreach (var item in deliveries)
            {
                var id = await history.AddAsync(item);
                var savedItem = await history.GetAsync(id);
                var postDeliveryAttachments = savedItem
                    .Attachments
                    .Select(a => new KeyValuePair<string, string>(a.Key, a.Value.ToBase64StringAsync().Result))
                    .ToDictionary(k => k.Key, k => k.Value);
                var postDeliveryBody = savedItem.Body;

                preDeliveryBody.Should().BeOfType(postDeliveryBody.GetType());

                foreach (var postAttach in postDeliveryAttachments)
                {
                    preDeliveryAttachments.FirstOrDefault(preAttach => preAttach.Key == postAttach.Key)
                        .Value
                        .Should()
                        .Be(postAttach.Value);
                }
            }
        }
    }
}