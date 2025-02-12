# Define variables
$AppDir = "webapp"
$ProjectRoot = "WebApplication"
$StorjBucket = "cdn-media"
$UplinkPath = "C:\Program Files\Storj\uplink.exe"
	
function ProcessErrors(){
  if($? -eq $false)
  {
    throw "The previous command failed (see above)";
  }
}

function _UploadMedia() {
  echo "Uploading media..."
  
  pushd $AppDir
  ProcessErrors
  
  # Folders to process
  $FoldersToUpload = @("Images", "Videos")

  # Change to the project root directory
  Set-Location -Path $ProjectRoot

  # Loop through specified folders
  foreach ($Folder in $FoldersToUpload) {

      # Check if folder exists
      if (Test-Path -Path $Folder) {
          # Get all files in the folder and subfolders
          Get-ChildItem -Path $Folder -Recurse -File | ForEach-Object {
              $File = $_
			  
              $Index = $File.FullName.IndexOf($ProjectRoot)
				if ($Index -ge 0) {
					$RelativePath = $File.FullName.Substring($Index + $ProjectRoot.Length).TrimStart("\/") -replace '\\', '/'
				} else {
					Write-Host "Warning: '$ProjectRoot' not found in '$($File.FullName)'"
					$RelativePath = $File.FullName -replace '\\', '/'  # Fallback to full path
				}

              # Construct the Storj path (keeping the relative structure)
              $StorjPath = "$StorjBucket/$RelativePath"

              # Upload file to Storj
              & "$UplinkPath" cp "$($File.FullName)" "sj://$StorjPath"

              if ($?) {
                  Write-Host "Uploaded: $StorjPath"
              } else {
                  Write-Host "Failed to upload: $StorjPath"
              }			  
          }
      } else {
          Write-Host "Folder not found: $FullFolderPath"
      }
  }
  
  ProcessErrors
  popd
}

function Main {
  Try {
    _UploadMedia
  }
  Catch {
    Write-Error $_.Exception
    exit 1
  }
}

Main