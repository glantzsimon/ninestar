# Isolated EF6 Code First OAuth storage

This project targets **.NET Framework 4.7.2**, matching the MVC website, and uses OpenIddict's **official EF6** model mappings. It maps **only** OAuth client/application, authorization, scope and token entities, **not** membership or password tables.

- `OAuthStoreContext` points to the existing website SQL Server **DefaultConnection** connection string but has a distinct EF6 model and migration configuration.
- `Migrations.Configuration` uses the dedicated context key `NineStarKi.ChatGPT.OAuthStore`, disables automatic migrations and data loss, and intentionally does **not** run from `WebApplication.App_Start.DataConfig.InitialiseDatabase`.
- In the current website, `DbMigrator(new DatabaseInitialiserLocal()).Update()` runs automatically on startup. OAuth migrations **must not** be inserted into that existing history or registered for automatic application.
- The OAuth issuer is **not yet wired to the existing website**. Do not deploy this separate class library to production yet.

## Generating the first Code First migration

After validating the isolated context build and restoring all packages, use the EF6 migration tool **against this project/context only**, on an isolated development SQL Server database:

```powershell
# From Visual Studio Package Manager Console, after adding this project to a
# developer solution and confirming the separate OAuth context is selected:
Add-Migration InitialOAuthStore -ConfigurationTypeName NineStarKi.OAuth.Persistence.Migrations.Configuration -ProjectName NineStarKi.OAuth.Persistence -StartUpProjectName WebApplication
```

The command must generate the EF6 migration C#, designer metadata and model resource files. **Commit all generated files for review**. Do not hand-write a SQL schema or rely on `AutomaticMigrationsEnabled`; keep `Update-Database` as a separate explicitly reviewed step for the integration SQL Server only. Later, when the OAuth issuer is enabled, run `DbMigrator(new NineStarKi.OAuth.Persistence.Migrations.Configuration()).Update()` only through a controlled migration/deployment task, never from normal website startup.

The OpenIddict OWIN host configuration will be added **after** the generated migration and EF6 model have been verified in Windows CI. Its authentication screen must use the site's existing WebSecurity/FormsAuthentication cookie and must not modify existing credentials.
