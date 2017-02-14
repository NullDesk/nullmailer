# NullDesk Email Extensions

## Overview
Email extension service packages for quickly integrating common mail scenarios into any .Net Core or .Net project using a variety of message delivery frameworks and cloud service providers.

## Status

|                                   |   |   |
|-----------------------------------|:-:|:-:|
|Project  [Issue Board](https://github.com/NullDesk/NullMailer/issues#boards?repos=79507993)|[![Build status](https://ci.appveyor.com/api/projects/status/5uc95cb6xho4qtdh/branch/master?svg=true)](https://ci.appveyor.com/project/StephenRedd/nullmailer/branch/master)|[![ZenHub](https://img.shields.io/badge/Shipping_faster_with-ZenHub-5e60ba.svg?style=flat-square)](https://github.com/NullDesk/NullMailer/issues#boards?repos=79507993)|
|NullDesk.Extensions.Mailer                                                                 |[![MyGet](https://img.shields.io/myget/nulldesk-ci/v/NullDesk.Extensions.Mailer.Core.svg)](https://www.myget.org/feed/nulldesk-ci/package/nuget/NullDesk.Extensions.Mailer.Core) [![MyGet](https://img.shields.io/myget/nulldesk-ci/vpre/NullDesk.Extensions.Mailer.Core.svg)](https://www.myget.org/feed/nulldesk-ci/package/nuget/NullDesk.Extensions.Mailer.Core)|[![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.Core.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.Core/)|
|NullDesk.Extensions.Mailer.MailKit                                                         |[![MyGet](https://img.shields.io/myget/nulldesk-ci/v/NullDesk.Extensions.Mailer.MailKit.svg)](https://www.myget.org/feed/nulldesk-ci/package/nuget/NullDesk.Extensions.Mailer.MailKit) [![MyGet](https://img.shields.io/myget/nulldesk-ci/vpre/NullDesk.Extensions.Mailer.MailKit.svg)](https://www.myget.org/feed/nulldesk-ci/package/nuget/NullDesk.Extensions.Mailer.MailKit)|[![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.MailKit.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.MailKit/)|
|NullDesk.Extensions.Mailer.SendGrid                                                        |[![MyGet](https://img.shields.io/myget/nulldesk-ci/v/NullDesk.Extensions.Mailer.SendGrid.svg)](https://www.myget.org/feed/nulldesk-ci/package/nuget/NullDesk.Extensions.Mailer.SendGrid) [![MyGet](https://img.shields.io/myget/nulldesk-ci/vpre/NullDesk.Extensions.Mailer.SendGrid.svg)](https://www.myget.org/feed/nulldesk-ci/package/nuget/NullDesk.Extensions.Mailer.SendGrid)|[![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.SendGrid.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.SendGrid/)|

## Package Descriptions


|                                   |           |
|-----------------------------------|-----------|
|NullDesk.Extensions.Mailer         |Base classes and interfaces for the mailer extensions, and settings.|
|NullDesk.Extensions.Mailer.MailKit |SMTP Relay Email service using the popular cross platform [MailKit library](https://github.com/jstedfast/MailKit). Includes support for basic Email Template files.|
|NullDesk.Extensions.Mailer.SendGrid|SendGrid email service using SendGrid APIs. Supports basic usage of SendGrid templates; can be inherited for more advanced SendGrid functionality.|
|NullDesk.Extensions.Mailer.NetMail |*(coming soon)* SMTP Relay Email service using the cross-platform System.Net.Mail framework from Microsoft.|


## Features  

- Cross platform framework
- Netstandard 1.3 packages
  - Compatible with classic .Net and .Net core applicaitons.
- Microsoft options framework for easy configuration settings management
- Use with or without DI frameworks 
- Sample applications demonstrating common integration scenarios
- Editor support for Visual Studio Code or Visual Studio 2017
- Store message and delivery history

## Creating your own mailers

To implement your own full-featured mailer, simply implement a class that implements <code>IStandardMailer&lt;TSettings&gt;</code>. 

If you don't care about templates, you can create a simple mailer by just implementing <code>ISimpleMailer&lt;TSettings&gt;</code>.

For the above interfaces, <code>TSettings</code> is a custom DTO class containing any configuration settings your mailer requires.

The NullDesk mailers include both a simple and full-featured mailer; the full-featured mailers typically inherit the simple mailer as a base class, then extend it to add additional tempalate features from <code>IStandardMailer&lt;TSettings&gt;</code>. Here is an example of how that looks:

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
