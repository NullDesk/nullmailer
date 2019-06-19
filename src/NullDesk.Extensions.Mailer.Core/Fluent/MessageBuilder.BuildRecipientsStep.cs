using System.Collections.Generic;
using System.Linq;
using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;

namespace NullDesk.Extensions.Mailer.Core.Fluent
{
    public partial class MessageBuilder
    {
        /// <summary>
        ///     Fluent message builder step for defining one or more message recipients.
        /// </summary>
        /// <seealso cref="BuilderContext" />
        public class BuildRecipientsStep : BuilderContext
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="BuildRecipientsStep" /> class.
            /// </summary>
            /// <param name="context">The context.</param>
            public BuildRecipientsStep(MailerMessage context) : base(context)
            {
            }

            /// <summary>
            ///     Adds a recipient with the specified email address.
            /// </summary>
            /// <param name="emailAddress">The email address.</param>
            /// <returns>BuildToStep.</returns>
            public BuildToStep To(string emailAddress)
            {
                return new BuildToStep(Context, emailAddress);
            }

            /// <summary>
            ///     Fluent message builder step for defining one or more message recipients.
            /// </summary>
            /// <seealso cref="NullDesk.Extensions.Mailer.Core.Fluent.BuilderContext" />
            public class BuildToStep : BuilderContext
            {
                /// <summary>
                ///     Initializes a new instance of the <see cref="BuildToStep" /> class.
                /// </summary>
                /// <param name="context">The context.</param>
                /// <param name="recipientAddress">The recipient address.</param>
                public BuildToStep(MailerMessage context, string recipientAddress) : base(context)
                {
                    Recipient = Context.Recipients.FirstOrDefault(r => r.EmailAddress == recipientAddress);
                    if (Recipient == null)
                    {
                        Recipient = new MessageRecipient {EmailAddress = recipientAddress};
                        context.To(Recipient);
                    }
                }

                /// <summary>
                ///     The recipient.
                /// </summary>
                /// <value>The recipient.</value>
                protected MessageRecipient Recipient { get; }

                /// <summary>
                ///     Advance to the next step in the fluent builder process
                /// </summary>
                /// <value>The and.</value>
                public BuildContentStep And
                    => new BuildContentStep(Context);

                /// <summary>
                ///     Adds a display name for the recipient.
                /// </summary>
                /// <param name="displayName">The display name.</param>
                /// <returns>BuiltToWithDisplayStep.</returns>
                public BuiltToWithDisplayStep WithDisplayName(string displayName)
                {
                    return new BuiltToWithDisplayStep(Context, Recipient.WithDisplayName(displayName));
                }


                /// <summary>
                ///     Adds a replacement token and value to the collection of personalized substitutions.
                /// </summary>
                /// <remarks>
                ///     Personalized substitutions will be used to replace tokens in the subject, body, or template on a per-user
                ///     basis. Personalized substitutions override message level substitutions defined for the same token.
                /// </remarks>
                /// <param name="replacementToken">The replacement token.</param>
                /// <param name="replacementValue">The replacement value.</param>
                /// <returns>BuildRecipientSubstitutionStep.</returns>
                public BuildRecipientSubstitutionStep WithPersonalizedSubstitution(string replacementToken,
                    string replacementValue)
                {
                    return new BuildRecipientSubstitutionStep(Context,
                        Recipient.WithSubstitution(replacementToken, replacementValue));
                }

                /// <summary>
                ///     Adds a collection of replacement tokens and values to the collection of substitutions.
                /// </summary>
                /// <param name="substitutions">The substitutions.</param>
                /// <returns>BuildRecipientSubstitutionStep.</returns>
                public BuildRecipientSubstitutionStep WithPersonalizedSubstitutions(
                    IDictionary<string, string> substitutions)
                {
                    return new BuildRecipientSubstitutionStep(Context, Recipient.WithSubstitutions(substitutions));
                }

                /// <summary>
                ///     Fluent message builder step for defining one or more message recipients.
                /// </summary>
                /// <seealso cref="NullDesk.Extensions.Mailer.Core.Fluent.BuilderRecipientContext" />
                public class BuiltToWithDisplayStep : BuilderRecipientContext
                {
                    /// <summary>
                    ///     Initializes a new instance of the <see cref="BuiltToWithDisplayStep" /> class.
                    /// </summary>
                    /// <param name="context">The context.</param>
                    /// <param name="recipient">The recipient.</param>
                    public BuiltToWithDisplayStep(MailerMessage context, MessageRecipient recipient)
                        : base(context, recipient)
                    {
                    }

                    /// <summary>
                    ///     Advance to the next step in the fluent builder process
                    /// </summary>
                    /// <value>The and.</value>
                    public BuildContentStep And
                        => new BuildContentStep(Context);

                    /// <summary>
                    ///     Adds a replacement token and value to the collection of personalized substitutions.
                    /// </summary>
                    /// <remarks>
                    ///     Personalized substitutions will be used to replace tokens in the subject, body, or template on a per-user
                    ///     basis. Personalized substitutions override message level substitutions defined for the same token.
                    /// </remarks>
                    /// <param name="replacementToken">The replacement token.</param>
                    /// <param name="replacementValue">The replacement value.</param>
                    /// <returns>BuildRecipientWithDisplaySubstitutionStep.</returns>
                    public BuildRecipientWithDisplaySubstitutionStep WithPersonalizedSubstitution(
                        string replacementToken, string replacementValue)
                    {
                        return new BuildRecipientWithDisplaySubstitutionStep(Context,
                            Recipient.WithSubstitution(replacementToken, replacementValue));
                    }
                }

                /// <summary>
                ///     Fluent message builder step for defining one or more message recipients.
                /// </summary>
                /// <seealso cref="NullDesk.Extensions.Mailer.Core.Fluent.BuilderRecipientContext" />
                public class BuildRecipientSubstitutionStep : BuilderRecipientContext
                {
                    /// <summary>
                    ///     Initializes a new instance of the <see cref="BuildRecipientSubstitutionStep" /> class.
                    /// </summary>
                    /// <param name="context">The context.</param>
                    /// <param name="recipient">The recipient.</param>
                    public BuildRecipientSubstitutionStep(MailerMessage context, MessageRecipient recipient)
                        : base(context, recipient)
                    {
                    }

                    /// <summary>
                    ///     Advance to the next step in the fluent builder process
                    /// </summary>
                    /// <value>The and.</value>
                    public BuildContentStep And
                        => new BuildContentStep(Context);

                    /// <summary>
                    ///     Adds a display name for the recipient.
                    /// </summary>
                    /// <param name="displayName">The display name.</param>
                    /// <returns>BuiltToWithDisplayStep.</returns>
                    public BuiltToWithDisplayStep WithDisplayName(string displayName)
                    {
                        return new BuiltToWithDisplayStep(Context, Recipient.WithDisplayName(displayName));
                    }


                    /// <summary>
                    ///     Adds a replacement token and value to the collection of personalized substitutions.
                    /// </summary>
                    /// <remarks>
                    ///     Personalized substitutions will be used to replace tokens in the subject, body, or template on a per-user
                    ///     basis. Personalized substitutions override message level substitutions defined for the same token.
                    /// </remarks>
                    /// <param name="replacementToken">The replacement token.</param>
                    /// <param name="replacementValue">The replacement value.</param>
                    /// <returns>BuildRecipientWithDisplaySubstitutionStep.</returns>
                    public BuildRecipientSubstitutionStep WithPersonalizedSubstitution(string replacementToken,
                        string replacementValue)
                    {
                        return new BuildRecipientSubstitutionStep(Context,
                            Recipient.WithSubstitution(replacementToken, replacementValue));
                    }
                }

                /// <summary>
                ///     Fluent message builder step for defining one or more message recipients.
                /// </summary>
                /// <seealso cref="NullDesk.Extensions.Mailer.Core.Fluent.BuilderRecipientContext" />
                public class BuildRecipientWithDisplaySubstitutionStep : BuilderRecipientContext
                {
                    /// <summary>
                    ///     Initializes a new instance of the <see cref="BuildRecipientWithDisplaySubstitutionStep" /> class.
                    /// </summary>
                    /// <param name="context">The context.</param>
                    /// <param name="recipient">The recipient.</param>
                    public BuildRecipientWithDisplaySubstitutionStep(MailerMessage context, MessageRecipient recipient)
                        : base(context, recipient)
                    {
                    }

                    /// <summary>
                    ///     Advance to the next step in the fluent builder process
                    /// </summary>
                    /// <value>The and.</value>
                    public BuildContentStep And
                        => new BuildContentStep(Context);


                    /// <summary>
                    ///     Adds a replacement token and value to the collection of personalized substitutions.
                    /// </summary>
                    /// <remarks>
                    ///     Personalized substitutions will be used to replace tokens in the subject, body, or template on a per-user
                    ///     basis. Personalized substitutions override message level substitutions defined for the same token.
                    /// </remarks>
                    /// <param name="replacementToken">The replacement token.</param>
                    /// <param name="replacementValue">The replacement value.</param>
                    /// <returns>BuildRecipientWithDisplaySubstitutionStep.</returns>
                    public BuildRecipientWithDisplaySubstitutionStep WithPersonalizedSubstitution(
                        string replacementToken, string replacementValue)
                    {
                        return new BuildRecipientWithDisplaySubstitutionStep(Context,
                            Recipient.WithSubstitution(replacementToken, replacementValue));
                    }
                }
            }
        }
    }
}