using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;

namespace NullDesk.Extensions.Mailer.Core.Fluent
{

    public partial class MessageBuilder //root
    {
        public class BuildPostContentStep : BuilderContext
        {
            internal BuildPostContentStep(MailerMessage context) : base(context) { }

            public BuildAttachmentStep WithAttachment(string fileName)
                => new BuildAttachmentStep(Context.WithAttachment(fileName));

            public BuildSubstitutionStep WithSubstitution(string replacementToken, string replacementValue)
                => new BuildSubstitutionStep(Context.WithSubstitution(replacementToken, replacementValue));

            public MailerMessage Build() => new MailerMessage();

            public class BuildAttachmentStep : BuilderContext
            {
                internal BuildAttachmentStep(MailerMessage context) : base(context) { }

                public BuildPostContentStep And
                    => new BuildPostContentStep(Context);

                public MailerMessage Build()
                    => new MailerMessage();
            }

            public class BuildSubstitutionStep : BuilderContext
            {
                public BuildSubstitutionStep(MailerMessage context) : base(context) { }

                public BuildPostContentStep And
                   => new BuildPostContentStep(Context);

                public MailerMessage Build()
                    => new MailerMessage();
            }
        }
    }
}
