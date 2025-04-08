# --- CONFIGURATION ---
$vaultPath  = "C:\inetpub\vhosts\9starkiastrology.com\vault"
$binPath    = "C:\inetpub\vhosts\9starkiastrology.com\httpdocs\bin"
$viewsPath  = "C:\inetpub\vhosts\9starkiastrology.com\httpdocs\Views"
$appPool    = "9starkiastrology.com"

Write-Host "`n🛠 Starting Deployment from vault to bin...`n"

# --- Copy DLLs & PDBs ---
$files = Get-ChildItem -Path $vaultPath -Include *.dll, *.pdb -File

foreach ($file in $files) {
    $sourcePath = $file.FullName
    $destPath = Join-Path $binPath $file.Name

    try {
        Copy-Item -Path $sourcePath -Destination $destPath -Force
        Write-Host "✅ Copied $($file.Name) to bin"
    } catch {
        Write-Warning "❌ Failed to copy $($file.Name): $_"
    }
}

# --- Copy .cshtml Files to Views ---
$cshtmlFiles = Get-ChildItem -Path "$vaultPath\views" -Filter *.cshtml -Recurse

foreach ($file in $cshtmlFiles) {
    $sourcePath = $file.FullName
    $relativePath = $file.FullName.Substring($vaultPath.Length).TrimStart('\')
    $destPath = Join-Path $viewsPath $relativePath

    try {
        # Ensure the subfolder exists
        $destFolder = [System.IO.Path]::GetDirectoryName($destPath)
        if (-not (Test-Path -Path $destFolder)) {
            New-Item -ItemType Directory -Path $destFolder -Force | Out-Null
        }

        Copy-Item -Path $sourcePath -Destination $destPath -Force
        Write-Host "✅ Copied view $($file.Name) to $destPath"
    } catch {
        Write-Warning "❌ Failed to copy view $($file.Name): $_"
    }
}

# --- Restart Application Pool ---
Write-Host "`n🔄 Restarting application pool: $appPool"

try {
    Import-Module WebAdministration
    Restart-WebAppPool -Name $appPool
    Write-Host "✅ App pool '$appPool' restarted successfully."
} catch {
    Write-Warning "⚠️ Failed to restart app pool '$appPool': $_"
}

Write-Host "`n🎉 Deployment complete.`n"
