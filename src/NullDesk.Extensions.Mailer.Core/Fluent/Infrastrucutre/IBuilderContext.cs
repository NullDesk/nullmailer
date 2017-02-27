namespace NullDesk.Extensions.Mailer.Core.Fluent
{
    public interface IBuilderContext
    {
        MailerMessage Message { get; set; }
    }
}