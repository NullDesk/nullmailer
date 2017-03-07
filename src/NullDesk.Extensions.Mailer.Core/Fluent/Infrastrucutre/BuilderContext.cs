// ReSharper disable CheckNamespace
namespace NullDesk.Extensions.Mailer.Core.Fluent
{
    /// <summary>
    ///     Base fluent context for the message builder.
    /// </summary>
    /// <seealso cref="NullDesk.Extensions.Mailer.Core.Fluent.IBuilderContext" />
    public abstract class BuilderContext : IBuilderContext
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BuilderContext" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        internal BuilderContext(MailerMessage context)
        {
            ((IBuilderContext) this).Message = context;
        }

        /// <summary>
        ///     Gets the context.
        /// </summary>
        /// <remarks>
        ///     Allows inheritors to access the mailer message directly without needing to cast to IBuilderContext each time.
        /// </remarks>
        /// <value>The context.</value>
        protected MailerMessage Context => ((IBuilderContext) this).Message;

        /// <summary>
        ///     The message this context refers to.
        /// </summary>
        /// <remarks>
        ///     Explicit interface implementation. Hidden from intellisense and general public usage, but still allows unit tests
        ///     and other callers to cast the instance to IBuilderContext and directly access the underlying mailer message if
        ///     necessary.
        /// </remarks>
        /// <value>The message.</value>
        MailerMessage IBuilderContext.Message { get; set; }
    }
}