# NullDesk Email Extensions

## Overview

Email extensions for quickly integrating common mail scenarios into any .Net Core project.

Easily configure your application for different email services at startup based on environment, deployment configuration, or custom detection of available mail providers.

## Status

|                                                                                             |                                                                                                                                                                                                           |
| ------------------------------------------------------------------------------------------- | :-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------: |
| Project  [Issue Board](https://github.com/NullDesk/NullMailer/issues#boards?repos=79507993) |                                                                                                                                                                                                           |
| NullDesk.Extensions.Mailer.Core                                                             |                              [![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.Core.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.Core/)                              |
| NullDesk.Extensions.Mailer.MailKit                                                          |                           [![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.MailKit.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.MailKit/)                           |
| NullDesk.Extensions.Mailer.SendGrid                                                         |                          [![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.SendGrid.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.SendGrid/)                          |
| NullDesk.Extensions.Mailer.History.EntityFramework                                          |           [![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.History.EntityFramework.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.History.EntityFramework/)           |
| NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer                                | [![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer/) |

## What's New

### 5.0 Features

- [Safety Mailer](#safetymailer)  is a generic proxy for 'IMailer' that will override the recipient email address for all messages delivered by that mailer, and replace it with a configured substitute. This is useful for dev/test deployments.
- Proxy mailer framework support allows easy creation of wrappers for mailers. Support for proxy instances and settings unique to the proxy are supported by both DI and Factory frameworks.

### 5.0 Potentially Breaking Changes

- Dependency injection allows registering of more than one IMailer; however, if your application uses multiple registrations please ensure you [understand the behavior](https://www.stevejgordon.co.uk/asp-net-core-dependency-injection-registering-multiple-implementations-interface).

## Contents

- [NullDesk Email Extensions](#nulldesk-email-extensions)
  - [Overview](#overview)
  - [Status](#status)
  - [What's New](#whats-new)
    - [5.0 Features](#features)
    - [5.0 Potentially Breaking Changes](#potentially-breaking-changes)
  - [Contents](#contents)
  - [Features](#features)
  - [Package Descriptions](#package-descriptions)
  - [Basic Usage](#basic-usage)
    - [Mailer Instantiation](#mailer-instantiation)
    - [Mailer Factory Usage](#mailer-factory-usage)
    - [Dependency Injection](#dependency-injection)
  - [Creating MailerMessages](#creating-mailermessages)
    - [Fluent Message Builder](#fluent-message-builder)
    - [Fluent Extensions](#fluent-extensions)
    - [Direct Message Instantiation](#direct-message-instantiation)
    - [Multiple Recipients](#multiple-recipients)
    - [Substitutions and PersonalizedSubstitutions](#substitutions-and-personalizedsubstitutions)
    - [Templates](#templates)
    - [Attachments](#attachments)
  - [Advanced Topics](#advanced-topics)
    - [Safety Mailer](#safety-mailer)
    - [Logging with ILogger](#logging-with-ilogger)
    - [History Store](#history-store)
      - [SQL Server History Store](#sql-server-history-store)
      - [Using History with Mailers](#using-history-with-mailers)
      - [ Resending Mail from History](#-resending-mail-from-history)
  - [Using Gmail](#using-gmail)
  - [Creating your own mailers](#creating-your-own-mailers)
  - [Custom History](#custom-history)
    - [Custom EF SQL History Contexts](#custom-ef-sql-history-contexts)
    - [Custom EF non-SQL History](#custom-ef-non-sql-history)
    - [Custom (non-EF) History](#custom-non-ef-history)
    - [Building and Publishing](#building-and-publishing)

## Features

- Template support for all mail services
- Replacement variables (substitutions) with or without templates
- Cross-platform Netstandard 1.3 packages
- Fluent Message Builder API
- Optional Delivery history store
- Optional support for:
  - Microsoft logging extensions
  - Microsoft dependency injection extensions

## Package Descriptions

|                                                              |                                                                                                                                                                     |
| ------------------------------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| NullDesk.Extensions.Mailer.Core                              | Base classes and interfaces for the mailer extensions, and settings.                                                                                                |
| NullDesk.Extensions.Mailer.MailKit                           | SMTP Relay Email service using the popular cross platform [MailKit library](https://github.com/jstedfast/MailKit). Includes support for basic Email Template files. |
| NullDesk.Extensions.Mailer.SendGrid                          | SendGrid email service using SendGrid APIs. Supports basic usage of SendGrid templates; can be inherited for more advanced SendGrid functionality.                  |
| NullDesk.Extensions.Mailer.History.EntityFramework           | Base classes for message and delivery history using entity framework                                                                                                |
| NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer | Implementation of message and delivery history using entity framework targeting for MS SQL Server                                                                   |
| _NullDesk.Extensions.Mailer.NetMail_                         | *(coming soon)* SMTP Relay Email service using the cross-platform System.Net.Mail framework from Microsoft.                                                         |


## Basic Usage

- Reference the package or packages your application needs:
  - For SMTP install `NullDesk.Extensions.Mailer.MailKit`.
  - For SendGrid install `NullDesk.Extensions.Mailer.MailKit`.
  - For SQL history install `NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer`.
- Obtain a mailer instance from DI, the mailer factory, or by instantiating one yourself
- Add one or more Messages to the Mailer with `AddMessage` or `CreateMessage` methods.
- Finally, call the `SendAllAsync` method.


> The built-in Mailers are reusable by default. As of 4.0.0, mailers no longer keep an internal reference to delivered items, but it is still recommended to create a new mailer instance each time.

### Mailer Instantiation

The simplest usage is to instantiate the mailer of your choice, use the fluent message builder API to create one or more messages, then send the messages:

         var settings = new SendGridMailerSettings
         {
             ApiKey = "123",
             FromDisplayName = "Person Name",
             FromEmailAddress = "someone@toast.com",
             IsSandboxMode = false
         };
         using (var mailer = new SendGridMailer(settings))
         {
             mailer.CreateMessage(b => b
                .Subject("Message for %name%")
                .And.To("recipient@toast.com").WithDisplayName("Recipient Name")
                .And.ForBody()
                    .WithHtml(htmlContentString)
                    .AndPlainText(textContentString)
                .And.WithSubstitutions(replacementVariablesDictionary)
                .And.WithAttachments(attachmentFileNamesList)
                .Build());

             await mailer.SendAllAsync(CancellationToken.None);
         }

### Mailer Factory Usage

For cases where dependency injection isn't available, or desired, you can use the mailer factory to obtain mailer instances.

        var settings = new SendGridMailerSettings
        {
            ApiKey = "123",
            FromDisplayName = "Person Name",
            FromEmailAddress = "someone@toast.com",
            ReplyToEmailAddress = "someoneFriendly@toast.com", //optional
            ReplyToDisplayName = "Reply to Person Name", //optional
            IsSandboxMode = false
        };
         
        var factory = new MailerFactory();
        factory.AddSendGridMailer(settings);
        
When you are ready to send messages:

        using(var mailer = factory.GetMailer())
        {
            mailer.CreateMessage(b => b
                .Subject("Message for %name%")
                .And.To("recipient@toast.com")
                    .WithDisplayName("Recipient Name")
                    .WithPersonalizedSubstitution("%name%", "Mr. Toast")
                .And.To("someoneElse@toast.com")
                    .WithDisplayName("Other Recipient Name")
                    .WithPersonalizedSubstitution("%name%", "Someone Else")
                .And.ForTemplate(myTemplateName)
                .Build());

            await mailer.SendAllAsync(CancellationToken.None);
        }

### Dependency Injection

When using any dependency injection framework, it is best to register types as follows:

- Mailers should be registered as `IMailer` with a transient or limited scope.
- Mailer and History settings should be registered with a singleton life-cycle.
- History Store should be registered as `IHistoryStore` with a singleton life-cyle

When using the Microsoft DI framework, there are several extension methods to assist in setting up mailers, settings, and history.

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSendGridMailer(new SendGridMailerSettings
            {
                  //your settings here
            });

            return services.BuildServiceProvider();
        }

Then you can just inject `IMailer` into any classes that need to send messages

        public MyController: Controller
        {
            private IMailer _mailer;
            
            public MyController(IMailer mailer)
            {
                _mailer = mailer;
            }

            [HttpGet]    
            public IActionResult Get()
            {
                _mailer.CreateMessage(b => b
                    .Subject("Message for %name%")
                    .And.To("recipient@toast.com")
                    .And.ForTemplate(myTemplateName)
                    .Build());

                await mailer.SendAllAsync(CancellationToken.None);
            }
        }

## Creating MailerMessages

### Fluent Message Builder

The mailer extensions include a fluent message builder API that provides a guided experience for creating complete and deliverable messages.

The main steps in the message builder are:

1. Sender
1. Subject
1. Recipient(s)
1. Body
1. Substitution(s) (optional)
1. Attachment(s) (optional)

Once you reach the body step of the builder, you can call the `Build()` method to create your message and end the builder method chain.

There are two primary ways to use the message builder. Either instantiate it yourself, or use the `CreateMessage()` method of your Mailer instance.

Minimal Example using `MessageBuilder` directly:

        var message = new MessageBuilder()
            .From("toast@toast.com")
            .And.Subject("Subject")
            .And.To("someone@somewhere.com")
            .And.ForTemplate("MyTemplate")
            .Build()

If using `CreateMessage()`, the sender and replyTo details will be provided by your mailer's settings, and the builder will begin with the subject step.

Minimal Example using `CreateMessage()`

        mailer.CreateMessage(b => b
            .Subject("Subject")
            .And.To("someone@somewhere.com")
            .And.ForTemplate("MyTemplate")
            .Build()

A Complete Example:

        var message = new MessageBuilder()
            .From("toast@toast.com")
                .WithDisplayName("Toast Man")
                .WithReplyTo("friendlyaddress@toast.com") //optional
                .WithReplyToDisplayName("Mr. Toast Man") //optional
            .And.Subject("Subject")
            .And.To("someone@somewhere.com")
                .WithDisplayName("Someone Nice")
                .WithPersonalizedSubstitution("%name%", "Mr. Nice")
            .And.To("otherguy@somewhere.com")
                .WithDisplayName("Other Guy")
                .WithPersonalizedSubstitution("%name%", "Mr. Guy")
                .WithPersonalizedSubstitution("%title%", "President")
            .And.ForBody()
                .WithHtml(html)
                .AndPlainText(text)
            .And.WithAttachment("someFile.zip")
            .And.WithAttachments(new List<string> { "secondFile.zip", "thirdFile.zup" })
            .And.WithSubstitution("%advertisement%", adText)
            .And.WithSubstitutions(new Dictionary<string, string>
                {
                    {"%company%", "Place Name"},
                    { "%phone%","1-800-123-1234" }
                })
            .Build();


### Fluent Extensions

The Mailer Extensions also includes a more traditional set of fluent extension methods.

With these extensions, the order in which you chain methods is irrelevant.

Here is a basic example:

        var message = MailerMessage.Create()
            .From(sender =>
            {
                sender.EmailAddress = "toast@toast.com";
                sender.DisplayName = "Toast Man";
            })
            .To(recipient =>
            {
                recipient.EmailAddress = "someone@somewhere.com";
                recipient.DisplayName = "Someone Nice";
            })
            .WithSubject("Subject")
            .WithBody<TemplateBody>(body => body.TemplateName = "MyTemplate");

### Direct Message Instantiation

While using the message builder fluent API is usually best, direct instantiation is also straight-forward:

        var message = new MailerMessage
        {
            From = new MessageSender()
            {
                EmailAddress = "toast@toast.com",
                DisplayName = "Toast Man"
            },
            Recipients = new List<MessageRecipient>
            {
                new MessageRecipient
                {
                    EmailAddress = "someone@somewhere.com",
                    DisplayName = "Nice Guy",
                    PersonalizedSubstitutions = new Dictionary<string, string>
                    {
                        {"%name%", "Nice Guy"},
                    }
                }
            },
            Subject = "Subject",
            Body = new ContentBody
            {
                HtmlContent = html,
                PlainTextContent = text
            },
            Attachments = new List<string>
            {
                "file1.zip",
                "file2.zip"
            }.GetAttachmentStreamsForFiles(),
            Substitutions = new Dictionary<string, string>
            {
                {"%title%", "President"}
            }
        };

### Multiple Recipients

A `MailMessage` may define multiple recipients, each potentially having their own personalized substitutions. The Mailer extensions will ensure that **each recipient receives a separate and isolated email** message.

Each `MailMessage` is converted into one or more `DeliveryItem` instances; one per recipient. When sending mail, each delivery item is sent as a unique email.

If a history store is used, then each `DeliveryItem` will have its own history record.

### Substitutions and PersonalizedSubstitutions

The mailer extensions support content substitutions for body content, subject, and templates. 

The `MailMessage` has a top-level dictionary property for supplying replacement tokens and values.Each recipient can also specify a dictionary substitutions to be used on a per-recipient basis. These collections are merged when sending mail. If the same key exists in both collections, the recipient's personalized value will be used. **Be sure your dictionary's key includes the delimiters your content or template uses.**

        message.Subject = "Hello %title% %name%";
        message.Substitutions = new Dictionary<string, string>
        {
            {"%name%", "Nice Guy"},
            {"%title%", "President"}
        };

### Templates

Different implementations of `IMailer` support templates in slightly different ways.

The `MkSmtpMailer` uses MailKit to deliver email using traditional SMTP services. To support templates, this mailer can use templates stored on the local file-system. Templates can be supplied for both HTML and Plain text content. Just before the message is delivered, the templates will be loaded, and the substitutions will be performed. The body's TemplateName should be set to the filename of the template(s) you wish to use, minus the file extension. The folder path and file extensions used to locate the template files on the file system are supplied by the mailer's settings.

The `SendGridMailer` uses SendGrid's server-side transactional templates. Here the template name should be the ID of the transactional template you wish to use. The mailer will simply pass the template name and substitutions collections to the server, which will then create the message and deliver it.

### Attachments

Attachments can added to a `MailMessage` as either a collection of file paths, or a Dictionary of file names and Streams.

## Advanced Topics

### Safety Mailer

A safety mailer is a proxy for another `IMailer` type. It replaces the recipient email address for all messages being delivered to a pre-configured setting supplied to the proxy. 

This is intended for DEV/TEST deployements where you want to enable real email, but want to ensure that all messages are delivered to a fake recipient address instead of a real user. 

Since this is a proxy mailer, it can be used to wrap any kine of `IMailer`. 

Example: Simple instantiation

         var mailerSettings = new SendGridMailerSettings
         {
             ApiKey = "123",
             FromDisplayName = "Person Name",
             FromEmailAddress = "someone@toast.com",
             IsSandboxMode = false
         }; 
         var safetySettings = new SafetyMailerSettings()
         {
             SafeRecipientEmailAddress = "safe@nowhere.com"
         };
         using (var mailer = new SafetyMailer<SendGridMailer>(new SendGridMailer(mailerSettings), safetySettings))
         {
             mailer.CreateMessage(b => b
                .Subject("Message for %name%")
                .And.To("recipient@toast.com").WithDisplayName("Recipient Name")
                .And.ForBody()
                    .WithHtml(htmlContentString)
                    .AndPlainText(textContentString)
                .And.WithSubstitutions(replacementVariablesDictionary)
                .And.WithAttachments(attachmentFileNamesList)
                .Build());

             await mailer.SendAllAsync(CancellationToken.None);
         }

There are overloads for both mailer factory and dependency injection extensions for safety mailer.

Example: DI Safety Mailer with SendGrid

    services.AddSafetyMailer(safetyMailerSettings, mailerSettings);

Example: Factory Safety Mailer with MailKit

    Mail.AddSafetyMailer(safetyMailerSettings, mkSmtpMailerSettings);

Example: DI Safety Mailer with a custom mailer

    services.AddSafetyMailer<MyMailer, MyMailerSettings>(safetyMailerSettings, myMailerSettings);

Example: Factory Safety Mailer with a custom mailer

    Mail.Register<SafetyMailer<MyMailer>, SafetyMailerSettings, MyMailer, MyMailerSettings>(safetyMailerSettings, myMailerSettings);

### Logging with ILogger

The mailer extensions support logging with [Microsoft's Logging Extensions](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging).

Example: Logging with direct Mailer instantiation:

        var loggerFactory = new LoggerFactory()
            .AddConsole(consoleLoggerConfig);

        using (var mailer = new SendGridMailer(
            settings,
            loggerFactory.CreateLogger<SendGridMailer>()))
        {
            //stuff
        }

Example: Logging with MailerFactory

        var mailerFactory = new MailerFactory(){
        {
            DefaultLoggerFactory = new LoggerFactory()
                .AddConsole(consoleLoggerConfig)
        };

        mailerFactory.AddSendGridMailer(new SendGridMailerSettings{
           //your settings here
        });

Example: Logging with MS DI extensions:

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(); //that's all you need
            services.AddOptions();
            services.Configure<SendGridMailerSettings>(s =>
            {
                   //your settings here
            });

            services.AddSendGridMailer(s => s.GetService<IOptions<SendGridMailerSettings>>().Value);

            return services.BuildServiceProvider();
        }

### History Store

The mailer extensions also supports storing message and delivery details in an optional history store.

Depending on which history package you select, configuration may be different.

The core package includes a `NullHistoryStore` (does nothing) and an `InMemoryHistoryStore` for testing purposes.

#### SQL Server History Store

To store history in SQL server using Entity Framework 7, add the `NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer` NuGet package.

There are two kinds of settings you can use.

Example: Standard SQL history store with `SqlEntityHistoryStoreSettings`:

    var historyStore = new EntityHistoryStore<SqlHistoryContext>(
        new SqlEntityHistoryStoreSettings()
        {
            ConnectionString = connectionString,
            SourceApplicationName = "MyApplication"
        });

Example: Standard SQL history with `EntityHistoryStoreSettings`:

    var historyStore = new EntityHistoryStore<SqlHistoryContext>(
        new EntityHistoryStoreSettings
        {
            DbOptions = new DbContextOptionsBuilder<HistoryContext>()
                .UseSqlServer(connectionString)
                .Options,
            SourceApplicationName = "MyApplication"
        });

Other settings of interest:

- **SourceApplicationName**: Useful when you have more than one application targeting the same history database.
- **AutoInitializeDatabase** (default=true): Run DB initialization automatically; for the SQL history store will ensure migrations have run; disable if you want manual control of the DB schema.
- **StoreAttachmentContents** (default=false): Set true to serialize the content of attachments into history; all history items will be re-sendable, but this can consume a lot of data storage (hint: if you need this kind of thing, you might want to write your own history store and store attachments to a filesystem, and otherwise optimize space usage).
- **IsEnabled** (default=true): Set to false to disable recording history.

#### Using History with Mailers

These examples demonstrate usage with the EntityFramework SQL Server History Store package, but usage will be similar for any IHistoryStore implementation.

Example: History with direct Mailer instantiation:

        var loggerFactory = new LoggerFactory();
        loggerFactory.AddConsole(consoleLoggerConfig);

        var builder = new DbContextOptionsBuilder<HistoryContext>()
                        .UseSqlServer(connectionString);

        using (var mailer = new SendGridMailer(
            settings,
            loggerFactory.CreateLogger<SendGridMailer>(),
            new EntityHistoryStore<SqlHistoryContext>(
                new EntityHistoryStoreSettings{DbOptions = builder.Options})
        {
            //stuff
        }

Example: History with the `MailerFactory`:

        var mailerFactory = new MailerFactory(){
        {
            DefaultLoggerFactory = new LoggerFactory()
                .AddConsole(consoleLoggerConfig)
        };

        mailerFactory.RegisterDefaultSqlHistory(
            new SqlEntityHistoryStoreSettings(){
                //your settings here
            });

        mailerFactory.AddSendGridMailer(
            new SendGridMailerSettings
            {
              //your settings here
            });

Example: History with MS DI extensions:

        public IServiceProvider ConfigureServices(IServiceCollection services, IConfigurationRoot config)
        {
            services.AddLogging();

            services.AddMailerSqlHistory(
                new SqlEntityHistoryStoreSettings(){
                    //your settings here
                });

            services.AddSendGridMailer(
                new SendGridMailerSettings
                {
                    //your settings here
                }
            );

            return services.BuildServiceProvider();
        }

#### Resending Mail from History

Using a history store enables mailers to re-send messages.

You can use the history store to get the ID of a message you want to re-send, then just call `ReSendAsync` on your mailer instance, passing in the the message Id you wish to resend.

The re-send will **immediately** re-deliver the message. You do not have to call `SendAllAsync`. Note that `ReSendAsync` will not deliver any other messages already added to the mailer instance.

**By default, messages with attachments cannot be resent**. Since the default setting for for most implementations of `IHistoryStore` is to ignore the attachment file contents, the complete message is not available for re-delivery.

You can enable attachment serialization and storage for the history store in the settings by changing the `StoreAttachmentContents` value to `true`.

## Using Gmail

If you are using GMail with the MailKit SMTP Mailer, you will need to use the less secure username/password authentication, or you will need to obtain and use oAuth access tokens.

Review the [MailKit GMail documentation](https://github.com/jstedfast/MailKit/blob/master/FAQ.md#GMailAccess) first.

When configuring your settings for GMail, you can just supply a username and password in `MkSmtpAuthenticationSettings` for the less secure connection method.

        var settings = new MkSmtpMailerSettings
        {
            FromDisplayName = "Person Name",
            FromEmailAddress = "someone@toast.com",
            AuthenticationSettings= new MkSmtpAuthenticationSettings
            {
                UserName = "myuser@gmail.com",
                Password = "abc"
            }
        };

When using oAuth, you will need to supply a value to the `Authenticator` property of `MkSmtpAuthenticationSettings`.

        var settings = new MkSmtpMailerSettings
        {
            FromDisplayName = "Person Name",
            FromEmailAddress = "someone@toast.com",
            AuthenticationSettings= new MkSmtpAuthenticationSettings
            {
                Authenticator = new MkSmtpAccessTokenAuthenticator
                {
                    UserName = "myuser@gmail.com",
                    AccessTokenFactory = () => MyTokenManager.SomeMethodThatGetsTokens()
                }
            }
        };

## Creating your own mailers

To implement your own mailer, simply implement a class that inherits `Mailer<TSettings>`.

For the above interfaces, `TSettings` is a custom class that implements `IMailerSettings`, then adds any custom configuration properties your mailer or the underlying mail framework needs.

        public class MyMailerSettings : IMailerSettings
        {
            public string FromEmailAddress { get; set; }

            public string FromDisplayName { get; set; }

            //... any other properties your mailer needs
        }

Every effort has been made to keep inheritance of `Mailer<TSettings>` fairly straight forward.

The primary method you must supply is a method that can deliver a single `DeliveryItem` using the email service your mailer supports. The method should return the DeliveryItem if it was successfully sent, or throw an exception if something went wrong.

The base class will handle logging the exception, updating the DeliveryItem's properties, recording the delivery attempt to the history store, etc. All you have to do is send the email via the mail service, and either throw an exception or return the delivery item if it was sent.

        public class MySimpleMailer : Mailer<MyMailerSettings>
        {
            protected override async Task<string> DeliverMessageAsync(
                DeliveryItem deliveryItem,
                bool autoCloseConnection = true,
                CancellationToken token = default(CancellationToken))
            {
                //... implementation here
                // return provider supplied message ID, or null if not applicable
            }

            protected override async Task CloseMailClientConnectionAsync(CancellationToken token = default(CancellationToken))
            {
                //explicitly close any open mail client connections
            }
        }

## Custom History

### Custom EF SQL History Contexts

For SQL History with EntityFramework, you can create your own DbContext type if you prefer not to use the build-in context.

Create a class inheriting `SqlHistoryContext`, and override any members you wish. You can treat this like any other EF DbContext and generate your own migrations, customize the persistance model, etc.

Note that DB options should still be of type `DbContextOptions<HistoryContext>` even when using a custom DbContext.

Example of a custom DbContext:

    public class TestSqlHistoryContext : SqlHistoryContext
    {
        public TestSqlHistoryContext(DbContextOptions<HistoryContext> options) : base(options)
        {
        }

        public override void InitializeDatabase()
        {
            // use simple db init
            Database.EnsureCreated();

            // this would run the built-in migrations
            //base.InitializeDatabase();
        }
    }

Using the custom DbContext:

    var historyStore = new EntityHistoryStore<MyCustomHistoryContext>(
        new EntityHistoryStoreSettings
        {
            DbOptions = new DbContextOptionsBuilder<HistoryContext>()
                .UseSqlServer(connectionString)
                .Options
        });

### Custom EF non-SQL History

If you wish to use a different EF database provider, you can create a context inheriting from `HistoryContext`.

In this case, you would only need to install the `NullDesk.Extensions.Mailer.History.EntityFramework` package.

### Custom (non-EF) History

You can also create a completely custom history store of your own using any technologies you like.

Simply create a class that inherits from `IHistoryStore<TSettings>` and implement the required members.

