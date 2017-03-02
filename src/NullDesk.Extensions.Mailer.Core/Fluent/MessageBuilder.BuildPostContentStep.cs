using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;

namespace NullDesk.Extensions.Mailer.Core.Fluent
{

    public partial class MessageBuilder //root
    {
        public class BuildPostContentStep : BuilderContext
        {
            public BuildPostContentStep(MailerMessage context) : base(context) { }

            public BuildAttachmentOrSubstitutionStep WithAttachment(string fileName)
                => new BuildAttachmentOrSubstitutionStep(Context.WithAttachment(fileName));

            public BuildAttachmentOrSubstitutionStep WithSubstitution(string replacementToken, string replacementValue)
                => new BuildAttachmentOrSubstitutionStep(Context.WithSubstitution(replacementToken, replacementValue));

            public MailerMessage Build() => Context;

            public class BuildAttachmentOrSubstitutionStep : BuilderContext
            {
                public BuildAttachmentOrSubstitutionStep(MailerMessage context) : base(context) { }

                public BuildPostContentStep And
                    => new BuildPostContentStep(Context);

                public MailerMessage Build()
                    => Context;
            }

         
        }
    }
}
