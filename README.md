# NullDesk Email Extensions

## Overview

Email extensions for quickly integrating common mail scenarios into any .Net Core or .Net Full-Framework project.

Easily configure your application for different email services at startup based on environment, deployment configuration, or custom detection of available mail providers.

## Status

|                                   |   |   |
|-----------------------------------|:-:|:-:|
|Project  [Issue Board](https://github.com/NullDesk/NullMailer/issues#boards?repos=79507993)|[![Build status](https://ci.appveyor.com/api/projects/status/5uc95cb6xho4qtdh/branch/master?svg=true)](https://ci.appveyor.com/project/StephenRedd/nullmailer/branch/master)|[![ZenHub](https://img.shields.io/badge/Shipping_faster_with-ZenHub-5e60ba.svg?style=flat-square)](https://github.com/NullDesk/NullMailer/issues#boards?repos=79507993)|
|NullDesk.Extensions.Mailer.Core                                                             |[![MyGet](https://img.shields.io/myget/nulldesk-ci/vpre/NullDesk.Extensions.Mailer.Core.svg)](https://www.myget.org/feed/nulldesk-ci/package/nuget/NullDesk.Extensions.Mailer.Core)|[![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.Core.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.Core/)|
|NullDesk.Extensions.Mailer.MailKit                                                         |[![MyGet](https://img.shields.io/myget/nulldesk-ci/vpre/NullDesk.Extensions.Mailer.MailKit.svg)](https://www.myget.org/feed/nulldesk-ci/package/nuget/NullDesk.Extensions.Mailer.MailKit)|[![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.MailKit.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.MailKit/)|
|NullDesk.Extensions.Mailer.SendGrid                                                        |[![MyGet](https://img.shields.io/myget/nulldesk-ci/vpre/NullDesk.Extensions.Mailer.SendGrid.svg)](https://www.myget.org/feed/nulldesk-ci/package/nuget/NullDesk.Extensions.Mailer.SendGrid)|[![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.SendGrid.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.SendGrid/)|
|NullDesk.Extensions.Mailer.History.EntityFramework                                         |[![MyGet](https://img.shields.io/myget/nulldesk-ci/vpre/NullDesk.Extensions.Mailer.History.EntityFramework.svg)](https://www.myget.org/feed/nulldesk-ci/package/nuget/NullDesk.Extensions.Mailer.History.EntityFramework)|[![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.History.EntityFramework.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.History.EntityFramework/)|
|NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer                               |[![MyGet](https://img.shields.io/myget/nulldesk-ci/vpre/NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.svg)](https://www.myget.org/feed/nulldesk-ci/package/nuget/NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer)|[![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer/)|

## Contents

- [Features](#features)
- [Package Descriptions](#package-descriptions)
- [Basic Usage](#basic-usage)
  - [Mailer Instantiation](#mailer-instantiation)
  - [Mailer Factory Usage](#mailer-factory-usage)
  - [Dependency Injection](#dependency-injection)
  - [Logging with ILogger](#ilogger)
  - [Storing Message History](#history)
    - [Setup History with EntityFramework and SQL Server](#sqlhistory)
    - [Delivery History with IHistoryStore](#ihistorystore)
    - [Re-Sending from IHistoryStore](#resend)
- [Creating MailerMessages](#creating-messages)
  - [Fluent Message Builder](#message-builder)
  - [Fluent Extensions](#fluent-extensions)
  - [Class Instantiation](#class-instantiation)
  - [Multiple Recipients](#recipients)
  - [Substitutions and PersonalizedSubstitutions](#subs)
  - [Body Content](#bodycontent)
  - [Templates](#templates)
  - [Attachments](#attachments)
- [Using GMail](#gmail)
- [Creating your own Mailer](#custom-mailer)

## <a name="features"></a>Features

- Templates for all mail services
- Replacement variables (substitutions) with or without templates
- Cross platform Netstandard 1.3 packages
- Fluent Message Builder API
- Delivery history store
- Optional support
  - Microsoft logging extensions
  - Microsoft options extensions
  - Microsoft dependency injection extensions

## <a name="pacakge-descriptions"></a>Package Descriptions

|                                                                                  |           |
|----------------------------------------------------------------------------------|-----------|
|NullDesk.Extensions.Mailer.Core                             |Base classes and interfaces for the mailer extensions, and settings.|
|NullDesk.Extensions.Mailer.MailKit                          |SMTP Relay Email service using the popular cross platform [MailKit library](https://github.com/jstedfast/MailKit). Includes support for basic Email Template files.|
|NullDesk.Extensions.Mailer.SendGrid                         |SendGrid email service using SendGrid APIs. Supports basic usage of SendGrid templates; can be inherited for more advanced SendGrid functionality.|
|NullDesk.Extensions.Mailer.History.EntityFramework          |Base classes for message and delivery history using entity framework |
|NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer|Implementation of message and delivery history using entity framework targeting for MS SQL Server |
|_NullDesk.Extensions.Mailer.NetMail_                         |*(coming soon)* SMTP Relay Email service using the cross-platform System.Net.Mail framework from Microsoft.|


## <a name="basic-usage"></a>Basic Usage

- Reference the package or packages your application needs:
  - For SMTP install `NullDesk.Extensions.Mailer.MailKit`.
  - For SendGrid install `NullDesk.Extensions.Mailer.MailKit`.
  - For SQL history install `NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer`.
- Obtain a mailer instance from DI, the mailer factory, or by instantiating one yourself
- Add one or more Messages to the Mailer with `AddMessage` or `CreateMessage` methods.
- Finally, call the `SendAllAsync` method.



> The built-in Mailers are reusable by default. As of 3.3.0, mailers no longer keep an internal reference to delivered items, but it is still recommended to create a new mailer instance each time.

### <a name="mailer-instantiation"></a>Mailer Instantiation

The simplest usage is to instantiate the mailer of your choice, use the fluent message builder API to create a message, then send it:

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

             var deliveryItem = await mailer.SendAllAsync(CancellationToken.None);
             if(deliveryItem.IsSuccess)
             {
                 //happy dance!
             }
         }

In the above example, the settings are supplied via the `OptionsWrapper<T>` class, but if your application is using the Microsoft Options Extensions for configuration, you can use IOptions, IOptionsSnapshot, etc. for more advanced control of runtime settings.

### <a name="mailer-factory-usage"></a>Mailer Factory Usage

For most real-world scenarios, you would use dependency injection or the provided factory to obtain mailer instances, without having to supply all the constructor parameters each time.

There are several extensions and overloads for working with the `MailerFactory`, these samples only show the more common usages.

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

Then anytime you need to send mail, just grab a new mailer instance from the factory and go:

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

            var deliveryItem = await mailer.SendAllAsync(CancellationToken.None);
            if(deliveryItem.IsSuccess)
            {
                //happy dance!
            }
        }

### <a name="dependency-injection"></a>Dependency Injection

When using any dependency injection framework, it is best to register types as follows:

- Mailers should be registered as `IMailer` with a transient or limited scope.
- Mailer and History settings should usually be registered as their actual type, with a singleton lifecycle.
- History Stores should be registered as `IHistoryStore` with a singleton lifecyle
  - History stores control the lifecycle of any associated `DbContext` automatically.

When using the Microsoft DI framework, there are numerous extension methods to assist in registering and setting up mailers, settings, and the optional loggers and history stores:

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSendGridMailer(new SendGridMailerSettings
            {
                    ApiKey = "123",
                    FromDisplayName = "Person Name",
                    FromEmailAddress = "someone@toast.com",
                    ReplyToEmailAddress = "someoneFriendly@toast.com", //optional
                    ReplyToDisplayName = "Reply to Person Name", //optional
                    IsSandboxMode = false
            });

            return services.BuildServiceProvider();
        }

### <a name="ilogger"></a>(optional) Logging with ILogger

The mailer extensions support logging via the ILogger interface from [Microsoft's Logging Extensions](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging). While designed for ASP.NET Core, the logging extensions can be used in any .Net Core project and Full-Framework .Net projects version 4.5 and higher. Additionally, the Logging extensions can be configured to operate with almost all popular logging frameworks for .Net --log4net, serilog, etc.

All classes derived from the `Mailer` class take an optional `ILogger` or `ILogger<T>` constructor parameter, as do the extensions for registering the Mailer with the MailerFactory.

Example using the MailerFactory:

        var mailerFactory = new MailerFactory(){
        {
            DefaultLoggerFactory = new LoggerFactory()
                .AddConsole(consoleLoggerConfig)
        };

        mailerFactory.AddSendGridMailer(new SendGridMailerSettings{
            ApiKey = "123",
            FromDisplayName = "Person Name",
            FromEmailAddress = "someone@toast.com",
            IsSandboxMode = false
        });

Example when using MS DI extensions:

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(); //that's all you need
            services.AddOptions();
            services.Configure<SendGridMailerSettings>(s =>
            {
                    s.ApiKey = "123";
                    s.FromDisplayName = "Person Name";
                    s.FromEmailAddress = "someone@toast.com";
                    s.ReplyToEmailAddress = "someoneFriendly@toast.com"; //optional
                    s.ReplyToDisplayName = "Reply to Person Name"; //optional
                    s.IsSandboxMode = false;
            });

            // gets settings from config options
            services.AddSendGridMailer(s => s.GetService<IOptions<SendGridMailerSettings>>().Value);

            return services.BuildServiceProvider();
        }


Example when instantiating a Mailer directly:

        var loggerFactory = new LoggerFactory()
            .AddConsole(consoleLoggerConfig);

        using (var mailer = new SendGridMailer(
            settings,
            loggerFactory.CreateLogger<SendGridMailer>()))
        {
            //stuff
        }

If a logger isn't supplied, the framework will automatically use the `Microsoft.Extensions.Logging.Abstractions.NullLogger` instance.

### <a name="history"></a>(optional) Message History Store

The mailer extensions also support recording messages and their delivery details in an optional message history store. Depending on which history package you select, configuration may be different.

The core package includes a `NullHistoryStore` (does nothing) and an `InMemoryHistoryStore`, which is usually used for testing purposes.

#### <a name="sqlhistory"></a>Setup History using EntityFramework and SQL Server

To store history in SQL server using Entity Framework 7, add the `NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer` NuGet package.

There are two settings types you can choose from. The simplified `SqlEntityHistoryStoreSettings` that can be serialized easily to or from JSON (good when using .NET Core configuration), or `EntityHistoryStoreSettings` which requires a `DbContextOptions<HistoryContext>` value --harder to serialize, but offers the full range of EF context options.

Either type of settings can be used.

The standard SQL history store with `SqlEntityHistoryStoreSettings`:

    var historyStore = new EntityHistoryStore<SqlHistoryContext>(
        new SqlEntityHistoryStoreSettings()
        {
            ConnectionString = connectionString
        });

Alternate SQL history instantiation with `EntityHistoryStoreSettings`:

    var historyStore = new EntityHistoryStore<SqlHistoryContext>(
        new EntityHistoryStoreSettings
        {
            DbOptions = new DbContextOptionsBuilder<HistoryContext>()
                .UseSqlServer(connectionString)
                .Options
        });

You can also create your own DbContext type, derived from `SqlHistoryContext`, and override any members you wish. You can treat this as any other EF DbContext and generate your own migrations, customize the persistance model using the module builder, etc.

Note that the Db options should still be of type `DbContextOptionsBuilder<HistoryContext>` even when using a custom context type.

Example of a custom DbContext:

    public class TestSqlHistoryContext : SqlHistoryContext
    {
        public TestSqlHistoryContext(DbContextOptions<HistoryContext> options) : base(options)
        {
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

You could also inherit from `HistoryContext` directly if you wanted to use another EF database provider framework instead of SQL server. In this case, you only need to reference the `NullDesk.Extensions.Mailer.History.EntityFramework` package.

#### <a name="ihistorystore"></a>Delivery History with IHistoryStore

Creating and using the History store features is similar to using the logging support described above. You just need to pass in an `IHistoryStore`.

The below examples demonstrate usage with the EntityFramework SQL Server History Store package, but usage will be similar for any IHistoryStore implementation. 

Example using the MailerFactory:

        var mailerFactory = new MailerFactory(){
        {
            DefaultLoggerFactory = new LoggerFactory()
                .AddConsole(consoleLoggerConfig)
        };

        fact.RegisterDefaultSqlHistory(new SqlEntityHistoryStoreSettings());

        mailerFactory.AddSendGridMailer(
            new SendGridMailerSettings{
                ApiKey = "123",
                FromDisplayName = "Person Name",
                FromEmailAddress = "someone@toast.com",
                IsSandboxMode = false
            }
        );

Example when using MS DI extensions:

        public IServiceProvider ConfigureServices(IServiceCollection services, IConfigurationRoot config)
        {
            services.AddLogging();

            services.AddOptions();

            //add settings from supplied configuration
            services.Configure<SqlEntityHistoryStoreSettings>(config.GetSection("MailHistoryDbSettings"));

            //get setting from options framework
            services.AddMailerSqlHistory(s => s.GetService<IOptions<SqlEntityHistoryStoreSettings>>().Value);

            //add new settings to configuration
            services.Configure<SendGridMailerSettings>(s =>
            {
                    s.ApiKey = "123";
                    s.FromDisplayName = "Person Name";
                    s.FromEmailAddress = "someone@toast.com";
                    s.IsSandboxMode = false;
            });

            //get setting from options framework
            services.AddSendGridMailer(s => s.GetService<IOptions<SendGridMailerSettings>>().Value);

            return services.BuildServiceProvider();
        }

Example when instantiating a Mailer directly:

        //optional: configure your desired logging providers
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

#### <a name="resend"></a> Re-Sending from the IHistoryStore

The history store also enables re-send capabilities within the mailers.

If a history store is configured, use the history store to get the ID of the message you want to re-send. Then call the mailer's `ReSendAsync` method, passing in the the message Id you wish to resend.

The re-send will **immediately attempt** to re-deliver the message, but will not deliver any other messages being tracked by the mailer.

**Limitations**

By default, messages with attachments cannot be resent. Since the default setting for for most implementations of `IHistoryStore` is to ignore the attachment file contents, the complete message is not available for re-delivery.

You can enable attachment serialization and storage for the history store in the settings by changing the `StoreAttachmentContents` value to `true`.

## <a name="creating-messages"></a>Creating MailerMessages

Emails are represented by a class called `MailerMessage`.

While you have a lot of flexibility in how messages are constructed, there are some requirements a message must meet before it can be successfully delivered. It must have a from address, at least one recipient, and specify either body content or a template.

Before attempting to send a message, you can check the `IsDeliverable` property to make sure.

### <a name="message-builder"></a>Fluent Message Builder

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


### <a name="fluent-extensions"></a>Fluent Extensions

The Mailer Extensions also includes a more traditional set of fluent extension methods.

With these extensions, the order in which you chain the methods is largely irrelevant.

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


### <a name="class-instantiation"></a>Class Instantiation

While the message builder fluent API is usually best, direct instantiation is also straight-forward:

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
                    DisplayName = "Nice Guy"
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
                {"%name%", "Nice Guy"},
                {"%title%", "President"}
            }
        };

### <a name="recipients"></a>Multiple Recipients

A `MailMessage` may define multiple recipients, each potentially having their own personalized substitutions. The Mailer extensions will ensure that **each recipient receives a separate and isolated email** message.

Each `MailMessage` is converted into one or more `DeliveryItem` instances. When sending mail, each delivery item will be sent as a unique email. 

If a history store is used, then each `DeliveryItem` has its own history record.

### <a name="subs"></a>Substitutions and PersonalizedSubstitutions

The mailer extensions support content substitutions for the body content, subject, and with templates. If your mail service supports server-side templates, these substitutions will be passed to the server for processing against the template. When providing your own message body in code, the substitutions will be made locally when the message is delivered.

The `MailMessage` has a top-level property for supplying the collection of replacement tokens and values as a dictionary. Additionally, each recipient can also specify a collection of substitutions to further personalize the content on a per-recipient basis. When the message is delivered, these collections are merged. If the same token exists in both collections, the recipient's personalized version will be used.

Several of the above examples for creating messages demonstrate using substitutions.

**Be sure your dictionary's key, the replacement token, includes the delimiters your content or template uses.**

### <a name="bodycontent"></a>Body Content

The `MailerMessage.Body` can be any type that implements `IMessageBody`. 

The framework includes a `ContentBody` class for cases where you wish to supply HTML and/or Plain text content directly. The `TemplateBody` class allows you to supply a template name instead. 

### <a name="templates"></a>Templates

Different implementations of `IMailer` support templates in slightly different ways.

The `MkSmtpMailer` uses MailKit to deliver email using traditional SMTP services. To support templates, this mailer can use templates stored on the local file-system. Templates can be supplied for both HTML and Plain text content. Just before the message is delivered, the templates will be loaded, and the substitutions will be performed. The body's TemplateName should be set to the filename of the template(s) you wish to use, minus the file extension. The folder path and file extensions used to locate the template files on the file system are supplied by the mailer's settings.

The `SendGridMailer` uses SendGrid's server-side transactional templates. Here the template name should be the ID of the transactional template you wish to use. The mailer will simply pass the template name and substitutions collections to the server, which will then create the message and deliver it.

### <a name="attachments"></a>Attachments

Attachments can added to a `MailMessage` as either a collection of file names, or using Dictionary of file names and Streams. When using file names, please use the full file path of the attachment.


## <a name="gmail"></a>Using Gmail

If you are using GMail with the MailKit SMTP Mailer, you will need to either use the less secure username/password authentication, or you will need to obtain and use oAuth access tokens.

Review the [MailKit GMail documentation](https://github.com/jstedfast/MailKit/blob/master/FAQ.md#GMailAccess) first.

When configuring your settings for GMail, you will need to use the `MkSmtpBasicAuthenticationSettings` if you are using the less secure connection method, or use `MkSmtpAccessTokenAuthenticationSettings` if using oAuth. 

For oAuth, please note that you will need to ensure that you create new settings and a new mailer instance each time you want to send a mail (or a batch of messages), and it is up to the consuming application to make sure that the settings contain an up-to-date and valid access token each time. The mailer extensions will not validate the token, nor does it have any built-in mechanism to handle expired tokens. 


## <a name="custom-mailer"></a>Creating your own mailers

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


----


