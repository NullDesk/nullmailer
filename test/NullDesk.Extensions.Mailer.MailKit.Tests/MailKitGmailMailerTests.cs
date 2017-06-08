using NullDesk.Extensions.Mailer.MailKit.Tests.Infrastructure;
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
        // https://github.com/jstedfast/MailKit/blob/master/FAQ.md#GMailAccess

        //Setup user and password in fixture, then uncomment


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