using System.Collections.Generic;

namespace Sample.Mailer.Cli.Configuration
{
    public class SimpleMessageSettings
    {
        public string ToAddress { get; set; }

        public string ToDisplayName { get; set; }

        public string Subject { get; set; }

        public string HtmlBody { get; set; }

        public string TextBody { get; set; }
    }

    public class TemplateMessageSettings
    {

        public string Template { get; set; }

        public string ToAddress { get; set; }

        public string ToDisplayName { get; set; }

        public string Subject { get; set; }

        public Dictionary<string, string> ReplacementVariables { get; set; }
    }

    public class TestMessageSettings
    {
        public SimpleMessageSettings SimpleMessageSettings { get; set; }

        public TemplateMessageSettings SendGridTemplateMessage { get; set; }

        public TemplateMessageSettings MailKitTemplateMessage { get; set; }
    }
}