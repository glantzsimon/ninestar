# CONFIGURATION
$sourceBin    = "D:\Workspace\ninestar\webapp\WebApplication\bin"
$vaultBin     = "D:\Workspace\ninestar\vault\bin"
$vaultViews   = "D:\Workspace\ninestar\vault\views"
$apiEndpoint  = "https://9starkiastrology.com/api/deploy/upload"
$apiKey       = "fdfebb9a-0f03-4237-b0d1-55e653dd8188"

# Files to copy from bin to vault
$filesToCopy = @(
    "K9.DataAccessLayer.dll",
    "K9.DataAccessLayer.pdb",
    "K9.Globalisation.dll",
    "K9.Globalisation.pdb",
    "K9.SharedLibrary.dll",
    "K9.SharedLibrary.pdb",
    "K9.WebApplication.dll",
    "K9.WebApplication.pdb"
)

# Ensure vault/bin and vault/views exist
foreach ($folder in @($vaultBin, $vaultViews)) {
    if (!(Test-Path -Path $folder)) {
        New-Item -ItemType Directory -Path $folder -Force | Out-Null
    }
}

# Copy DLL and PDB files from bin to vault/bin
foreach ($file in $filesToCopy) {
    $src  = Join-Path $sourceBin $file
    $dest = Join-Path $vaultBin $file
    if (Test-Path $src) {
        Copy-Item -Path $src -Destination $dest -Force
        Write-Host "Copied $file to vault/bin"
    } else {
        Write-Warning "$file not found in bin folder"
    }
}

# Upload files in vault/bin to server
Get-ChildItem -Path $vaultBin -File | ForEach-Object {
    $filePath = $_.FullName
    $fileName = $_.Name
    Write-Host "Uploading $fileName..."

    try {
        $response = Invoke-RestMethod -Uri $apiEndpoint `
                                      -Headers @{ Authorization = "ApiKey $apiKey" } `
                                      -Method Post `
                                      -InFile $filePath `
                                      -ContentType "multipart/form-data"
        Write-Host "Uploaded $fileName successfully"
    } catch {
        Write-Warning "Failed to upload ${fileName}: $_"
    }
}

# Upload .cshtml files in vault/views (recursive)
Get-ChildItem -Path $vaultViews -Filter *.cshtml -Recurse | ForEach-Object {
    $filePath = $_.FullName
    $fileName = $_.Name
    $relativePath = $_.FullName.Substring($vaultViews.Length).TrimStart('\')
    Write-Host "Uploading view: $relativePath..."

    try {
        $response = Invoke-RestMethod -Uri $apiEndpoint `
                                      -Headers @{ Authorization = "ApiKey $apiKey" } `
                                      -Method Post `
                                      -InFile $filePath `
                                      -ContentType "multipart/form-data"
        Write-Host "Uploaded view $relativePath successfully"
    } catch {
        Write-Warning "Failed to upload view ${relativePath}: $_"
    }
}