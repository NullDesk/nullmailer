using System;
using Microsoft.Extensions.DependencyInjection;
using NullDesk.Extensions.Mailer.Core;
using SendGrid;

namespace NullDesk.Extensions.Mailer.SendGrid.Tests.Infrastructure
{
    public static class ServiceProviderExtensions
    {
        //public static IMailer GetSendGridMailer(this IServiceProvider provider, Response fakeTestResponse)
        //{
        //    var client = provider.GetService<Client>();
        //    var mailer = provider.GetService<IMailer>();
        //    ((SendGridMailer)mailer).MailClient = client;
        //    return mailer;
        //}

        //public static ITemplateMailer GetSendGridTemplateMailer(this IServiceProvider provider, Response fakeTestResponse)
        //{
        //    var client = provider.GetService<Client>();
        //    var mailer = provider.GetService<ITemplateMailer>();
        //    ((SendGridTemplateMailer)mailer).MailClient = client;
        //    return mailer;
        //}
    }
}