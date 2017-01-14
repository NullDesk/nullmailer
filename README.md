# NullDesk Email Extensions

## Overview
Email service extensions for sending email through a variety of different email frameworks and cloud service providers.

## Features  

- Netstandard 1.3 packages
  - Compatible with classic .Net and .Net core applicaitons.
- Uses the Microsoft options framework for configuration settings
- Designed for use with or without DI frameworks 
  - Support Microsoft's .Net Core Dependency Injection Extensions framework for both mail services and option settings

## Packages

|   |   |
|--:|:--|
|NullDesk.Extensions.Mailer.Core|Base classes and interfaces for the mailer extensions, and settings.|
|NullDesk.Extensions.Mailer.MailKit|SMTP Relay Email service using the popular cross platform [MailKit library](https://github.com/jstedfast/MailKit). Includes support for basic Email Template files.|
|NullDesk.Extensions.Mailer.SendGrid|SendGrid email service using SendGrid APIs. Supports basic usage of SendGrid templates; can be inherited for more advanced SendGrid functionality.|
|NullDesk.Extensions.Mailer.NetMail|*(coming soon)* SMTP Relay Email service using the cross-platform System.Net.Mail framework from Microsoft.

