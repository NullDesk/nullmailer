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
    public class MkGmailMailerTests : IClassFixture<GmailMailFixture>
    {
        public MkGmailMailerTests(GmailMailFixture fixture)
        {
            Fixture = fixture;
        }

        private GmailMailFixture Fixture { get; }

        //Setup user and password in fixture, then uncomment
        // https://github.com/jstedfast/MailKit/blob/master/FAQ.md#GMailAccess



        //[Fact]
        //[Trait("TestType", "Integration")]
        //public async Task MailKit_Gmail_Basic_SendAll()
        //{
           
            //var mailer = Fixture.BasicAuthServiceProvider.GetService<IMailer>();

            //var deliveryItems =
            //    mailer.CreateMessage(b => b
            //        .Subject($"xunit Test run: content body")
            //        .And.To("abc@xyz.net")
            //        .WithDisplayName("No One Important")
            //        .And.ForBody()
            //        .WithPlainText("nothing to see here")
            //        .Build());

            //var result = await mailer.SendAllAsync(CancellationToken.None);

            //result
            //    .Should()
            //    .NotBeNull()
            //    .And.AllBeOfType<DeliveryItem>()
            //    .And.HaveSameCount(deliveryItems)
            //    .And.OnlyContain(i => i.IsSuccess);
        //}

        //[Fact]
        //[Trait("TestType", "Integration")]
        //public async Task MailKit_Gmail_Token_SendAll()
        //{

            //var mailer = Fixture.TokenAuthServiceProvider.GetService<IMailer>();

            //var deliveryItems =
            //    mailer.CreateMessage(b => b
            //        .Subject($"xunit Test run: content body")
            //        .And.To("abc@xyz.net")
            //        .WithDisplayName("No One Important")
            //        .And.ForBody()
            //        .WithPlainText("nothing to see here")
            //        .Build());

            //var result = await mailer.SendAllAsync(CancellationToken.None);

            //result
            //    .Should()
            //    .NotBeNull()
            //    .And.AllBeOfType<DeliveryItem>()
            //    .And.HaveSameCount(deliveryItems)
            //    .And.OnlyContain(i => i.IsSuccess);
        //}

    }
}