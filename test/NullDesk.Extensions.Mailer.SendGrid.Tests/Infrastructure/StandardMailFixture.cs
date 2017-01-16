using System;
using Microsoft.Extensions.DependencyInjection;
using NullDesk.Extensions.Mailer.Core;
using SendGrid;

namespace NullDesk.Extensions.Mailer.SendGrid.Tests.Infrastructure
{
    public static class ServiceProviderExtensions
    {
        public static IMailer GetSendGridMailer(this IServiceProvider provider, Response fakeTestResponse)
        {
            var client = provider.GetService<Client>();
            var mailer = provider.GetService<IMailer>();
            ((SendGridMailer)mailer).MailClient = client;
            return mailer;
        }
    }
    public class StandardMailFixture: IDisposable
    {
        public IServiceProvider ServiceProvider { get; set; }

        public StandardMailFixture()
        {

            //setup the dependency injection service
            var services = new ServiceCollection();

            services.AddOptions();
           
            services.Configure<SendGridMailerSettings>(s => s.ApiKey = "abc");
            services.AddTransient<Client>(s => new FakeClient("abc"));
            services.AddTransient<SendGridMailer>();
            services.AddTransient<IMailer>(s => s.GetService<SendGridMailer>());


            ServiceProvider = services.BuildServiceProvider();
            
        }

        

        public void Dispose()
        {
            
        }

        
    }
}
