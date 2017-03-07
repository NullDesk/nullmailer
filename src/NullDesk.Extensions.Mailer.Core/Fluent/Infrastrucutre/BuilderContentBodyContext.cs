// ReSharper disable CheckNamespace
namespace NullDesk.Extensions.Mailer.Core.Fluent
{
    /// <summary>
    ///     Base fluent context for the message body content builder.
    /// </summary>
    /// <seealso cref="NullDesk.Extensions.Mailer.Core.Fluent.BuilderContext" />
    public abstract class BuilderContentBodyContext : BuilderContext
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BuilderContentBodyContext" /> class.
        /// </summary>
        /// <param name="context">The message context.</param>
        /// <param name="body">The body.</param>
        internal BuilderContentBodyContext(MailerMessage context, ContentBody body) : base(context)
        {
            Body = body;
        }

        /// <summary>
        ///     Gets the body.
        /// </summary>
        /// <value>The body.</value>
        protected ContentBody Body { get; }
    }
}