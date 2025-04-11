# --- CONFIGURATION ---
$vaultBinPath        = "C:\inetpub\vhosts\9starkiastrology.com\vault\bin"
$vaultRootPath       = "C:\inetpub\vhosts\9starkiastrology.com\vault"
$vaultViewsPath      = "C:\inetpub\vhosts\9starkiastrology.com\vault\views"
$vaultCssPath        = "C:\inetpub\vhosts\9starkiastrology.com\vault\css"
$vaultScriptsPath    = "C:\inetpub\vhosts\9starkiastrology.com\vault\scripts"
$vaultImagesPath     = "C:\inetpub\vhosts\9starkiastrology.com\vault\images"
$vaultVideosPath     = "C:\inetpub\vhosts\9starkiastrology.com\vault\videos"

$destRootPath        = "C:\inetpub\vhosts\9starkiastrology.com\httpdocs"
$destBinPath         = "C:\inetpub\vhosts\9starkiastrology.com\httpdocs\bin"
$destViewsPath       = "C:\inetpub\vhosts\9starkiastrology.com\httpdocs\Views"
$destCssPath         = "C:\inetpub\vhosts\9starkiastrology.com\httpdocs\Content"
$destScriptsPath     = "C:\inetpub\vhosts\9starkiastrology.com\httpdocs\Scripts"
$destImagesPath      = "C:\inetpub\vhosts\9starkiastrology.com\httpdocs\Images"
$destVideosPath      = "C:\inetpub\vhosts\9starkiastrology.com\httpdocs\Videos"

$appPool             = "9starkiastrology.com(domain)(4.0)(pool)"

Write-Host "`n🛠 Starting deployment from vault to live site...`n"

function Copy-And-CleanFile {
    param (
        [string]$sourcePath,
        [string]$destPath,
        [string]$label
    )

    Write-Host "Copying $label: $sourcePath"
    try {
        $destFolder = [System.IO.Path]::GetDirectoryName($destPath)
        if (-not (Test-Path $destFolder)) {
            New-Item -ItemType Directory -Path $destFolder -Force | Out-Null
        }

        Copy-Item -Path $sourcePath -Destination $destPath -Force -ErrorAction Stop -Verbose
        Write-Host "✅ Copied $label $([System.IO.Path]::GetFileName($sourcePath)) to $destPath"

        if (Test-Path $destPath) {
            Remove-Item -Path $sourcePath -Force -ErrorAction Stop
            Write-Host "🗑 Deleted $label $([System.IO.Path]::GetFileName($sourcePath)) from vault"
        }
    } catch {
        Write-Warning "❌ Failed to copy $label $([System.IO.Path]::GetFileName($sourcePath)): $($_.Exception.Message)"
    }
}

# --- Copy DLLs & PDBs ---
$files = Get-ChildItem -Path "$vaultBinPath\*" -Include *.dll,*.pdb -File -Force
Write-Host "📦 Found $($files.Count) files in '$vaultBinPath' to copy."

if ($files.Count -eq 0) {
    Write-Warning "⚠️ No DLL or PDB files found. Verify the folder and file extensions."
}

foreach ($file in $files) {
    $sourcePath = $file.FullName
    $destPath = Join-Path $destBinPath $file.Name
    Copy-And-CleanFile -sourcePath $sourcePath -destPath $destPath -label "binary"
}

# --- Copy web.config ---
$configFiles = Get-ChildItem -Path "$vaultRootPath\*" -Include *.config -File -Force
Write-Host "📦 Found $($configFiles.Count) config file(s) in '$vaultRootPath' to copy."

if ($configFiles.Count -eq 0) {
    Write-Warning "⚠️ No config files found."
}

foreach ($file in $configFiles) {
    $sourcePath = $file.FullName
    $destPath = Join-Path $destRootPath $file.Name
    Copy-And-CleanFile -sourcePath $sourcePath -destPath $destPath -label "config"
}

# --- Copy Views (.cshtml) ---
$cshtmlFiles = Get-ChildItem -Path "$vaultViewsPath\*" -Filter *.cshtml -Recurse -Force
Write-Host "`n📄 Found $($cshtmlFiles.Count) .cshtml files to copy."

foreach ($file in $cshtmlFiles) {
    $sourcePath = $file.FullName
    $relativePath = $file.FullName.Substring($vaultViewsPath.Length).TrimStart('\')
    $destPath = Join-Path $destViewsPath $relativePath
    Copy-And-CleanFile -sourcePath $sourcePath -destPath $destPath -label "view"
}

# --- Copy CSS ---
$cssFiles = Get-ChildItem -Path "$vaultCssPath\*" -Filter *.css -Recurse -Force
Write-Host "`n🎨 Found $($cssFiles.Count) .css files to copy."

foreach ($file in $cssFiles) {
    $sourcePath = $file.FullName
    $relativePath = $file.FullName.Substring($vaultCssPath.Length).TrimStart('\')
    $destPath = Join-Path $destCssPath $relativePath
    Copy-And-CleanFile -sourcePath $sourcePath -destPath $destPath -label "CSS"
}

# --- Copy JS ---
$jsFiles = Get-ChildItem -Path "$vaultScriptsPath\*" -Filter *.js -Recurse -Force
Write-Host "`n🧠 Found $($jsFiles.Count) .js files to copy."

foreach ($file in $jsFiles) {
    $sourcePath = $file.FullName
    $relativePath = $file.FullName.Substring($vaultScriptsPath.Length).TrimStart('\')
    $destPath = Join-Path $destScriptsPath $relativePath
    Copy-And-CleanFile -sourcePath $sourcePath -destPath $destPath -label "JS"
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
    Copy-And-CleanFile -sourcePath $sourcePath -destPath $destPath -label "image"
}

# --- Copy Videos ---
$videoExtensions = @("*.mp4", "*.webm", "*.MP4", "*.WEBM")
$videoFiles = @()
foreach ($ext in $videoExtensions) {
    $videoFiles += Get-ChildItem -Path "$vaultVideosPath" -Include $ext -Recurse -File -Force
}
Write-Host "`n🎥 Found $($videoFiles.Count) video files to copy."

foreach ($file in $videoFiles) {
    $sourcePath = $file.FullName
    $relativePath = $file.FullName.Substring($vaultVideosPath.Length).TrimStart('\')
    $destPath = Join-Path $destVideosPath $relativePath
    Copy-And-CleanFile -sourcePath $sourcePath -destPath $destPath -label "video"
}

# --- Restart App Pool if DLLs were copied ---
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
