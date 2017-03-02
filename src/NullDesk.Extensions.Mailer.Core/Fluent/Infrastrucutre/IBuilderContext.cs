// ReSharper disable CheckNamespace

namespace NullDesk.Extensions.Mailer.Core.Fluent
{
    /// <summary>
    ///     Interface for fluent message builder contexts.
    /// </summary>
    public interface IBuilderContext
    {
        /// <summary>
        ///     The message being built by the context.
        /// </summary>
        /// <value>The message.</value>
        MailerMessage Message { get; set; }
    }
}