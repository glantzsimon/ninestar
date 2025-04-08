# --- CONFIGURATION ---
$vaultPath     = "C:\inetpub\vhosts\9starkiastrology.com\vault\bin"
$vaultVewsPath = "C:\inetpub\vhosts\9starkiastrology.com\vault\views"
$binPath       = "C:\inetpub\vhosts\9starkiastrology.com\httpdocs\bin"
$viewsPath     = "C:\inetpub\vhosts\9starkiastrology.com\httpdocs\Views"
$appPool        = "9starkiastrology.com(domain)(4.0)(pool)"

Write-Host "`n🛠 Starting Deployment from vault to bin and views...`n"

# --- Copy DLLs & PDBs from vault\bin ---
$files = Get-ChildItem -Path "$vaultPath\*" -Include *.dll,*.pdb -File -Force
Write-Host "Found $($files.Count) files in '$vaultPath' to copy."

if ($files.Count -eq 0) {
    Write-Warning "No DLL or PDB files found in $vaultPath. Verify the folder and file extensions."
}

foreach ($file in $files) {
    $sourcePath = $file.FullName
    $destPath = Join-Path $binPath $file.Name

    Write-Host "Copying file: $sourcePath"
    try {
        # Attempt to copy the file
        Copy-Item -Path $sourcePath -Destination $destPath -Force -ErrorAction Stop -Verbose
        Write-Host "✅ Copied $($file.Name) to $destPath"
        
        # Verify the file exists at the destination
        if (Test-Path $destPath) {
            # Delete the file from the vault
            Remove-Item -Path $sourcePath -Force -ErrorAction Stop
            Write-Host "🗑 Deleted $($file.Name) from vault"
        } else {
            Write-Warning "File $($file.Name) does not exist at destination; skipping deletion."
        }
    } catch {
        Write-Warning "❌ Failed to copy $($file.Name): $($_.Exception.Message)"
    }
}

# --- Copy .cshtml Files from vault\views ---
$cshtmlFiles = Get-ChildItem -Path "$vaultVewsPath\*" -Filter *.cshtml -Recurse -Force
Write-Host "`nFound $($cshtmlFiles.Count) .cshtml files in '$vaultVewsPath' to copy."

foreach ($file in $cshtmlFiles) {
    $sourcePath = $file.FullName
    # Compute the relative path based on the vault views folder
    $relativePath = $file.FullName.Substring($vaultVewsPath.Length).TrimStart('\')
    $destPath = Join-Path $viewsPath $relativePath

    Write-Host "Copying view: $sourcePath"
    try {
        # Ensure the destination subfolder exists
        $destFolder = [System.IO.Path]::GetDirectoryName($destPath)
        if (-not (Test-Path -Path $destFolder)) {
            New-Item -ItemType Directory -Path $destFolder -Force | Out-Null
        }
        
        Copy-Item -Path $sourcePath -Destination $destPath -Force -ErrorAction Stop -Verbose
        Write-Host "✅ Copied view $($file.Name) to $destPath"
        
        # If the file exists at the destination, delete the source file.
        if (Test-Path $destPath) {
            Remove-Item -Path $sourcePath -Force -ErrorAction Stop
            Write-Host "🗑 Deleted view $($file.Name) from vault"
        } else {
            Write-Warning "View $($file.Name) not found at destination; skipping deletion."
        }
    } catch {
        Write-Warning "❌ Failed to copy view $($file.Name): $($_.Exception.Message)"
    }
}

# --- Restart Application Pool ---
# --- Restart Application Pool (only if files were found in vault\bin) ---
if ($files.Count -gt 0) {
    Write-Host "`n🔄 Restarting application pool: $appPool"
    try {
        Import-Module WebAdministration
        Restart-WebAppPool -Name $appPool
        Write-Host "✅ App pool '$appPool' restarted successfully."
    } catch {
        Write-Warning "⚠️ Failed to restart app pool '$appPool': $($_.Exception.Message)"
    }
} else {
    Write-Host "`nNo files were found in vault/bin, so the application pool restart was skipped."
}

Write-Host "`n🎉 Deployment complete.`n"