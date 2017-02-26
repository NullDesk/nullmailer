using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;

namespace NullDesk.Extensions.Mailer.Core.Fluent
{
    public partial class MessageBuilder //root
    {
        private MailerMessage Context { get; } = new MailerMessage();

        public BuildFromStep From(string emailAddress)
            => new BuildFromStep(Context.From(emailAddress));

        //Step 1 = MessageBuilder.BuildFromStep.cs

        //Step 2 = MessageBuilder.BuildSubjectStep.cs

        //Step 3 = MessageBuilder.BuildRecipientsStep.cs

        //Step 4 = MessageBuilder.BuildContentStep.cs

        //Step 5 = MessageBuilder.BuildPostContentStep

    }
}
