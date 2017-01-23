# NullDesk Email Extensions

 



## Overview
Email extension service packages for quickly integrating common mail scenarios into any .Net Core or .Net project using a variety of message delivery frameworks and cloud service providers.

## Status

|                                   |   |   |
|-----------------------------------|:-:|:-:|
|Project  [Issue Board](https://github.com/NullDesk/NullMailer/issues#boards?repos=79507993)|[![Build status](https://ci.appveyor.com/api/projects/status/5uc95cb6xho4qtdh/branch/master?svg=true)](https://ci.appveyor.com/project/StephenRedd/nullmailer/branch/master)|[![ZenHub](https://img.shields.io/badge/Shipping_faster_with-ZenHub-5e60ba.svg?style=flat-square)](https://github.com/NullDesk/NullMailer/issues#boards?repos=79507993)|
|NullDesk.Extensions.Mailer                                                                 |[![MyGet](https://img.shields.io/myget/nulldesk-ci/v/NullDesk.Extensions.Mailer.Core.svg)](NullDesk-CI)|[![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.Core.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.Core/)|
|NullDesk.Extensions.Mailer.MailKit                                                         |[![MyGet](https://img.shields.io/myget/nulldesk-ci/v/NullDesk.Extensions.Mailer.MailKit.svg)](NullDesk-CI)|[![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.MailKit.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.MailKit/)|
|NullDesk.Extensions.Mailer.SendGrid                                                        |[![MyGet](https://img.shields.io/myget/nulldesk-ci/v/NullDesk.Extensions.Mailer.SendGrid.svg)](NullDesk-CI)|[![NuGet](https://img.shields.io/nuget/v/NullDesk.Extensions.Mailer.SendGrid.svg)](https://www.nuget.org/packages/NullDesk.Extensions.Mailer.SendGrid/)|

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
- Editor support for Visual Studio Code or Visual Studio 2015

## **WARNING:** SendGrid Beta Package

This project uses a beta version of [SendGrid v9](https://github.com/sendgrid/sendgrid-csharp), which has not yet been published to a public NuGet feed. 

As a work-around, the beta package is hosted on the NullDesk public feed at MyGet.org until official versions are released.

Create a nuget.config file in the root of your project:

    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
      <packageSources>
        <add key="NullDesk" value="https://www.myget.org/F/nulldesk/api/v3/index.json" />
      </packageSources>
    </configuration>   

Alternately, you can setup nuget to get the package from the filesystem. The nupkg files are included in the repository in the <code>./library</code> folder.
