using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.SendGrid.Tests
{
    public class Program
    {
        public static IServiceProvider ServiceProvider { get; set; }

        static Program()
        {
            //setup the dependency injection service
            var services = new ServiceCollection();

            services.AddOptions();

            ServiceProvider = services.BuildServiceProvider();
        }

        public static void Main(string[] args)
        {
        }
    }
}
