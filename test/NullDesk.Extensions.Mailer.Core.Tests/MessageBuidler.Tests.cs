using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class MessageBuidlerTests
    {
        [Theory]
        [InlineData("toast@toast.com")]
        [InlineData("")]
        [InlineData(null)]
        public void MessageBuilder_From(string address)
        {
            var messageBuilder = new MessageBuilder();
            var stepBuilder = messageBuilder.From(address);

            stepBuilder
                .Should().NotBeNull()
                .And.BeOfType<MessageBuilder.BuildFromStep>()
                .Which.As<IBuilderContext>().Message.From
                    .Should().NotBeNull()
                    .And.BeOfType<MessageSender>().Which
                    .EmailAddress
                    .Should().Be(address).And
                    .BeEquivalentTo(
                        messageBuilder.As<IBuilderContext>()?
                        .Message?
                        .From?
                        .EmailAddress);
            
            ////current
            //mailer.CreateMessage()
            //    .From(f => f.FromAddress("toast@toast.com").WithDisplayName("toastman"))
            //    .To(t => t.ToAddress("junk@junk.com").WithDisplayName("junkman").WithSubstitution("x", "y"))
            //    .To(
            //        t =>
            //            t.ToAddress("junk@junk.com")
            //                .WithDisplayName("junkman")
            //                .WithSubstitution("x", "y")
            //                .WithSubstitution("a", "b"))
            //    .WithSubject("subject")
            //    .WithBody<ContentBody>(b => b.HtmlContent = "some html");

            //var builder = new MessageBuilder();
            //builder
            //    .From("toast@toastg.com")
            //    .And.WithOutSubject()
            //    .And.To("junk@junk.com")
            //    .And.ForTemplate("template1")
            //    .Build();

            //builder = new MessageBuilder();
            //builder
            //    .From("toast@toast.com")
            //        .WithDisplayName("toatman")
            //    .And.Subject("subject")
            //    .And.To("yes@no.com")
            //    .And.To("me@me.com")
            //        .WithPersonalizedSubstitution("x","z")
            //    .And.To("toast@toast.com")
            //        .WithDisplayName("toastman")
            //        .WithPersonalizedSubstitution("x", "z")
            //    .And.To("junk@junk.com")
            //        .WithPersonalizedSubstitution("x", "z")
            //        .WithDisplayName("junkman")
            //    .And.ForBody()
            //        .WithHtml("html content here")
            //        .AndPlainText("text content")
            //    .And.WithSubstitution("x", "y")
            //    .And.WithSubstitution("a", "b")
            //    .And.WithAttachment("file1")
            //    .And.WithAttachment("file2")
            //    .Build();
        }
    }
}
