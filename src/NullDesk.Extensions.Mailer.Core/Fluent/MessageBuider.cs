using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;

namespace NullDesk.Extensions.Mailer.Core.Fluent
{
    /// <summary>
    ///     Fluent builder for creating a MailerMessage.
    /// </summary>
    /// <seealso cref="BuilderContext" />
    public partial class MessageBuilder : BuilderContext
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageBuilder" /> class.
        /// </summary>
        public MessageBuilder() : base(new MailerMessage())
        {
        }

        /// <summary>
        ///     Add sender info with the specified email address.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <returns>BuildFromStep.</returns>
        public BuildFromStep From(string emailAddress)
            => new BuildFromStep(Context.From(emailAddress));

        //Step 2 = MessageBuilder.BuildSubjectStep.cs


        //Step 1 = MessageBuilder.BuildFromStep.cs

        //Step 3 = MessageBuilder.BuildRecipientsStep.cs

        //Step 4 = MessageBuilder.BuildContentStep.cs

        //Step 5 = MessageBuilder.BuildPostContentStep
    }
}