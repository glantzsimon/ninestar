# NINESTAR_DEPLOY_V2
[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [ValidateSet('prod', 'int')][string]$Environment = 'prod',
    [string]$SourceRoot = ''
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

try {
    $vaultRoot = [IO.Path]::GetFullPath($PSScriptRoot)
    $expectedSource = if ($Environment -eq 'int') { Join-Path $vaultRoot 'int' } else { $vaultRoot }
    if ([string]::IsNullOrWhiteSpace($SourceRoot)) { $SourceRoot = $expectedSource }
    $SourceRoot = [IO.Path]::GetFullPath($SourceRoot).TrimEnd('\')
    if ($SourceRoot -ine $expectedSource.TrimEnd('\')) { throw 'Source vault does not match the selected environment.' }
    if (-not (Test-Path -LiteralPath $SourceRoot -PathType Container)) { throw 'Source vault is missing.' }

    $siteRoot = Split-Path -Parent $vaultRoot
    $destination = Join-Path $siteRoot $(if ($Environment -eq 'int') { 'int' } else { 'httpdocs' })
    if (-not (Test-Path -LiteralPath $destination -PathType Container)) { throw 'Destination website is missing.' }
    $plan = @()
    $groups = @(
        @{ Source = ''; Target = ''; Extensions = @('.config'); Recurse = $false },
        @{ Source = 'bin'; Target = 'bin'; Extensions = @('.dll', '.pdb'); Recurse = $false },
        @{ Source = 'views'; Target = 'Views'; Extensions = @('.cshtml', '.config'); Recurse = $true },
        @{ Source = 'config'; Target = 'Config'; Extensions = @('.json'); Recurse = $true },
        @{ Source = 'css'; Target = 'Content'; Extensions = @('.css'); Recurse = $true },
        @{ Source = 'scripts'; Target = 'Scripts'; Extensions = @('.js'); Recurse = $true },
        @{ Source = 'images'; Target = 'Images'; Extensions = @('.png', '.jpg', '.jpeg', '.gif'); Recurse = $true },
        @{ Source = 'videos'; Target = 'Videos'; Extensions = @('.mp4', '.webm'); Recurse = $true }
    )
    foreach ($group in $groups) {
        $source = if ($group.Source) { Join-Path $SourceRoot $group.Source } else { $SourceRoot }
        if (-not (Test-Path -LiteralPath $source -PathType Container)) { continue }
        $recurse = [bool]$group.Recurse
        $files = @(Get-ChildItem -LiteralPath $source -File -Recurse:$recurse |
            Where-Object { $group.Extensions -contains $_.Extension.ToLowerInvariant() })
        foreach ($file in $files) {
            $relative = $file.FullName.Substring($source.Length).TrimStart('\')
            $targetRoot = if ($group.Target) { Join-Path $destination $group.Target } else { $destination }
            $plan += [pscustomobject]@{ Source = $file.FullName; Target = (Join-Path $targetRoot $relative); Binary = ($group.Source -eq 'bin') }
        }
    }
    Write-Host "Deployment target: $Environment -> $destination ($($plan.Count) files)"
    if ($WhatIfPreference) {
        foreach ($item in $plan) { Write-Host "Would copy $($item.Source) -> $($item.Target)" }
        return
    }
    # ASP.NET reloads the application when bin files or web.config change.
    # Do not consume staged files until every copy has been verified.
    foreach ($item in $plan) {
        $folder = Split-Path -Parent $item.Target
        if (-not (Test-Path -LiteralPath $folder)) { New-Item -ItemType Directory -Path $folder -Force | Out-Null }
        Copy-Item -LiteralPath $item.Source -Destination $item.Target -Force -ErrorAction Stop
        if ((Get-FileHash -LiteralPath $item.Source -Algorithm SHA256).Hash -ne
            (Get-FileHash -LiteralPath $item.Target -Algorithm SHA256).Hash) { throw "Copy verification failed: $($item.Target)" }
    }
    foreach ($item in $plan) { Remove-Item -LiteralPath $item.Source -Force -ErrorAction Stop }
    Write-Host "Deployment completed successfully: $Environment"
    exit 0
} catch {
    Write-Error "Deployment failed; inspect destination and retained staging files: $($_.Exception.Message)" -ErrorAction Continue
    exit 1
}
