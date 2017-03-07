using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;

namespace NullDesk.Extensions.Mailer.Core.Fluent
{
    public partial class MessageBuilder
    {
        /// <summary>
        ///     Fluent message builder step for creating the message's main content.
        /// </summary>
        /// <seealso cref="NullDesk.Extensions.Mailer.Core.Fluent.BuilderContext" />
        public class BuildContentStep : BuilderContext
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="BuildContentStep" /> class.
            /// </summary>
            /// <param name="context">The context.</param>
            public BuildContentStep(MailerMessage context) : base(context)
            {
            }

            /// <summary>
            ///     Add a recipient with the specified email address.
            /// </summary>
            /// <param name="emailAddress">The email address.</param>
            /// <returns>BuildRecipientsStep.BuildToStep.</returns>
            public BuildRecipientsStep.BuildToStep To(string emailAddress)
                => new BuildRecipientsStep.BuildToStep(Context, emailAddress);

            /// <summary>
            ///     Add message content using the specified template.
            /// </summary>
            /// <param name="templateName">Name of the template.</param>
            /// <returns>BuildContentTemplateStep.</returns>
            public BuildContentTemplateStep ForTemplate(string templateName)
                => new BuildContentTemplateStep(Context.WithBody<TemplateBody>(b => b.TemplateName = templateName));

            /// <summary>
            ///     Create an explicitly defined body.
            /// </summary>
            /// <returns>BuildBodyStep.</returns>
            public BuildBodyStep ForBody()
                => new BuildBodyStep(Context);

            /// <summary>
            ///     Fluent message builder step for specifying a template for the message's main content .
            /// </summary>
            /// <seealso cref="NullDesk.Extensions.Mailer.Core.Fluent.BuilderContext" />
            public class BuildContentTemplateStep : BuilderContext, IBuilderStepsCompleted
            {
                /// <summary>
                ///     Initializes a new instance of the <see cref="BuildContentTemplateStep" /> class.
                /// </summary>
                /// <param name="context">The context.</param>
                public BuildContentTemplateStep(MailerMessage context) : base(context)
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
                    => Context;
            }

            /// <summary>
            ///     Fluent message builder step for explicitly defining the message's main content.
            /// </summary>
            /// <seealso cref="BuilderContext" />
            public class BuildBodyStep : BuilderContext
            {
                /// <summary>
                ///     Initializes a new instance of the <see cref="BuildBodyStep" /> class.
                /// </summary>
                /// <param name="context">The context.</param>
                public BuildBodyStep(MailerMessage context) : base(context)
                {
                    Body = new ContentBody();
                    Context.Body = Body;
                }

                /// <summary>
                ///     The builder context for the message body.
                /// </summary>
                /// <value>The body.</value>
                private ContentBody Body { get; }

                /// <summary>
                ///     Adds an HTML body to the message.
                /// </summary>
                /// <param name="html">The HTML.</param>
                /// <returns>BuildHtmlBodyStep.</returns>
                public BuildHtmlBodyStep WithHtml(string html)
                    => new BuildHtmlBodyStep(Context, Body.WithHtml(html));

                /// <summary>
                ///     Adds a plain text body to the message.
                /// </summary>
                /// <param name="text">The text.</param>
                /// <returns>BuildTextBodyStep.</returns>
                public BuildTextBodyStep WithPlainText(string text)
                    => new BuildTextBodyStep(Context, Body.WithPlainText(text));

                /// <summary>
                ///     Fluent message builder step for explicitly defining the message's main content.
                /// </summary>
                /// <seealso cref="BuilderContentBodyContext" />
                public class BuildHtmlBodyStep : BuilderContentBodyContext, IBuilderStepsCompleted
                {
                    /// <summary>
                    ///     Initializes a new instance of the <see cref="BuildHtmlBodyStep" /> class.
                    /// </summary>
                    /// <param name="context">The context.</param>
                    /// <param name="body">The body.</param>
                    public BuildHtmlBodyStep(MailerMessage context, ContentBody body) : base(context, body)
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
                        => Context;

                    /// <summary>
                    ///     Adds a plain text body to the message.
                    /// </summary>
                    /// <param name="text">The text.</param>
                    /// <returns>BuildBodyCompleteStep.</returns>
                    public BuildBodyCompleteStep AndPlainText(string text)
                    {
                        Body.WithPlainText(text);
                        return new BuildBodyCompleteStep(Context);
                    }
                }

                /// <summary>
                ///     Fluent message builder step for explicitly defining the message's main content.
                /// </summary>
                /// <seealso cref="NullDesk.Extensions.Mailer.Core.Fluent.BuilderContentBodyContext" />
                public class BuildTextBodyStep : BuilderContentBodyContext, IBuilderStepsCompleted
                {
                    /// <summary>
                    ///     Initializes a new instance of the <see cref="BuildTextBodyStep" /> class.
                    /// </summary>
                    /// <param name="context">The context.</param>
                    /// <param name="body">The body.</param>
                    public BuildTextBodyStep(MailerMessage context, ContentBody body) : base(context, body)
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
                        => Context;

                    /// <summary>
                    ///     Adds an HTML body to the message.
                    /// </summary>
                    /// <param name="html">The HTML.</param>
                    /// <returns>BuildBodyCompleteStep.</returns>
                    public BuildBodyCompleteStep AndHtml(string html)
                    {
                        Body.WithHtml(html);
                        return new BuildBodyCompleteStep(Context);
                    }
                }

                /// <summary>
                ///     Fluent message builder step for explicitly defining the message's main content.
                /// </summary>
                /// <seealso cref="NullDesk.Extensions.Mailer.Core.Fluent.BuilderContext" />
                public class BuildBodyCompleteStep : BuilderContext, IBuilderStepsCompleted
                {
                    /// <summary>
                    ///     Initializes a new instance of the <see cref="BuildBodyCompleteStep" /> class.
                    /// </summary>
                    /// <param name="context">The context.</param>
                    public BuildBodyCompleteStep(MailerMessage context) : base(context)
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
                        => Context;
                }
            }
        }
    }
}