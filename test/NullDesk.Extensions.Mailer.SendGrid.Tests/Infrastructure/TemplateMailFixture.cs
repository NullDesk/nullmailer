using System;
using Microsoft.Extensions.DependencyInjection;
using NullDesk.Extensions.Mailer.Core;
using SendGrid;

namespace NullDesk.Extensions.Mailer.SendGrid.Tests.Infrastructure
{

    public class TemplateMailFixture : IDisposable
    {
        public IServiceProvider ServiceProvider { get; set; }

        public TemplateMailFixture()
        {

            //setup the dependency injection service
            var services = new ServiceCollection();

            services.AddOptions();

            services.Configure<SendGridMailerSettings>(s => s.ApiKey = "abc");
            services.Configure<EmptyMailerTemplateSettings>(s => { });
            services.AddTransient<Client>(s => new FakeClient("abc"));
            services.AddTransient<SendGridTemplateMailer>();
            services.AddTransient<ITemplateMailer>(s => s.GetService<SendGridTemplateMailer>());


            ServiceProvider = services.BuildServiceProvider();

        }



        public void Dispose()
        {

        }


    }
}
