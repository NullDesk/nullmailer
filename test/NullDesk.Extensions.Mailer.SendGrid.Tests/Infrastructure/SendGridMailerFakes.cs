using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NullDesk.Extensions.Mailer.Core;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace NullDesk.Extensions.Mailer.SendGrid.Tests.Infrastructure
{
    public class SendGridSimpleMailerFake: SendGridSimpleMailer
    {
        public SendGridSimpleMailerFake(SendGridClient client, IOptions<SendGridMailerSettings> settings, ILogger<SendGridSimpleMailer> logger = null, IHistoryStore historyStore = null) : base(client, settings, logger, historyStore)
        {
        }

        public SendGridSimpleMailerFake(IOptions<SendGridMailerSettings> settings, ILogger<SendGridSimpleMailer> logger = null, IHistoryStore historyStore = null) : base(settings, logger, historyStore)
        {
        }

        protected override Task<Response> SendToApiAsync(SendGridMessage message, CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult(new Response(HttpStatusCode.Accepted, null, null));
        }
    }

    public class SendGridMailerFake : SendGridMailer
    {
        public SendGridMailerFake(SendGridClient client, IOptions<SendGridMailerSettings> settings, ILogger<SendGridMailer> logger = null, IHistoryStore historyStore = null) : base(client, settings, logger, historyStore)
        {
        }

        public SendGridMailerFake(IOptions<SendGridMailerSettings> settings, ILogger<SendGridMailer> logger = null, IHistoryStore historyStore = null) : base(settings, logger, historyStore)
        {
        }

        protected override Task<Response> SendToApiAsync(SendGridMessage message, CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult(new Response(HttpStatusCode.Accepted, null, null));
        }
    }
}
