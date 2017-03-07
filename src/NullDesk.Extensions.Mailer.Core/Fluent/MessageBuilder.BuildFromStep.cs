using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;

namespace NullDesk.Extensions.Mailer.Core.Fluent
{
    public partial class MessageBuilder
    {
        /// <summary>
        ///     Fluent message builder step for defining the message's sender.
        /// </summary>
        /// <seealso cref="NullDesk.Extensions.Mailer.Core.Fluent.BuilderContext" />
        public class BuildFromStep : BuilderContext
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="BuildFromStep" /> class.
            /// </summary>
            /// <param name="context">The context.</param>
            public BuildFromStep(MailerMessage context) : base(context)
            {
            }

            /// <summary>
            ///     Advance to the next step in the fluent builder process
            /// </summary>
            /// <value>The and.</value>
            public BuildSubjectStep And
                => new BuildSubjectStep(Context);

            //TODO: ReplyTo

            /// <summary>
            ///     Adds a display name for the sender
            /// </summary>
            /// <param name="displayName">The display name.</param>
            /// <returns>BuildFromWithDisplayStep.</returns>
            public BuildFromWithDisplayStep WithDisplayName(string displayName)
                => new BuildFromWithDisplayStep(Context.From(Context.From.WithDisplayName(displayName)));

            /// <summary>
            ///     Fluent message builder step for defining the message's sender.
            /// </summary>
            /// <seealso cref="NullDesk.Extensions.Mailer.Core.Fluent.BuilderContext" />
            public class BuildFromWithDisplayStep : BuilderContext
            {
                /// <summary>
                ///     Initializes a new instance of the <see cref="BuildFromWithDisplayStep" /> class.
                /// </summary>
                /// <param name="context">The context.</param>
                public BuildFromWithDisplayStep(MailerMessage context) : base(context)
                {
                }

                /// <summary>
                ///     Advance to the next step in the fluent builder process
                /// </summary>
                /// <value>The and.</value>
                public BuildSubjectStep And
                    => new BuildSubjectStep(Context);
            }
        }
    }
}