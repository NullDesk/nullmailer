using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace NullDesk.Extensions.Mailer.MailKit.Tests
{
    public class SmtpMailerTests
    {
        private const string HtmlBody = "<!doctypehtml><htmllang=\"en\"><head><metacharset=\"utf-8\"><title>TestMessage</title></head><body><p>Hello,</p><p>This is a test html message from xUnit.</p><p>Thanks,</p><p>Bot</p></body></html>";

        private const string TextBody = "Hello,\n\nThis is a test plain text message from xUnit.\n\nThanks,\n\nBot\n\n";

        [Theory]
        [InlineData(HtmlBody, TextBody, new string[] { })]
        [InlineData(HtmlBody, TextBody, new[] { "./app_data/attachments/testFile.1.txt" })]
        [InlineData(HtmlBody, TextBody, new[] { "./app_data/attachments/testFile.1.txt", "./app_data/attachments/testFile.2.txt" })]
        [InlineData(null, TextBody, new[] { "./app_data/attachments/testFile.1.txt" })]
        [InlineData(HtmlBody, null, null)]
        [Trait("TestType", "Unit")]
        public async Task SendMail(string html, string text, string[] attachments)
        {
            var mailer = Program.ServiceProvider.GetService<SmtpMailer>();
            var result =
                await
                    mailer.SendMailAsync(
                        "noone@toast.com",
                        "No One Important",
                        "xunit Test run: no template",
                        html,
                        text,
                        attachments,
                        CancellationToken.None
                    );
            result.Should().BeTrue();
        }
    }
}