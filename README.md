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



> The built-in Mailers are reusable by default, but it is recommended to create a new instance each time instead. Since the mailer will continue to track previously delivered items in memory, disposing of the mailer instance after each message (or batch) is preferable.

### <a name="mailer-instantiation"></a>Mailer Instantiation

The simplest usage is to instantiate the mailer of your choice, use the fluent message builder API to create a message, then send it:

         var settings = new SendGridMailerSettings
         {
             ApiKey = "123",
             FromDisplayName = "Person Name",
             FromEmailAddress = "someone@toast.com",
             IsSandboxMode = false
         };
         using (var mailer = new SendGridMailer(
                  new OptionsWrapper<SendGridMailerSettings>(settings)))
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

Using the mailer factory, you can configure the mailer once in startup:

        var settings = new SendGridMailerSettings
        {
            ApiKey = "123",
            FromDisplayName = "Person Name",
            FromEmailAddress = "someone@toast.com",
            IsSandboxMode = false
        };

        var factory = new MailerFactory();
        factory.AddSendGridMailer(sendGridSettings); 

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

When using dependency injection frameworks, it is best to register the mailers to be transient instances. This example demonstrates this using Microsoft's own dependency injection extensions:

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddOptions();
            services.Configure<SendGridMailerSettings>(s =>
            {
                    s.ApiKey = "123";
                    s.FromDisplayName = "Person Name";
                    s.FromEmailAddress = "someone@toast.com";
                    s.IsSandboxMode = false;
            });

            services.AddTransient<IMailer, SendGridMailer>();

            return services.BuildServiceProvider();
        }

### <a name="ilogger"></a>(optional) Logging with ILogger

The mailer extensions support logging via the ILogger interface from [Microsoft's Logging Extensions](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging). While designed for ASP.NET Core, the logging extensions can be used in any .Net Core project and Full-Framework .Net projects version 4.5 and higher. Additionally, the Logging extensions can be configured to operate with almost all popular logging frameworks for .Net --log4net, serilog, etc.

All classes derived from the `Mailer` class take an optional `ILogger` or `ILogger<T>` constructor parameter, as do the extensions for registering the Mailer with the MailerFactory.

Example using the MailerFactory:

        var loggerFactory = new LoggerFactory();
        //configure your desired logging providers
        loggerFactory.AddConsole(consoleLoggerConfig);

        var mailerFactory = new MailerFactory();
        mailerFactory.AddSendGridMailer(
            sendGridSettings,
            loggerFactory.CreateLogger<SendGridMailer>());

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
                    s.IsSandboxMode = false;
            });

            services.AddTransient<IMailer, SendGridMailer>();

            return services.BuildServiceProvider();
        }


Example when instantiating a Mailer directly:

        var loggerFactory = new LoggerFactory();

        //configure your desired logging providers
        loggerFactory.AddConsole(consoleLoggerConfig);

        using (var mailer = new SendGridMailer(
            new OptionsWrapper<SendGridMailerSettings>(settings),
            loggerFactory.CreateLogger<SendGridMailer>()))
        {
            //stuff
        }

If a logger isn't supplied, the framework will automatically use the `Microsoft.Extensions.Logging.Abstractions.NullLogger` instance.

### <a name="history"></a>(optional) Message History Store

The mailer extensions also support recording messages and their delivery details in an optional message history store. Depending on which history package you select, configuration may be different.


#### <a name="sqlhistory"></a>Setup History using EntityFramework and SQL Server

To store history in SQL server using Entity Framework 7, add the `NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer` NuGet package.

You will then need to create a DbContext inheriting from `SqlHistoryContext`.

       public class MySqlHistoryContext: SqlHistoryContext
        {
            public MySqlHistoryContext(DbContextOptions options) : base(options)
            {
            }
        }

You can override any members or provide alternate constructors if you desire. Also, you can treat this as any EF DbContext and generate migrations, customize the persistance model using the module builder, etc. 

Once you have your DbContext, you can then create a  `EntityHistoryStore<TContext>` and pass it to your `IMailer`. The Mailer will take care of recording message delivery history automatically. The examples in the next section demonstrate usage.


#### <a name="ihistorystore"></a>Delivery History with IHistoryStore

Creating and using the History store features is similar to using the logging support described above. You just need to pass in an `IHistoryStore`.

The below examples demonstrate usage with the EntityFramework SQL Server History Store package, but usage will be similar for any IHistoryStore implementation.

Example using the MailerFactory:

        var loggerFactory = new LoggerFactory();
        //configure your desired logging providers
        loggerFactory.AddConsole(consoleLoggerConfig);

        var builder = new DbContextOptionsBuilder<MySqlHistoryContext>()
                        .UseSqlServer(connectionString);

        var mailerFactory = new MailerFactory();
        mailerFactory.AddSendGridMailer(
            sendGridSettings,
            loggerFactory.CreateLogger<SendGridMailer>(),
            new EntityHistoryStore<MySqlHistoryContext>(builder.Options));

Example when using MS DI extensions:

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();

            services.AddOptions();

            services.AddSingleton<DbContextOptions>(s =>
            {
                var builder = new DbContextOptionsBuilder<MySqlHistoryContext>()
                    .UseSqlServer(connectionString);
                return builder.Options;
            });
            services.AddSingleton<IHistoryStore, EntityHistoryStore<MySqlHistoryContext>>();

            services.Configure<SendGridMailerSettings>(s =>
            {
                    s.ApiKey = "123";
                    s.FromDisplayName = "Person Name";
                    s.FromEmailAddress = "someone@toast.com";
                    s.IsSandboxMode = false;
            });

            services.AddTransient<IMailer, SendGridMailer>();

            return services.BuildServiceProvider();
        }

Example when instantiating a Mailer directly:

        var loggerFactory = new LoggerFactory();

        //configure your desired logging providers
        loggerFactory.AddConsole(consoleLoggerConfig);

        var builder = new DbContextOptionsBuilder<MySqlHistoryContext>()
                        .UseSqlServer(connectionString);


        using (var mailer = new SendGridMailer(
            new OptionsWrapper<SendGridMailerSettings>(settings),
            loggerFactory.CreateLogger<SendGridMailer>(),
            new EntityHistoryStore<MySqlHistoryContext>(builder.Options)))
        {
            //stuff
        }

When using the provided EF history store, it is up to your if your client application will manage the database by calling EF migrations in code, of if you want to handle running migrations as part of your deployment process. If you want to do this in code, simple run the migration during your application's initialization.

        var builder = new DbContextOptionsBuilder<MySqlHistoryContext>()
                        .UseSqlServer(connectionString);

        using (var ctx = new MySqlHistoryContext(builder.options))
        {
            ctx.Database.Migrate();
        }

#### <a name="resend"></a> Re-Sending from the IHistoryStore

The history store also enables re-send capabilities within the mailers.

If a history store is configured, use the history store to get the ID of the message you want to re-send. Then call the mailer's `ReSendAsync` method, passing in the the message Id you wish to resend.

The re-send will **immediately attempt** to re-deliver the message, but will not deliver any other messages being tracked by the mailer.

**Limitations**

- By default, messages with attachments cannot be resent.

Since the default setting for for most implementations of `IHistoryStore` is to ignore the attachment file contents, the complete message is not available for re-delivery.

You can enable attachment serialization for the history store by explicitly changing this setting in code.

     myHistoryStore.SerializeAttachments = true;


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

If using `CreateMessage()`, the sender details will be provided by your mailer's settings, and the builder will begin with the subject step.

Minimal Example using `CreateMessage()`

        mailer.CreateMessage(b => b
            .Subject("Subject")
            .And.To("someone@somewhere.com")
            .And.ForTemplate("MyTemplate")
            .Build()

A Complete Example:

        var message = new MessageBuilder()
            .From("toast@toast.com").WithDisplayName("Toast Man")
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

The Mailer Extensions also includes a more traditional set of fluent extension methods that can sometimes be useful in creating messages.

While these are mostly intended for internal use in the fluent message builder, they are available for your convenience if you choose to use them. They are often handy for modifying a message after it has been created by another mechanism.

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

While the message builder fluent API is probably the best way to create a new message, direct instantiation is also straight-forward:

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

Each `MailMessage` is converted into one `DeliveryItem`. When sending, each delivery item is sent as a unique email. If a history store is used, each `DeliveryItem` has its own status and history.

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


