using System.Collections.Generic;
using System.IO;
using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;

namespace NullDesk.Extensions.Mailer.Core.Fluent
{
    public partial class MessageBuilder
    {
        /// <summary>
        ///     Fluent message builder step for defining optional message features such as attachments and content substitutions.
        /// </summary>
        /// <seealso cref="NullDesk.Extensions.Mailer.Core.Fluent.BuilderContext" />
        public class BuildPostContentStep : BuilderContext, IBuilderStepsCompleted
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="BuildPostContentStep" /> class.
            /// </summary>
            /// <param name="context">The context.</param>
            public BuildPostContentStep(MailerMessage context) : base(context)
            {
            }

            /// <summary>
            ///     Completes the fluent builder and returns the mailer message instance.
            /// </summary>
            /// <returns>MailerMessage.</returns>
            public MailerMessage Build()
            {
                return Context;
            }

            /// <summary>
            ///     Adds a collection of attachments to the message.
            /// </summary>
            /// <param name="attachments">The attachments.</param>
            /// <returns>BuildAttachmentOrSubstitutionStep.</returns>
            public BuildAttachmentOrSubstitutionStep WithAttachments(IDictionary<string, Stream> attachments)
            {
                return new BuildAttachmentOrSubstitutionStep(Context.WithAttachments(attachments));
            }

            /// <summary>
            ///     Adds a collection of attachments to the message.
            /// </summary>
            /// <param name="attachments">The attachments.</param>
            /// <returns>BuildAttachmentOrSubstitutionStep.</returns>
            public BuildAttachmentOrSubstitutionStep WithAttachments(IEnumerable<string> attachments)
            {
                return new BuildAttachmentOrSubstitutionStep(Context.WithAttachments(attachments));
            }


            /// <summary>
            ///     Adds an attachment to the message.
            /// </summary>
            /// <param name="fileName">Name of the file.</param>
            /// <param name="stream">The stream.</param>
            /// <returns>BuildAttachmentOrSubstitutionStep.</returns>
            public BuildAttachmentOrSubstitutionStep WithAttachment(string fileName, Stream stream)
            {
                return new BuildAttachmentOrSubstitutionStep(Context.WithAttachment(fileName, stream));
            }


            /// <summary>
            ///     Adds an attachment to the message.
            /// </summary>
            /// <param name="fileName">Name of the file.</param>
            /// <returns>BuildAttachmentOrSubstitutionStep.</returns>
            public BuildAttachmentOrSubstitutionStep WithAttachment(string fileName)
            {
                return new BuildAttachmentOrSubstitutionStep(Context.WithAttachment(fileName));
            }

            /// <summary>
            ///     Adds a collection of replacement tokens and values to the collection of substutions.
            /// </summary>
            /// <param name="substitutions">The substitutions.</param>
            /// <returns>BuildAttachmentOrSubstitutionStep.</returns>
            public BuildAttachmentOrSubstitutionStep WithSubstitutions(IDictionary<string, string> substitutions)
            {
                return new BuildAttachmentOrSubstitutionStep(Context.WithSubstitutions(substitutions));
            }


            /// <summary>
            ///     Adds a replacement token and value to the collection of substutions.
            /// </summary>
            /// <param name="replacementToken">The replacement token.</param>
            /// <param name="replacementValue">The replacement value.</param>
            /// <returns>BuildAttachmentOrSubstitutionStep.</returns>
            public BuildAttachmentOrSubstitutionStep WithSubstitution(string replacementToken, string replacementValue)
            {
                return new BuildAttachmentOrSubstitutionStep(
                    Context.WithSubstitution(replacementToken, replacementValue));
            }


            /// <summary>
            ///     Fluent message builder step for defining optional message features such as attachments and content substitutions.
            /// </summary>
            /// <seealso cref="NullDesk.Extensions.Mailer.Core.Fluent.BuilderContext" />
            public class BuildAttachmentOrSubstitutionStep : BuilderContext, IBuilderStepsCompleted
            {
                /// <summary>
                ///     Initializes a new instance of the <see cref="BuildAttachmentOrSubstitutionStep" /> class.
                /// </summary>
                /// <param name="context">The context.</param>
                public BuildAttachmentOrSubstitutionStep(MailerMessage context) : base(context)
                {
                }

                /// <summary>
                ///     Advance to the next step in the fluent builder process
                /// </summary>
                /// <value>The and.</value>
                public BuildPostContentStep And
                    => new BuildPostContentStep(Context);

                /// <summary>
                ///     Completes the fluent builder and returns the mailer message instance.
                /// </summary>
                /// <returns>MailerMessage.</returns>
                public MailerMessage Build()
                {
                    return Context;
                }
            }
        }
    }
}