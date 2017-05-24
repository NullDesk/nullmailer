using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.MailKit.Tests.Infrastructure;
using NullDesk.Extensions.Mailer.Tests.Common;
using Xunit;

namespace NullDesk.Extensions.Mailer.MailKit.Tests
{
    public class MkSmtpMailerFailureTests : IClassFixture<FailureMailFixture>
    {
        public MkSmtpMailerFailureTests(FailureMailFixture fixture)
        {
            ReplacementVars.Add("%name%", "Mr. Toast");
            Fixture = fixture;
        }

        private FailureMailFixture Fixture { get; }

        private Dictionary<string, string> ReplacementVars { get; } = new Dictionary<string, string>();

        [Fact]
        [Trait("TestType", "Integration")]
        public async Task MailKit_SendAll_WithInvalidSettings()
        {
            //this test only matters if using a live service
            if (Fixture.IsMailServiceAlive)
            {

                var mailer = Fixture.ServiceProvider.GetService<IMailer>();

                var deliveryItems = mailer.CreateMessage(b => b
                    .Subject($"xunit Test run: content body")
                    .And.To("noone@toast.com")
                    .WithDisplayName("No One Important")
                    .And.ForBody()
                    .WithPlainText("nothing to see here")
                    .Build());
                var result = await mailer.SendAllAsync(CancellationToken.None);
                result
                    .Should()
                    .NotBeNull()
                    .And.AllBeOfType<DeliveryItem>()
                    .And.HaveSameCount(deliveryItems)
                    .And.OnlyContain(i => !i.IsSuccess);
                //Func<Task> send  = async () => { await mailer.SendAllAsync(CancellationToken.None); };

                //send.ShouldThrow<Exception>();

                ((MkSmtpMailer) mailer).Settings.SmtpServer = "127.0.0.1"; //FIXED!
                var deliveryItemsFixed =
                    mailer.CreateMessage(b => b
                        .Subject($"xunit Test run: content body")
                        .And.To("noone@toast.com")
                        .WithDisplayName("No One Important")
                        .And.ForBody()
                        .WithPlainText("nothing to see here")
                        .Build());
                var resultFixed = await mailer.SendAllAsync(CancellationToken.None);
                resultFixed
                    .Should()
                    .NotBeNull()
                    .And.AllBeOfType<DeliveryItem>()
                    .And.HaveSameCount(deliveryItemsFixed)
                    .And.OnlyContain(i => i.IsSuccess);
            }
            
        }

        [Fact]
        [Trait("TestType", "Integration")]
        public async Task MailKit_SendAll_WithInvalidRecipient()
        {
            //this test only matters if using a live service
            if (Fixture.IsMailServiceAlive)
            {
                var mailer = Fixture.ServiceProvider.GetService<IMailer>();
                ((MkSmtpMailer) mailer).Settings.SmtpServer = "127.0.0.1"; //Start FIXED settings!
                var deliveryItems = mailer.CreateMessage(b => b
                    .Subject($"xunit Test run: content body")
                    .And.To("noonex")
                    .WithDisplayName("No One Important")
                    .And.ForBody()
                    .WithPlainText("nothing to see here")
                    .Build());
                var result = await mailer.SendAllAsync(CancellationToken.None);
                result
                    .Should()
                    .NotBeNull()
                    .And.AllBeOfType<DeliveryItem>()
                    .And.HaveSameCount(deliveryItems)
                    .And.OnlyContain(i => !i.IsSuccess);
                //Func<Task> send  = async () => { await mailer.SendAllAsync(CancellationToken.None); };

                //send.ShouldThrow<Exception>();


                var deliveryItemsFixed =
                    mailer.CreateMessage(b => b
                        .Subject($"xunit Test run: content body")
                        .And.To("noone@toast.com")
                        .WithDisplayName("No One Important")
                        .And.ForBody()
                        .WithPlainText("nothing to see here")
                        .Build());
                var resultFixed = await mailer.SendAllAsync(CancellationToken.None);
                resultFixed
                    .Should()
                    .NotBeNull()
                    .And.AllBeOfType<DeliveryItem>()
                    .And.HaveSameCount(deliveryItemsFixed)
                    .And.OnlyContain(i => i.IsSuccess);
            }
        }

    }
}