using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.MailKit.Tests.Infrastructure;
using NullDesk.Extensions.Mailer.Tests.Common;

namespace NullDesk.Extensions.Mailer.MailKit.Tests
{
    public class MailKitSmtpParallelMailerTests : IClassFixture<ReusableMailFixture>
    {
        private ReusableMailFixture Fixture { get; }

        public MailKitSmtpParallelMailerTests(ReusableMailFixture fixture)
        {
            Fixture = fixture;
        }

        [Theory]
        [Trait("TestType", "Integration")]
        [ClassData(typeof(StandardMailerTestData))]
        public void SendParallelMail(string html, string text, string[] attachments)
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
                result.Should().BeOfType<MessageDeliveryItem>().Which.IsSuccess.Should().BeTrue();
                var m = result.Should().NotBeNull().And.BeOfType<MessageDeliveryItem>().Which;
                m.IsSuccess.Should().BeTrue();
                m.MessageData.Should().NotBeNullOrEmpty();
            });
        }
    }

   

}