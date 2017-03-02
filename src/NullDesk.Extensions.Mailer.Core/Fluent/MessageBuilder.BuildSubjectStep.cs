using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;

namespace NullDesk.Extensions.Mailer.Core.Fluent
{
    public partial class MessageBuilder
    {
        /// <summary>
        ///     Fluent message builder step for defining the message's subject.
        /// </summary>
        /// <seealso cref="NullDesk.Extensions.Mailer.Core.Fluent.BuilderContext" />
        public class BuildSubjectStep : BuilderContext
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="BuildSubjectStep" /> class.
            /// </summary>
            /// <param name="context">The context.</param>
            public BuildSubjectStep(MailerMessage context) : base(context)
            {
            }

            /// <summary>
            ///     Adds a subject to the message.
            /// </summary>
            /// <param name="subject">The subject.</param>
            /// <returns>BuildWithSubjectStep.</returns>
            public BuildWithSubjectStep Subject(string subject)
                => new BuildWithSubjectStep(Context.WithSubject(subject));

            /// <summary>
            ///     Specifies that this message should have an empty subject line.
            /// </summary>
            /// <returns>BuildWithSubjectStep.</returns>
            public BuildWithSubjectStep WithOutSubject()
                => new BuildWithSubjectStep(Context);

            /// <summary>
            ///     Fluent message builder step for defining the message's subject.
            /// </summary>
            /// <seealso cref="NullDesk.Extensions.Mailer.Core.Fluent.BuilderContext" />
            public class BuildWithSubjectStep : BuilderContext
            {
                /// <summary>
                ///     Initializes a new instance of the <see cref="BuildWithSubjectStep" /> class.
                /// </summary>
                /// <param name="context">The context.</param>
                public BuildWithSubjectStep(MailerMessage context) : base(context)
                {
                }

                /// <summary>
                ///     Advance to the next step in the fluent builder process
                /// </summary>
                /// <value>The and.</value>
                public BuildRecipientsStep And
                    => new BuildRecipientsStep(Context);
            }
        }
    }
}