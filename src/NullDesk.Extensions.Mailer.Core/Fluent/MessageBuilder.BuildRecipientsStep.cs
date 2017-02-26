using System.Linq;
using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;

namespace NullDesk.Extensions.Mailer.Core.Fluent
{

    public partial class MessageBuilder //root
    {
        public class BuildRecipientsStep : BuilderContext
        {
            internal BuildRecipientsStep(MailerMessage context) : base(context) { }

            public BuildToStep To(string emailAddress)
                => new BuildToStep(Context, emailAddress);

            public class BuildToStep : BuilderContext
            {
                private MailerRecipient Recipient { get; }

                internal BuildToStep(MailerMessage context, string recipientAddress) : base(context)
                {
                    Recipient = Context.Recipients.FirstOrDefault(r => r.EmailAddress == recipientAddress);
                    if (Recipient == null)
                    {
                        Recipient = new MailerRecipient() { EmailAddress = recipientAddress };
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
                    internal BuiltToWithDisplayStep(MailerMessage context, MailerRecipient recipient)
                        : base(context, recipient) { }

                    public BuildRecipientWithDisplaySubstitutionStep WithPersonalizedSubstitution(string replacementToken, string replacementValue)
                        => new BuildRecipientWithDisplaySubstitutionStep(Context, Recipient.WithSubstitution(replacementToken, replacementValue));

                    public BuildContentStep And
                        => new BuildContentStep(Context);
                }

                public class BuildRecipientSubstitutionStep : BuilderRecipientContext
                {
                    internal BuildRecipientSubstitutionStep(MailerMessage context, MailerRecipient recipient)
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
                    internal BuildRecipientWithDisplaySubstitutionStep(MailerMessage context, MailerRecipient recipient)
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
