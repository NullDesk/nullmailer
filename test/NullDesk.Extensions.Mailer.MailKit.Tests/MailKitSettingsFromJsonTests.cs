using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace NullDesk.Extensions.Mailer.MailKit.Tests
{
    public class MailKitSettingsFromJsonTests
    {
        [Fact]
        public void GetSettings_NoAuthentication()
        {
            var config = AcquireConfiguration();

            //setup the dependency injection service
            var services = new ServiceCollection();
            services.AddOptions();
            services.Configure<MkSmtpMailerSettings>(config.GetSection("NoAuthMailSettings:MkSmtpMailerSettings"));

            var provider = services.BuildServiceProvider();
            var settings = provider.GetService<IOptions<MkSmtpMailerSettings>>();
            settings.Value.FromEmailAddress.Should().Be("test@test.com");
            settings.Value.AuthenticationSettings.Should().BeNull();
        }
        [Fact]
        public void GetSettings_AuthenticationNoCredentials()
        {
            var config = AcquireConfiguration();

            //setup the dependency injection service
            var services = new ServiceCollection();
            services.AddOptions();
            services.Configure<MkSmtpMailerSettings>(config.GetSection("AuthMailNoCredentialsSettings:MkSmtpMailerSettings"));

            var provider = services.BuildServiceProvider();
            var settings = provider.GetService<IOptions<MkSmtpMailerSettings>>();
            settings.Value.FromEmailAddress.Should().Be("test@test.com");
            settings.Value.AuthenticationSettings
                .Should().NotBeNull()
                .And.BeOfType<MkSmtpAuthenticationSettings>()
                .Which.AuthenticationMode.Should().Be(MkSmtpAuthenticationMode.Token);
                
            settings.Value.AuthenticationSettings.AccessTokenAuthentication.Should().BeNull();
            settings.Value.AuthenticationSettings.BasicAuthentication.Should().BeNull();
            settings.Value.AuthenticationSettings.CredentialsAuthentication.Should().BeNull();
            settings.Value.EnableSslServerCertificateValidation.Should().BeTrue();
        }

        [Fact]
        public void GetSettings_BasicAuthentication()
        {
            var config = AcquireConfiguration();

            //setup the dependency injection service
            var services = new ServiceCollection();
            services.AddOptions();
            services.Configure<MkSmtpMailerSettings>(config.GetSection("BasicAuthMailSettings:MkSmtpMailerSettings"));

            var provider = services.BuildServiceProvider();
            var settings = provider.GetService<IOptions<MkSmtpMailerSettings>>();
            settings.Value.FromEmailAddress.Should().Be("test@test.com");
            settings.Value.AuthenticationSettings.BasicAuthentication
                .Should().NotBeNull()
                .And.BeOfType<MkSmtpBasicAuthenticationSettings>()
                .Which.Password.Should().Be("xyz");
            
            settings.Value.EnableSslServerCertificateValidation.Should().BeTrue();
        }

        [Fact]
        public void GetSettings_TokenAuthentication()
        {
            var config = AcquireConfiguration();

            //setup the dependency injection service
            var services = new ServiceCollection();
            services.AddOptions();
            services.Configure<MkSmtpMailerSettings>(config.GetSection("TokenAuthMailSettings:MkSmtpMailerSettings"));

            var provider = services.BuildServiceProvider();
            var settings = provider.GetService<IOptions<MkSmtpMailerSettings>>();
            settings.Value.FromEmailAddress.Should().Be("test@test.com");
            settings.Value.AuthenticationSettings
                .Should().NotBeNull()
                .And.BeOfType<MkSmtpAuthenticationSettings>()
                .Which.AccessTokenAuthentication
                .Should().NotBeNull()
                .And.BeOfType<MkSmtpAccessTokenAuthenticationSettings>()
                .Which.AccessToken.Should().Be("abc");

            settings.Value.EnableSslServerCertificateValidation.Should().BeFalse();
        }

        [Fact]
        public void GetSettings_CombinedAuthentication()
        {
            var config = AcquireConfiguration();

            //setup the dependency injection service
            var services = new ServiceCollection();
            services.AddOptions();
            services.Configure<MkSmtpMailerSettings>(config.GetSection("CombinedAuthMailSettings:MkSmtpMailerSettings"));

            var provider = services.BuildServiceProvider();
            var settings = provider.GetService<IOptions<MkSmtpMailerSettings>>();
            settings.Value.FromEmailAddress.Should().Be("test@test.com");
            settings.Value.AuthenticationSettings.GetSettingsForAuthenticationMode()
                .Should().NotBeNull()
                .And.BeOfType<MkSmtpBasicAuthenticationSettings>()
                .Which.Password.Should().Be("xyz");
            settings.Value.AuthenticationSettings.AccessTokenAuthentication
                .Should()
                .NotBeNull();

            settings.Value.EnableSslServerCertificateValidation.Should().BeTrue();
        }

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
    }
}
