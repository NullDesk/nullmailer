using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;

namespace NullDesk.Extensions.Mailer.Core.Fluent
{
    public partial class MessageBuilder //root
    {
        public class BuildContentStep : BuilderContext
        {
            public BuildContentStep(MailerMessage context) : base(context) { }

            public BuildRecipientsStep.BuildToStep To(string emailAddress)
                => new BuildRecipientsStep.BuildToStep(Context, emailAddress);

            public BuildContentTemplateStep ForTemplate(string templateName)
                => new BuildContentTemplateStep(Context.WithBody<TemplateBody>(b => b.TemplateName = templateName));

            public BuildBodyStep ForBody()
                => new BuildBodyStep(Context);

            public class BuildContentTemplateStep : BuilderContext
            {
                public BuildContentTemplateStep(MailerMessage context) : base(context) { }

                public BuildPostContentStep And
                    => new BuildPostContentStep(Context);

                public MailerMessage Build()
                    => Context;
            }

            public class BuildBodyStep : BuilderContext
            {
                private ContentBody Body { get; }

                public BuildBodyStep(MailerMessage context) : base(context)
                {
                    Body = new ContentBody();
                    Context.Body = Body;
                }

                public BuildHtmlBodyStep WithHtml(string html)
                    => new BuildHtmlBodyStep(Context, Body.WithHtml(html));

                public BuildTextBodyStep WithPlainText(string text)
                    => new BuildTextBodyStep(Context, Body.WithPlainText(text));

                public class BuildHtmlBodyStep : BuilderContentBodyContext
                {
                    
                    public BuildHtmlBodyStep(MailerMessage context, ContentBody body) : base(context, body) { }

                    public BuildBodyCompleteStep AndPlainText(string text)
                    {
                        Body.WithPlainText(text);
                        return new BuildBodyCompleteStep(Context);
                    }

                    public BuildPostContentStep And
                        => new BuildPostContentStep(Context);

                    public MailerMessage Build()
                        => Context;
                }

                public class BuildTextBodyStep : BuilderContentBodyContext
                {

                    public BuildTextBodyStep(MailerMessage context, ContentBody body) : base(context, body) { }

                    public BuildBodyCompleteStep AndHtml(string html)
                    {
                        Body.WithHtml(html);
                        return new BuildBodyCompleteStep(Context);
                    }

                    public BuildPostContentStep And
                        => new BuildPostContentStep(Context);

                    public MailerMessage Build()
                        => Context;
                }

                public class BuildBodyCompleteStep : BuilderContext
                {
                    public BuildBodyCompleteStep(MailerMessage context) : base(context) { }

                    public BuildPostContentStep And
                        => new BuildPostContentStep(Context);

                    public MailerMessage Build()
                        => Context;
                }
            }
        }
    }
}
