# NullDesk Email Extensions



[ZenHub Project Board](https://github.com/NullDesk/NullMailer/issues#boards?repos=79507993)  

<a href="https://zenhub.io"><img src="https://raw.githubusercontent.com/ZenHubIO/support/master/zenhub-badge.png"></a>

## Overview
Email extension services for quickly integrating common mail scenarios into any .Net Core or .Net project using a variety of different message frameworks and cloud service providers.

## Features  

- Cross platform framework
- Netstandard 1.3 packages
  - Compatible with classic .Net and .Net core applicaitons.
- Microsoft options framework for easy configuration settings management
- Use with or without DI frameworks 
- Sample applications demonstrating common integration scenarios
- Editor support for Visual Studio Code or Visual Studio 2015
 

## Packages


|Package|Description|
|------:|:----------|
|NullDesk.Extensions.Mailer.Core|Base classes and interfaces for the mailer extensions, and settings.|
|NullDesk.Extensions.Mailer.MailKit|SMTP Relay Email service using the popular cross platform [MailKit library](https://github.com/jstedfast/MailKit). Includes support for basic Email Template files.|
|NullDesk.Extensions.Mailer.SendGrid|SendGrid email service using SendGrid APIs. Supports basic usage of SendGrid templates; can be inherited for more advanced SendGrid functionality.|
|NullDesk.Extensions.Mailer.NetMail|*(coming soon)* SMTP Relay Email service using the cross-platform System.Net.Mail framework from Microsoft.


## **WARNING:** SendGrid Beta Package

This project uses a beta version of SendGrid v9, which has not yet been published to a public NuGet feed. To use the SendGrid package you may need to copy the sendgrid nupkg files from the <code>/library</code> folder to your local file, or a private NuGet feed of your own, and configure it as a package source for your project.  
