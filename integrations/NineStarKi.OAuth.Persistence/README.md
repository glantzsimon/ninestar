# Isolated EF6 Code First OAuth storage

This project targets **.NET Framework 4.7.2**, matching the MVC website, and uses OpenIddict's **official EF6** model mappings. It maps **only** OAuth client/application, authorization, scope and token entities, **not** membership or password tables.

- `OAuthStoreContext` points to the existing website SQL Server **DefaultConnection** connection string but has a distinct EF6 model and migration configuration.
- `Migrations.Configuration` uses the dedicated context key `NineStarKi.ChatGPT.OAuthStore`, disables automatic migrations and data loss, and intentionally does **not** run from `WebApplication.App_Start.DataConfig.InitialiseDatabase`.
- In the current website, `DbMigrator(new DatabaseInitialiserLocal()).Update()` runs automatically on startup. OAuth migrations **must not** be inserted into that existing history or registered for automatic application.
- The OpenIddict 7.7.2 EF6 store requires **EntityFramework 6.5.2**, while the current legacy website still references **6.5.1**. The isolated project can build with 6.5.2 independently, but before linking it to the website we must review the EF6 patch-level upgrade and assembly binding redirects in the existing MVC solution, preferably on the integration site. No existing website package has been upgraded here.

The OAuth issuer is **not yet wired to the existing website**. Do not deploy this separate class library to production yet.

## Generating the first Code First migration

GitHub Actions now runs `generate-initial-migration.ps1` on a clean **Windows runner**, and publishes the generated EF6 migration files (`.cs`, `.Designer.cs`, `.resx`) as an artifact for code review. The script uses only the runner's temporary **SQL Server LocalDB** and never calls `Update-Database`. The initial scaffold is not yet committed to source control or applied anywhere.

After a successful build, inspect the generated migration and verify all operations affect only OpenIddict's own tables; then add the generated files to this isolated project. Future schema changes must also be generated through `Add-Migration` and reviewed before being applied.

**Deployment gate:** Only apply reviewed OAuth migrations to the integration SQL Server through a dedicated, explicit migrator or EF6 migration command. The existing website's automatic `DataConfig.InitialiseDatabase` must never execute this configuration. Test migration rollback before production use.
