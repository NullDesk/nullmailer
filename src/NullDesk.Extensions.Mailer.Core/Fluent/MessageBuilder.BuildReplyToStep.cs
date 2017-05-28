using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;

namespace NullDesk.Extensions.Mailer.Core.Fluent
{
    public partial class MessageBuilder
    {
        /// <summary>
        ///     Fluent message builder step for defining the message's reply to address.
        /// </summary>
        /// <seealso cref="NullDesk.Extensions.Mailer.Core.Fluent.BuilderContext" />
        public class BuildReplyToStep : BuilderContext
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="BuildReplyToStep" /> class.
            /// </summary>
            /// <param name="context">The context.</param>
            public BuildReplyToStep(MailerMessage context) : base(context)
            {
            }

            /// <summary>
            ///     Advance to the next step in the fluent builder process
            /// </summary>
            /// <value>The and.</value>
            public BuildSubjectStep And
                => new BuildSubjectStep(Context);


            /// <summary>
            ///     Adds a display name for the sender
            /// </summary>
            /// <param name="displayName">The display name.</param>
            /// <returns>BuildFromWithDisplayStep.</returns>
            public BuildReplyToWithDisplayStep WithReplyToDisplayName(string displayName)
            {
                return new BuildReplyToWithDisplayStep(Context.From(Context.From.WithReplyToDisplayName(displayName)));
            }

            /// <summary>
            ///     Fluent message builder step for defining the message's reply to addres.
            /// </summary>
            /// <seealso cref="NullDesk.Extensions.Mailer.Core.Fluent.BuilderContext" />
            public class BuildReplyToWithDisplayStep : BuilderContext
            {
                /// <summary>
                ///     Initializes a new instance of the <see cref="BuildReplyToWithDisplayStep" /> class.
                /// </summary>
                /// <param name="context">The context.</param>
                public BuildReplyToWithDisplayStep(MailerMessage context) : base(context)
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