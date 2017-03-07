// ReSharper disable CheckNamespace
namespace NullDesk.Extensions.Mailer.Core.Fluent
{
    /// <summary>
    ///     Marker Interface for a message builder step that can be built to return a usable mailer message
    /// </summary>
    public interface IBuilderStepsCompleted
    {
        /// <summary>
        ///     Completes the fluent builder and returns the mailer message instance.
        /// </summary>
        /// <returns>MailerMessage.</returns>
        MailerMessage Build();
    }
}