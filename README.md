# NullDesk Email Extensions

## Overview
Email extension service packages for quickly integrating common mail scenarios into any .Net Core or .Net project using a variety of message delivery frameworks and cloud service providers.

## Status

|                                   |   |   |
|-----------------------------------|:-:|:-:|
|Project  [Issue Board](https://github.com/NullDesk/NullMailer/issues#boards?repos=79507993)|[![Build status](https://ci.appveyor.com/api/projects/status/5uc95cb6xho4qtdh/branch/master?svg=true)](https://ci.appveyor.com/project/StephenRedd/nullmailer/branch/master)|[![ZenHub](https://img.shields.io/badge/Shipping_faster_with-ZenHub-5e60ba.svg?style=flat-square)](https://github.com/NullDesk/NullMailer/issues#boards?repos=79507993)|
|NullDesk.Extensions.Mailer                                                                 |[![MyGet](https://img.shields.io/myget/nulldesk-ci/vpre/NullDesk.Extensions.Mailer.Core.svg)](https://www.myget.org/feed/nulldesk-ci/package/nuget/NullDesk.Extensions.Mailer.Core)|[![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.Core.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.Core/)|
|NullDesk.Extensions.Mailer.MailKit                                                         |[![MyGet](https://img.shields.io/myget/nulldesk-ci/vpre/NullDesk.Extensions.Mailer.MailKit.svg)](https://www.myget.org/feed/nulldesk-ci/package/nuget/NullDesk.Extensions.Mailer.MailKit)|[![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.MailKit.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.MailKit/)|
|NullDesk.Extensions.Mailer.SendGrid                                                        |[![MyGet](https://img.shields.io/myget/nulldesk-ci/vpre/NullDesk.Extensions.Mailer.SendGrid.svg)](https://www.myget.org/feed/nulldesk-ci/package/nuget/NullDesk.Extensions.Mailer.SendGrid)|[![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.SendGrid.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.SendGrid/)|
|NullDesk.Extensions.Mailer.History.EntityFramework                                         |[![MyGet](https://img.shields.io/myget/nulldesk-ci/vpre/NullDesk.Extensions.Mailer.History.EntityFramework.svg)](https://www.myget.org/feed/nulldesk-ci/package/nuget/NullDesk.Extensions.Mailer.History.EntityFramework)|[![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.History.EntityFramework.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.History.EntityFramework/)|
|NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer                               |[![MyGet](https://img.shields.io/myget/nulldesk-ci/vpre/NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.svg)](https://www.myget.org/feed/nulldesk-ci/package/nuget/NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer)|[![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer/)|



## Package Descriptions


|                                                                                  |           |
|----------------------------------------------------------------------------------|-----------|
|NullDesk.Extensions.Mailer                                  |Base classes and interfaces for the mailer extensions, and settings.|
|NullDesk.Extensions.Mailer.MailKit                          |SMTP Relay Email service using the popular cross platform [MailKit library](https://github.com/jstedfast/MailKit). Includes support for basic Email Template files.|
|NullDesk.Extensions.Mailer.SendGrid                         |SendGrid email service using SendGrid APIs. Supports basic usage of SendGrid templates; can be inherited for more advanced SendGrid functionality.|
|NullDesk.Extensions.Mailer.History.EntityFramework          |Base classes for message and delivery history using entity framework |
|NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer|Implemenation of message and delivery history using entity framework targeting for MS SQL Server |
|_NullDesk.Extensions.Mailer.NetMail_                         |*(coming soon)* SMTP Relay Email service using the cross-platform System.Net.Mail framework from Microsoft.|


## Features  

- Cross platform framework
- Netstandard 1.3 packages
  - Compatible with classic .Net and .Net core applicaitons.
- Microsoft options framework for easy configuration settings management
- Use with or without DI frameworks 
- Sample applications demonstrating common integration scenarios
- Editor support for Visual Studio Code or Visual Studio 2017
- Store message and delivery history

## Basic Usage

The simplest usage is to just instantiate the mailer of your choice and go:

         var settings = new SendGridMailerSettings
         {
             ApiKey = "123",
             FromDisplayName = "Person Name",
             FromEmailAddress = "someone@toast.com",
             IsSandboxMode = false
         };
         using (var mailer = new SendGridMailer(new OptionsWrapper<SendGridMailerSettings>(settings)))
         {
             var result = await mailer.SendMailAsync(
                 "recipient@toast.com",
                 "Recipient Name", 
                 "Message Subject", 
                 thig.GenerateHtmlBody(), 
                 "plain text body", 
                 CancellationToken.None);
         }

> The built-in Mailers are reusable by default; however, it is recommended to create a new instance each time instead. There is rarely a performance advantage to reuse, and some mail frameworks are less friendly toward reuse and thread safety. Also, some mailers (like the MailKit SMTP mailers) can suffer a performance penalty if multiple callers are trying to send mail in parallel using the same instance.

For most real-world scenarios though, you would either use a dependency injection or the provided mailer factory instead.

This next example shows registering a mailer with the built-in mailer factory, and with sending a simple template based message:

        var settings = new SendGridMailerSettings
        {
            ApiKey = "123",
            FromDisplayName = "Person Name",
            FromEmailAddress = "someone@toast.com",
            IsSandboxMode = false
        };

        var factory = new MailerFactory();
        factory.AddSendGridMailer(sendGridSettings);
        
        var replacementVars = new Dictionary<string, string>()
        replacementVars.Add("%name%", "Mr. Toast"); 
        
        var result = await
            factory.StandardMailer.SendMailAsync(
                "TemplateName",
                "recipient@toast.com",
                "Recipient Name", 
                "Message Subject", 
                ReplacementVars,
                CancellationToken.None);

The provided samples and unit tests illustrate more complex scenarios. 

## Creating your own mailers

To implement your own full-featured mailer, simply implement a class that implements <code>IStandardMailer&lt;TSettings&gt;</code>. 

If you don't care about templates, you can create a simple mailer by just implementing <code>ISimpleMailer&lt;TSettings&gt;</code>.

For the above interfaces, <code>TSettings</code> is a custom DTO class containing any configuration settings your mailer requires.

The NullDesk mailers include both a simple and full-featured mailer; the full-featured mailers inherit from a simple mailer as a base class, then extend it to add additional tempalate features from <code>IStandardMailer&lt;TSettings&gt;</code>. Here is an example of how that looks:

        public class MyMailerSettings : IMailerSettings
        {
            //... implementation here
        }
    
        public class MySimpleMailer : ISimpleMailer<MyMailerSettings>
        {
            //... implementation here
        }
    
        public class MyFullMailer : MySimpleMailer, IStandardMailer<MyMailerSettings>
        {
            //... implementation here
        }

When implementing your own mailers, there is no need to necessarily follow the same pattern. You could just as easily create a single class that handles all mail functions.

This example shows a class implementing all mailer functions, as well as the interface for message delivery history

        public class MyAmazingMailer : IStandardMailer<MyAmazingMailerSettings>, IHistoryMailer
        {
            //... implementation here
        } 

