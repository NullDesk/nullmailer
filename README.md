# NullDesk Email Extensions

## Overview

Email extension service packages for quickly integrating common mail scenarios into any .Net Core or .Net project using a variety of message delivery frameworks and cloud service providers.

Easily configure your application for different email services at startup based on environment, deployment configuration, or runtime detection of available mail providers.

## Status

|                                   |   |   |
|-----------------------------------|:-:|:-:|
|Project  [Issue Board](https://github.com/NullDesk/NullMailer/issues#boards?repos=79507993)|[![Build status](https://ci.appveyor.com/api/projects/status/5uc95cb6xho4qtdh/branch/master?svg=true)](https://ci.appveyor.com/project/StephenRedd/nullmailer/branch/master)|[![ZenHub](https://img.shields.io/badge/Shipping_faster_with-ZenHub-5e60ba.svg?style=flat-square)](https://github.com/NullDesk/NullMailer/issues#boards?repos=79507993)|
|NullDesk.Extensions.Mailer                                                                 |[![MyGet](https://img.shields.io/myget/nulldesk-ci/vpre/NullDesk.Extensions.Mailer.Core.svg)](https://www.myget.org/feed/nulldesk-ci/package/nuget/NullDesk.Extensions.Mailer.Core)|[![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.Core.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.Core/)|
|NullDesk.Extensions.Mailer.MailKit                                                         |[![MyGet](https://img.shields.io/myget/nulldesk-ci/vpre/NullDesk.Extensions.Mailer.MailKit.svg)](https://www.myget.org/feed/nulldesk-ci/package/nuget/NullDesk.Extensions.Mailer.MailKit)|[![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.MailKit.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.MailKit/)|
|NullDesk.Extensions.Mailer.SendGrid                                                        |[![MyGet](https://img.shields.io/myget/nulldesk-ci/vpre/NullDesk.Extensions.Mailer.SendGrid.svg)](https://www.myget.org/feed/nulldesk-ci/package/nuget/NullDesk.Extensions.Mailer.SendGrid)|[![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.SendGrid.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.SendGrid/)|
|NullDesk.Extensions.Mailer.History.EntityFramework                                         |[![MyGet](https://img.shields.io/myget/nulldesk-ci/vpre/NullDesk.Extensions.Mailer.History.EntityFramework.svg)](https://www.myget.org/feed/nulldesk-ci/package/nuget/NullDesk.Extensions.Mailer.History.EntityFramework)|[![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.History.EntityFramework.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.History.EntityFramework/)|
|NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer                               |[![MyGet](https://img.shields.io/myget/nulldesk-ci/vpre/NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.svg)](https://www.myget.org/feed/nulldesk-ci/package/nuget/NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer)|[![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer/)|

## Contents

- [Features](#features)
- [Package Descriptions](#pacakge-descriptions)
- [Basic Usage](#basic-usage)
  - [Mailer Instantiation](#mailer-instantitaion)
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
  - [Substitutions and PersonalizedSubstitutions](#subs)
  - [Body Content](#bodycontent)
  - [Templates](#tempaltes)
  - [Attachments](#attachments)
- [Creating your own Mailer](#custom-mailer)

## <a name="features"></a>Features

- Template support for all mailers
  - local filesystem body templates for mail services without their own
  - native templates for mail services that support them
- Replacement variables (substitutions) for message subject and content
  - works with explicitly defined message bodies, as well as templates
  - support message-wide and per-recipient variables both     
- Optional Fluent Message Builder API 
- Cross platform Netstandard 1.3 packages
  - Compatible with classic .Net and .Net core applicaitons.
- Uses MS Options extensions for simplified and flexible configuration  
- Optional supports for Microsoft logging extensions 
- Use with or without DI frameworks
  - Build-in extensions for Microsoft's DI extensions
  - Factory for cases where DI isn't desired or needed  
- Sample applications demonstrating common scenarios
- Editor support for Visual Studio Code and Visual Studio 2017
- Optional message and delivery history store

## <a name="pacakge-descriptions"></a>Package Descriptions

|                                                                                  |           |
|----------------------------------------------------------------------------------|-----------|
|NullDesk.Extensions.Mailer.Core                             |Base classes and interfaces for the mailer extensions, and settings.|
|NullDesk.Extensions.Mailer.MailKit                          |SMTP Relay Email service using the popular cross platform [MailKit library](https://github.com/jstedfast/MailKit). Includes support for basic Email Template files.|
|NullDesk.Extensions.Mailer.SendGrid                         |SendGrid email service using SendGrid APIs. Supports basic usage of SendGrid templates; can be inherited for more advanced SendGrid functionality.|
|NullDesk.Extensions.Mailer.History.EntityFramework          |Base classes for message and delivery history using entity framework |
|NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer|Implemenation of message and delivery history using entity framework targeting for MS SQL Server |
|_NullDesk.Extensions.Mailer.NetMail_                         |*(coming soon)* SMTP Relay Email service using the cross-platform System.Net.Mail framework from Microsoft.|


## <a name="basic-usage"></a>Basic Usage

The first step is to reference the pacakge or pacakges that your application needs:

For SMTP Email Reference <code>NullDesk.Extensions.Mailer.MailKit</code>.
For SendGrid Reference <code>NullDesk.Extensions.Mailer.MailKit</code>.
And if you want History, reference <code>NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer</code>.

Once you have the packages, usage follows these steps:

- Obtain a mailer instance (from DI, the factory, or by instantiating one yourself)
- Add one or more Messages to the Mailer instance.
- Once you have added all the messages you wish to deliery to the Mailer, simply call the SendAllAsync method. 

> The built-in Mailers are reusable by default, but it is recommended to create a new instance each time instead. Since the mailer will continue to track previously delivered items in memory, disposing of the mailer instance after each message (or batch) is preferrable. 

### <a name="mailer-instantitaion"></a>Mailer Instantiation

The simplest usage is to just instantiate the mailer of your choice, use the fluent message builder API to create a message, then send it:

         var settings = new SendGridMailerSettings
         {
             ApiKey = "123",
             FromDisplayName = "Person Name",
             FromEmailAddress = "someone@toast.com",
             IsSandboxMode = false
         };
         using (var mailer = new SendGridMailer(new OptionsWrapper<SendGridMailerSettings>(settings)))
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

In the above example, the settings are supplied via the <code>OptionsWrapper&lt;T&gt;</code> class; but if your application is using the Microsoft Options Extensions for configuration, you can use IOptions, IOptionsSnapshot, etc. for more advanced control of runtime settings.

### <a name="mailer-factory-usage"></a>Mailer Factory Usage

For most real-world scenarios, you would use dependency injection or the provided factory to obtain mailer instanaces, without having to supply all the constructor parameters each time.

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

The mailer extensions support logging via the ILogger interface from [Microsoft's Logging Extensions](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging). While designed for ASP.NET Core, the logging extensions can be used in any .Net Core project and Full-Framework .Net projects version 4.5 and higher. Additionally, the Logging extensions can be configured to interoperate with almost all popular logging frameworks for .Net --log4net, serilog, etc. 

All classes derived from the <code>Mailer</code> class take an optional <code>ILogger</code> or <code>ILogger&lt;T&gt;</code> constructor parameter, as do the extensions for registering the Mailer with the MailerFactory.

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

If a logger isn't supplied, the framework will automatically use the <code>Microsoft.Extensions.Logging.Abstractions.NullLogger</code> instance.

### <a name="history"></a>(optional) Message History Store

The mailer extensions also support recording mesages and their delivery details in an optional message history store. Depending on which history package you select, configuration may be diffrent. 


#### <a name="sqlhistory"></a>Setup History using EntityFramework and SQL Server

To store history in SQL server using Entity Framework 7, add the <code>NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer</code> NuGet package.

You will then need to create a DbContext inheriting from <code>SqlHistoryContext</code>.

       public class MySqlHistoryContext: SqlHistoryContext
        {
            public MySqlHistoryContext(DbContextOptions options) : base(options)
            {
            }
        }

You can override any members or provide alternate constructors if you desire. Also, you can treat this as any EF DbContext and generate migrations, customize the persistance model using the module builder, etc. 

Once you have your DbContext, you can then create a  <code>EntityHistoryStore&lt;TContext&gt;</code> and pass it to your <code>IMailer</code>. The Mailer will take care of recording message delivery history automatically. The examples in the next section demonstrate usage.


#### <a name="ihistorystore"></a>Delivery History with IHistoryStore

Creating and using the History store features is similar to using the logging support described above. You just need to pass in an <code>IHistoryStore</code>.

The below examples demonstrate usage with the EntityFramework SQL Server History Store pacakge, but usage will be similar for any IHistoryStore implementation.

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

When using the provided EF hsitory store, it is up to your if your client application will manage the database by calling EF migrations in code, of if you want to handle running migrations as part of your deployment process. If you want to do this in code, simple run the migration during your application's initilization.

        var builder = new DbContextOptionsBuilder<MySqlHistoryContext>()
                        .UseSqlServer(connectionString);

        using (var ctx = new MySqlHistoryContext(builder.options))
        {
            ctx.Database.Migrate();
        }
####<a name="resend"></a> Re-Sending from the IHistoryStore

The history store also enables re-send capabilities within the mailers.

If a history store is configured, use the history store to get the ID of the message you want to re-send. Then call the mailer's <code>ReSendAsync</code> method, passing in the the message Id you wish to resend.

The re-send will **immediately attempt** to re-deliver the message, but will not deliver any other messages being tracked by the mailer.

**Limitations**

- By default, messages with attachments cannot be resent.

Since the default setting for for most implementations of <code>IHistoryStore</code> is to ignore the attachment file contents, the complete message is not available for re-delivery.

You can enable attachment serialization for the history store by explicitly changing this setting in code.

     myHistoryStore.SerializeAttachments = true;


## <a name="creating-messages"></a>Creating MailerMessages

Emails are represented by a class called <code>MailerMessage</code>.

While you have a lot of flexibility in how messages are constructed, there are some requirments a message must meet before it can be successfully delivered. It must have a from address, at least one recipient, and specify either body content or a template.

Before attempting to send a message, you can check the <code>IsDeliverable</code> property to make sure.

### <a name="message-builder"></a>Fluent Message Builder

The mailer extensions include a fluent message builder API that provides a guided experience for creating complete and deliverable messages.

The main steps in the message builder are:

1. Sender
1. Subject
1. Recipient(s)
1. Body
1. Substitution(s) (optional)
1. Attachment(s) (optional)

Once you reach the body step of the builder, you can call the <code>Build()</code> method to create your message and end the builder method chain.

There are two primary ways to use the message builder. Either instantiate it yourself, or use the <code>CreateMessage()</code> method of your Mailer instance.

Minimal Example using <code>MessageBuilder</code> directly:

        var message = new MessageBuilder()
            .From("toast@toast.com")
            .And.Subject("Subject")
            .And.To("someone@somewhere.com")
            .And.ForTemplate("MyTemplate")
            .Build()

If using <code>CreateMessage()</code>, the sender details will be provided by your mailer's settings, and the bulder will begin with the subject step.

Minimal Example using <code>CreateMessage()</code>

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
            .And.WithSubstitution("%advertisemtn%", adText)
            .And.WithSubstitutions(new Dictionary<string, string>
                {
                    {"%company%", "Place Name"},
                    { "%phone%","1-800-123-1234" }
                })
            .Build();


### <a name="fluent-extensions"></a>Fluent Extensions

The Mailer Extensions also includes a more traitional set of fluent extension methods that can sometimes be useful in creating messages.

While these are mostly intended for internal use in the fluent message builder, they are available for your convienience if you choose to use them. They are often handy for modifying a message after it has been created by another mechanism.

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

While the message builder fluent API is probably the best way to create a new message, direct instantation is also straight-forward:

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

### <a name="subs"></a>Substitutions and PersonalizedSubstitutions

The mailer extensions support content substitutions for the body content, subject, and with templates. If your mail service supports server-side templates, these substitions will be passed to the server for processing against the template. When providing your own message body in code, the substitutions will be made locally when the message is delivered.

The <code>MailMessage</code> has a top-level property for supplying the collection of replacement tokens and values as a dictionary. Additionally, each recipient can also specify a collection of substitions to further personalize the content on a per-recipient basis. When the message is delivered, these collections are merged. If the same token exists in both collections, the recipient's personalized version will be used.

Several of the above examples for creating messages demonstrate using substitions.

**Be sure your dictionary's key, the replacement token, includes the delimeters your content or template uses.**

### <a name="bodycontent"></a>Body Content

The <code>MailerMessage.Body</code> can be any type that implements <code>IMessageBody</code>. 

The framework includes a <code>ContentBody</code> class for cases where you wish to supply HTML and/or Plain text content directly. The <code>TemplateBody</code> class allows you to supply a template name instead. 

### <a name="templates"></a>Templates

Differnt implementations of <code>IMailer</code> support templates in slightly different ways.

The <code>MkSmtpMailer</code> uses MailKit to deliver email using traditional SMTP services. To support templates, this mailer can use templates stored on the local file-system. Templates can be supplied for both HTML and Plain text content. Just before the message is delivered, the templates will be loaded, and the substitions will be performed. The body's TempalteName should be set to the filename of the template(s) you wish to use, minus the file extension. The folder path and file extensions used to locate the template files on the file system are supplied by the mailer's settings.

The <code>SendGridMailer</code> uses SendGrid's server-side transactional templates. Here the template name should be the ID of the transactional template you wish to use. The mailer will simply pass the template name and substitutions collections to the server, which will then create the message and deliver it.

### <a name="attachments"></a>Attachments

Attachments can added to a <code>MailMessage</code> as either a collection of file names, or using Dictionery of file names and Streams. When using file names, please use the full file path of the attachment.

## <a name="custom-mailer"></a>Creating your own mailers

To implement your own mailer, simply implement a class that inherits <code>Mailer&lt;TSettings&gt;</code>.

For the above interfaces, <code>TSettings</code> is a custom class that implements <code>IMailerSettings</code>, then adds any custom configuration properties your mailer or the underlying mail framework needs.

        public class MyMailerSettings : IMailerSettings
        {
            public string FromEmailAddress { get; set; }

            public string FromDisplayName { get; set; }

            //... any other properties your mailer needs
        }

Every effort has been made to keep inheritance of <code>Mailer&lt;TSettings&gt;</code> fairly straight forward.

The primary method you must supply is a method that can deliver a single <code>DeliveryItem</code> using the email service your mailer supports. The method should return the DeliveryItem if it was successfully sent, or throw an exception if something went wrong. The base class will handle logging the exception, updating the DeliveryItem's properties, recording the delivery attempt to the history store, etc. All you have to do is send the email via the mail service, and either throw an exception or return the deliery item if it was sent.

        public class MySimpleMailer : Mailer<MyMailerSettings>
        {
            protected override async Task<string> DeliverMessageAsync(
                DeliveryItem deliveryItem,
                CancellationToken token = default(CancellationToken))
            {
                //... implementation here
                // return provider supplied message ID, or null if not applicable
            }
        }


