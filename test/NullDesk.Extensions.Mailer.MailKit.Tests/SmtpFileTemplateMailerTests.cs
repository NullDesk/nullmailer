using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace NullDesk.Extensions.Mailer.MailKit.Tests
{
    public class SmtpFileTemplateMailerTests
    {
        private Dictionary<string, string> ReplacementVars { get; } = new Dictionary<string, string>();

        public SmtpFileTemplateMailerTests()
        {
            ReplacementVars.Add("%name%", "Mr. Toast");
        }

        [Theory]
        [InlineData("template1", new string[] { })]
        [InlineData("template1", new[] { "./app_data/attachments/testFile.1.txt" })]
        [InlineData("template1", new[] { "./app_data/attachments/testFile.1.txt", "./app_data/attachments/testFile.2.txt" })]
        [InlineData("template2", new[] { "./app_data/attachments/testFile.1.txt" })]
        [Trait("TestType", "Unit")]
        public async Task SendMailWithTemplate(string templateName, string[] attachments)
        {

            var mailer = Program.ServiceProvider.GetService<SmtpFileTemplateMailer>();

            var result =
                await
                    mailer.SendMailAsync(
                        templateName,
                        "noone@toast.com",
                        "No One Important",
                        $"xunit Test run: {templateName}",
                        ReplacementVars,
                        attachments,
                        CancellationToken.None);

            result.Should().BeTrue();
        }
    }
}