//using System;
//using System.IO;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using NullDesk.Extensions.Mailer.MailKit.Tests.Infrastructure;
//using NullDesk.Extensions.Mailer.Tests.Common;
//using Xunit;
//using FluentAssertions;
//using NullDesk.Extensions.Mailer.Core;

//namespace NullDesk.Extensions.Mailer.MailKit.Tests
//{
//    public class MailKitMessageHistoryTests : IClassFixture<FactoryMailFixture>
//    {

//        private const string Subject = "xunit Test run: no template - resend";

//        private FactoryMailFixture Fixture { get; }

//        public MailKitMessageHistoryTests(FactoryMailFixture fixture)
//        {
//            Fixture = fixture;
//        }

//        [Theory]
//        [Trait("TestType", "Unit")]
//        [ClassData(typeof(StandardMailerTestData))]
//        public async Task ReSendMail(string html, string text, string[] attachments)
//        {
//            attachments = attachments?.Select(a => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, a))).ToArray();

//            var mailer = Fixture.Mail.SimpleMailer;
//            mailer.Should().BeOfType<MkSimpleSmtpMailer>();
//            mailer.Should().NotBeOfType<MkSmtpMailer>();
//            var result =
//                await
//                    mailer.SendMailAsync(
//                        "noone@toast.com",
//                        "No One Important",
//                        Subject,
//                        html,
//                        text,
//                        attachments,
//                        CancellationToken.None
//                    );
//            result.Should().BeOfType<MessageDeliveryItem>();
//            var history = await Fixture.Store.GetAsync(result.Id, CancellationToken.None);
//            var m = history.Should().NotBeNull().And.BeOfType<DeliveryItem>().Which;
//            m.IsSuccess.Should().BeTrue();
//            m.MessageData.Should().NotBeNullOrEmpty();
//        }
//    }
//}

