using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;

namespace NullDesk.Extensions.Mailer.Core.Fluent
{

    public partial class MessageBuilder //root
    {
        public class BuildFromStep : BuilderContext
        {
            internal BuildFromStep(MailerMessage context) : base(context) { }

            //TODO: ReplyTo

            public BuildFromWithDisplayStep WithDisplayName(string displayName)
                => new BuildFromWithDisplayStep(Context.From(Context.From.WithDisplayName(displayName)));

            public BuildSubjectStep And
                => new BuildSubjectStep(Context);

            public class BuildFromWithDisplayStep : BuilderContext
            {
                internal BuildFromWithDisplayStep(MailerMessage context) : base(context) { }

                public BuildSubjectStep And
                    => new BuildSubjectStep(Context);
            }
        }
    }
}
