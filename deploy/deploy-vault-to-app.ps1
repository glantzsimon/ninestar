# --- CONFIGURATION ---
$vaultBinPath      = "C:\inetpub\vhosts\9starkiastrology.com\vault\bin"
$vaultViewsPath    = "C:\inetpub\vhosts\9starkiastrology.com\vault\views"
$vaultCssPath      = "C:\inetpub\vhosts\9starkiastrology.com\vault\css"
$vaultScriptsPath      = "C:\inetpub\vhosts\9starkiastrology.com\vault\scripts"
$vaultImagesPath   = "C:\inetpub\vhosts\9starkiastrology.com\vault\images"

$destBinPath       = "C:\inetpub\vhosts\9starkiastrology.com\httpdocs\bin"
$destViewsPath     = "C:\inetpub\vhosts\9starkiastrology.com\httpdocs\Views"
$destCssPath       = "C:\inetpub\vhosts\9starkiastrology.com\httpdocs\Content"
$destScriptsPath       = "C:\inetpub\vhosts\9starkiastrology.com\httpdocs\Scripts"
$destImagesPath    = "C:\inetpub\vhosts\9starkiastrology.com\httpdocs\Images"

$appPool           = "9starkiastrology.com(domain)(4.0)(pool)"

Write-Host "`n🛠 Starting deployment from vault to live site...`n"

# --- Copy DLLs & PDBs from vault\bin ---
$files = Get-ChildItem -Path "$vaultBinPath\*" -Include *.dll,*.pdb -File -Force
Write-Host "📦 Found $($files.Count) files in '$vaultBinPath' to copy."

if ($files.Count -eq 0) {
    Write-Warning "⚠️ No DLL or PDB files found. Verify the folder and file extensions."
}

foreach ($file in $files) {
    $sourcePath = $file.FullName
    $destPath = Join-Path $destBinPath $file.Name

    Write-Host "Copying binary: $sourcePath"
    try {
        Copy-Item -Path $sourcePath -Destination $destPath -Force -ErrorAction Stop -Verbose
        Write-Host "✅ Copied $($file.Name) to $destPath"

        if (Test-Path $destPath) {
            Remove-Item -Path $sourcePath -Force -ErrorAction Stop
            Write-Host "🗑 Deleted $($file.Name) from vault"
        }
    } catch {
        Write-Warning "❌ Failed to copy $($file.Name): $($_.Exception.Message)"
    }
}

# --- Copy .cshtml Views ---
$cshtmlFiles = Get-ChildItem -Path "$vaultViewsPath\*" -Filter *.cshtml -Recurse -Force
Write-Host "`n📄 Found $($cshtmlFiles.Count) .cshtml files to copy."

foreach ($file in $cshtmlFiles) {
    $sourcePath = $file.FullName
    $relativePath = $file.FullName.Substring($vaultViewsPath.Length).TrimStart('\')
    $destPath = Join-Path $destViewsPath $relativePath

    Write-Host "Copying view: $sourcePath"
    try {
        $destFolder = [System.IO.Path]::GetDirectoryName($destPath)
        if (-not (Test-Path $destFolder)) {
            New-Item -ItemType Directory -Path $destFolder -Force | Out-Null
        }

        Copy-Item -Path $sourcePath -Destination $destPath -Force -ErrorAction Stop -Verbose
        Write-Host "✅ Copied view $($file.Name) to $destPath"

        if (Test-Path $destPath) {
            Remove-Item -Path $sourcePath -Force -ErrorAction Stop
            Write-Host "🗑 Deleted view $($file.Name) from vault"
        }
    } catch {
        Write-Warning "❌ Failed to copy view $($file.Name): $($_.Exception.Message)"
    }
}

# --- Copy .css Files ---
$cssFiles = Get-ChildItem -Path "$vaultCssPath\*" -Filter *.css -Recurse -Force
Write-Host "`n🎨 Found $($cssFiles.Count) .css files to copy."

foreach ($file in $cssFiles) {
    $sourcePath = $file.FullName
    $relativePath = $file.FullName.Substring($vaultCssPath.Length).TrimStart('\')
    $destPath = Join-Path $destCssPath $relativePath

    Write-Host "Copying CSS: $sourcePath"
    try {
        $destFolder = [System.IO.Path]::GetDirectoryName($destPath)
        if (-not (Test-Path $destFolder)) {
            New-Item -ItemType Directory -Path $destFolder -Force | Out-Null
        }

        Copy-Item -Path $sourcePath -Destination $destPath -Force -ErrorAction Stop -Verbose
        Write-Host "✅ Copied CSS $($file.Name) to $destPath"

        if (Test-Path $destPath) {
            Remove-Item -Path $sourcePath -Force -ErrorAction Stop
            Write-Host "🗑 Deleted CSS $($file.Name) from vault"
        }
    } catch {
        Write-Warning "❌ Failed to copy CSS $($file.Name): $($_.Exception.Message)"
    }
}

# --- Copy .js Files ---
$jsFiles = Get-ChildItem -Path "$vaultScriptsPath\*" -Filter *.js -Recurse -Force
Write-Host "`n🎨 Found $($jsFiles.Count) .js files to copy."

foreach ($file in $jsFiles) {
    $sourcePath = $file.FullName
    $relativePath = $file.FullName.Substring($vaultScriptsPath.Length).TrimStart('\')
    $destPath = Join-Path $destScriptsPath $relativePath

    Write-Host "Copying Scripts: $sourcePath"
    try {
        $destFolder = [System.IO.Path]::GetDirectoryName($destPath)
        if (-not (Test-Path $destFolder)) {
            New-Item -ItemType Directory -Path $destFolder -Force | Out-Null
        }

        Copy-Item -Path $sourcePath -Destination $destPath -Force -ErrorAction Stop -Verbose
        Write-Host "✅ Copied Js $($file.Name) to $destPath"

        if (Test-Path $destPath) {
            Remove-Item -Path $sourcePath -Force -ErrorAction Stop
            Write-Host "🗑 Deleted Js $($file.Name) from vault"
        }
    } catch {
        Write-Warning "❌ Failed to copy Js $($file.Name): $($_.Exception.Message)"
    }
}

# --- Copy Images ---
$imageExtensions = @("*.png", "*.jpg", "*.jpeg", "*.PNG", "*.JPG", "*.JPEG")
$imageFiles = @()
foreach ($ext in $imageExtensions) {
    $imageFiles += Get-ChildItem -Path "$vaultImagesPath" -Include $ext -Recurse -File -Force
}
Write-Host "`n🖼 Found $($imageFiles.Count) image files to copy."

foreach ($file in $imageFiles) {
    $sourcePath = $file.FullName
    $relativePath = $file.FullName.Substring($vaultImagesPath.Length).TrimStart('\')
    $destPath = Join-Path $destImagesPath $relativePath

    Write-Host "Copying image: $sourcePath"
    try {
        $destFolder = [System.IO.Path]::GetDirectoryName($destPath)
        if (-not (Test-Path $destFolder)) {
            New-Item -ItemType Directory -Path $destFolder -Force | Out-Null
        }

        Copy-Item -Path $sourcePath -Destination $destPath -Force -ErrorAction Stop -Verbose
        Write-Host "✅ Copied image $($file.Name) to $destPath"

        if (Test-Path $destPath) {
            Remove-Item -Path $sourcePath -Force -ErrorAction Stop
            Write-Host "🗑 Deleted image $($file.Name) from vault"
        }
    } catch {
        Write-Warning "❌ Failed to copy image $($file.Name): $($_.Exception.Message)"
    }
}

# --- Restart App Pool if any DLLs were copied ---
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
    Write-Host "`nℹ️ No DLLs copied. App pool restart skipped."
}

Write-Host "`n🎉 Deployment complete.`n"
