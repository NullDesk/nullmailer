using System;
using System.IO;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NullDesk.Extensions.Mailer.MailKit.Authentication;
using Xunit;

namespace NullDesk.Extensions.Mailer.MailKit.Tests
{
    public class MailKitSettingsFromJsonTests
    {
        private IConfigurationRoot AcquireConfiguration()
        {
            var hasUserSettings = File.Exists($"{AppContext.BaseDirectory}\\appsettings-user.json");

            var appsettingsFileName =
                hasUserSettings
                    ? "appsettings-user.json"
                    : "appsettings.json";

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(appsettingsFileName, false, true);

            return builder.Build();
        }

        [Fact]
        public void MailKit_Settings_BasicAuthentication()
        {
            var config = AcquireConfiguration();

            //setup the dependency injection service
            var services = new ServiceCollection();
            services.AddOptions();
            var c = config.GetSection("BasicAuthMailSettings:MkSmtpMailerSettings");
            services.Configure<MkSmtpMailerSettings>(c);

            var provider = services.BuildServiceProvider();
            var settings = provider.GetService<IOptions<MkSmtpMailerSettings>>();
            settings.Value.FromEmailAddress.Should().Be("test@test.com");
            settings.Value.AuthenticationSettings
                .Should()
                .NotBeNull()
                .And.BeOfType<MkSmtpAuthenticationSettings>()
                .Which.Authenticator.Should()
                .BeOfType<MkSmtpBasicAuthenticator>()
                .Which.Password.Should()
                .Be("xyz");

            settings.Value.EnableSslServerCertificateValidation.Should().BeTrue();
        }

        [Fact]
        public void MailKit_Settings_EmptyAuthentication()
        {
            var config = AcquireConfiguration();

            //setup the dependency injection service
            var services = new ServiceCollection();
            services.AddOptions();
            services.Configure<MkSmtpMailerSettings>(config.GetSection("EmptyAuthMailSettings:MkSmtpMailerSettings"));

            var provider = services.BuildServiceProvider();
            var settings = provider.GetService<IOptions<MkSmtpMailerSettings>>();
            settings.Value.FromEmailAddress.Should().Be("test@test.com");
            settings.Value.AuthenticationSettings
                .Should()
                .NotBeNull()
                .And.BeOfType<MkSmtpAuthenticationSettings>()
                .Which.Authenticator.Should()
                .NotBeNull()
                .And.BeOfType<NullAuthenticator>();
        }


        [Fact]
        public void MailKit_Settings_NoAuthentication()
        {
            var config = AcquireConfiguration();

            //setup the dependency injection service
            var services = new ServiceCollection();
            services.AddOptions();
            services.Configure<MkSmtpMailerSettings>(config.GetSection("NoAuthMailSettings:MkSmtpMailerSettings"));

            var provider = services.BuildServiceProvider();
            var settings = provider.GetService<IOptions<MkSmtpMailerSettings>>();
            settings.Value.FromEmailAddress.Should().Be("test@test.com");
            settings.Value.AuthenticationSettings
                .Should()
                .NotBeNull()
                .And.BeOfType<MkSmtpAuthenticationSettings>()
                .Which.Authenticator.Should()
                .NotBeNull()
                .And.BeOfType<NullAuthenticator>();
        }

        [Fact]
        public void MailKit_Settings_TokenAuthentication()
        {
            var config = AcquireConfiguration();

            //setup the dependency injection service
            var services = new ServiceCollection();
            services.AddOptions();
            services.Configure<MkSmtpMailerSettings>(config.GetSection("TokenAuthMailSettings:MkSmtpMailerSettings"));

            var provider = services.BuildServiceProvider();
            var settings = provider.GetService<IOptions<MkSmtpMailerSettings>>();
            settings.Value.AuthenticationSettings.Authenticator = new MkSmtpAccessTokenAuthenticator
            {
                AccessTokenFactory = () => "tokenabc",
                UserName = settings.Value.AuthenticationSettings.UserName
            };
            settings.Value.FromEmailAddress.Should().Be("test@test.com");
            var authSettings = settings.Value.AuthenticationSettings
                .Should()
                .NotBeNull()
                .And.BeOfType<MkSmtpAuthenticationSettings>();


            var auth = authSettings.Which.Authenticator.Should()
                .BeOfType<MkSmtpAccessTokenAuthenticator>();
            auth
                .Which.AccessTokenFactory()
                .Should()
                .Be("tokenabc");
            auth
                .Which.UserName.Should()
                .Be("toast@toast.com");

            settings.Value.EnableSslServerCertificateValidation.Should().BeFalse();
        }
    }
}