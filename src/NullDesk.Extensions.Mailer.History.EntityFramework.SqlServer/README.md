# Using EF

## Manage Migations

To manage migrations, use the EF CLI commands at a command line or powershell prompt. Since EF Core doesn't currently support running from class library projects, you must specify a startup project. It's easiest to use the unit test project.

     dotnet ef  --startup-project ..\..\test\NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Tests migrations add initial

## Compiler Warnings for XML Comments

Add the following two lines to the top of each generated migration, migration.designer, and snapshot file:

    // ReSharper disable All
    #pragma warning disable 1591