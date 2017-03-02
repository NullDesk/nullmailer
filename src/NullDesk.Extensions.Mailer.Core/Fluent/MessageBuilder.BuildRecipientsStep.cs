using System.Linq;
using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;

namespace NullDesk.Extensions.Mailer.Core.Fluent
{

    public partial class MessageBuilder //root
    {
        public class BuildRecipientsStep : BuilderContext
        {
            public BuildRecipientsStep(MailerMessage context) : base(context) { }

            public BuildToStep To(string emailAddress)
                => new BuildToStep(Context, emailAddress);

            public class BuildToStep : BuilderContext
            {
                private MessageRecipient Recipient { get; }

                public BuildToStep(MailerMessage context, string recipientAddress) : base(context)
                {
                    Recipient = Context.Recipients.FirstOrDefault(r => r.EmailAddress == recipientAddress);
                    if (Recipient == null)
                    {
                        Recipient = new MessageRecipient() { EmailAddress = recipientAddress };
                        context.To(Recipient);
                    }
                }

                public BuiltToWithDisplayStep WithDisplayName(string displayName)
                    => new BuiltToWithDisplayStep(Context, Recipient.WithDisplayName(displayName));


                public BuildRecipientSubstitutionStep WithPersonalizedSubstitution(string replacementToken, string replacementValue)
                    => new BuildRecipientSubstitutionStep(Context, Recipient.WithSubstitution(replacementToken, replacementValue));

                public BuildContentStep And
                    => new BuildContentStep(Context);

                public class BuiltToWithDisplayStep : BuilderRecipientContext
                {
                    public BuiltToWithDisplayStep(MailerMessage context, MessageRecipient recipient)
                        : base(context, recipient) { }

                    public BuildRecipientWithDisplaySubstitutionStep WithPersonalizedSubstitution(string replacementToken, string replacementValue)
                        => new BuildRecipientWithDisplaySubstitutionStep(Context, Recipient.WithSubstitution(replacementToken, replacementValue));

                    public BuildContentStep And
                        => new BuildContentStep(Context);
                }

                public class BuildRecipientSubstitutionStep : BuilderRecipientContext
                {
                    public BuildRecipientSubstitutionStep(MailerMessage context, MessageRecipient recipient)
                        : base(context, recipient) { }

                    public BuiltToWithDisplayStep WithDisplayName(string displayName)
                        => new BuiltToWithDisplayStep(Context, Recipient.WithDisplayName(displayName));

                    public BuildRecipientSubstitutionStep WithPersonalizedSubstitution(string replacementToken, string replacementValue)
                        => new BuildRecipientSubstitutionStep(Context, Recipient.WithSubstitution(replacementToken, replacementValue));

                    public BuildContentStep And
                        => new BuildContentStep(Context);
                }

                public class BuildRecipientWithDisplaySubstitutionStep : BuilderRecipientContext
                {
                    public BuildRecipientWithDisplaySubstitutionStep(MailerMessage context, MessageRecipient recipient)
                        : base(context, recipient) { }

                    public BuildRecipientWithDisplaySubstitutionStep WithPersonalizedSubstitution(string replacementToken, string replacementValue)
                        => new BuildRecipientWithDisplaySubstitutionStep(Context, Recipient.WithSubstitution(replacementToken, replacementValue));

                    public BuildContentStep And
                        => new BuildContentStep(Context);
                }
            }
        }
    }
}
