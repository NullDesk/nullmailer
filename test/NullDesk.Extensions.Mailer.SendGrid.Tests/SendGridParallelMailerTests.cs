using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.SendGrid.Tests.Infrastructure;
using NullDesk.Extensions.Mailer.Tests.Common;
using Microsoft.Extensions.DependencyInjection;

namespace NullDesk.Extensions.Mailer.SendGrid.Tests
{
    public class SendGridParallelMailerTests : IClassFixture<ReusableMailFixture>
    {
        private ReusableMailFixture Fixture { get; }

        public SendGridParallelMailerTests(ReusableMailFixture fixture)
        {
            Fixture = fixture;
        }


        [Theory]
        [Trait("TestType", "Integration")]
        [ClassData(typeof(StandardMailerTestData))]
        public void SendMail(string html, string text, string[] attachments)
        {
            var mailer = Fixture.ServiceProvider.GetService<ISimpleMailer>();
            var messages = new List<TestParallelMailMessage>();
            for (var i = 0; i < 10; i++)
            {
                messages.Add(new TestParallelMailMessage
                {
                    To = $"noone{i}@toast.com",
                    ToDisplay = "No One Important",
                    Subject = $"#{i} - Parallel xunit Test run: no template",
                    Html = html,
                    Text = text
                });
            }
            Parallel.ForEach(messages, (mes) =>
            {
                var result =
                   mailer.SendMailAsync(
                            mes.To,
                            mes.ToDisplay,
                            mes.Subject,
                            mes.Html,
                            mes.Text,
                            (IEnumerable<string>)null,
                            CancellationToken.None
                        ).Result;

                var m = result.Should().NotBeNull().And.BeOfType<MessageDeliveryItem>().Which;
                m.IsSuccess.Should().BeTrue();
                m.MessageData.Should().NotBeNullOrEmpty();
            });
        }

    }
}
