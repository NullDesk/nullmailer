using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Mailer.Cli.Configuration
{
    public class MailHistoryDbSettings
    {
        public bool EnableHistory { get; set; } = true;

        public string ConnectionString { get; set; }
    }
}
