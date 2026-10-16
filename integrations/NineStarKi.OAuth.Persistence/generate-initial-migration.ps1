# Generates the OAuth EF6 Code First migration on a Windows development
# machine/CI runner. Uses ONLY an ephemeral local SQL Server LocalDB.
# This never runs Update-Database and cannot alter the live website.
$ErrorActionPreference = 'Stop'

$projectDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectAssembly = Join-Path $projectDirectory 'bin\Release\net472\NineStarKi.OAuth.Persistence.dll'
$ef6 = Join-Path $env:USERPROFILE '.nuget\packages\entityframework\6.5.2\tools\net45\any\ef6.exe'

if (-not (Test-Path $projectAssembly)) {
    throw "Build NineStarKi.OAuth.Persistence in Release first."
}
if (-not (Test-Path $ef6)) {
    throw "Restore EntityFramework 6.5.2 before generating the migration."
}
if (Get-ChildItem (Join-Path $projectDirectory 'Migrations') -Filter '*_InitialOAuthStore.cs' -ErrorAction SilentlyContinue) {
    throw 'An InitialOAuthStore migration already exists. Do not regenerate it.'
}

# This LocalDB database exists only to provide EF6's migration scaffolder
# with a safe comparison target; NEVER pass production credentials here.
$connectionString = 'Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=NineStarKiOAuthScaffolding;Integrated Security=True;MultipleActiveResultSets=True'

& $ef6 migrations add InitialOAuthStore `
    --assembly $projectAssembly `
    --project-dir $projectDirectory `
    --migrations-config NineStarKi.OAuth.Persistence.Migrations.Configuration `
    --connection-string $connectionString `
    --connection-provider System.Data.SqlClient `
    --root-namespace NineStarKi.OAuth.Persistence `
    --no-color

if ($LASTEXITCODE -ne 0) {
    throw "EF6 migration generation failed (exit code $LASTEXITCODE)."
}

Get-ChildItem (Join-Path $projectDirectory 'Migrations') -Filter '*_InitialOAuthStore*' |
    Select-Object Name, Length
