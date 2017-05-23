namespace Sample.Mailer.Cli.Configuration
{
    public class MailHistoryDbSettings
    {
        public bool EnableHistory { get; set; } = true;

        public string ConnectionString { get; set; }
    }
}