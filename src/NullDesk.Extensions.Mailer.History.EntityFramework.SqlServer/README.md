# Using EF

## Manage Migations
At the time of this writing (March 2017) VS2017 is in RC, and the EF 1.1 CLI tools (dotnet ef) seem to have difficulty handling migrations for csproj based projects.

To manage migrations in the short term, do the following:

- Open Package Manager Console in VS 2017
- Set the default project to src\NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer
  - The PM> console usually opens to the solution root
- Use the Add-Migration commend
  - Specify the startup project and migration name

     PM> Add-Migration -StartupProject test\NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Tests -Name v3.0.0

