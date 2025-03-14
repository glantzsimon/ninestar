param([String]$publishPassword='', [String]$env='')

$publishDir = "publish"
$appDir = "webapp"
$projectPath = "WebApplication\WebApplication.csproj"
	
function ProcessErrors(){
  if($? -eq $false)
  {
    throw "The previous command failed (see above)";
  }
}

function _CreateDirectory($dir) {
  If (-Not (Test-Path $dir)) {
    New-Item -ItemType Directory -Path $dir
  }
}

function _Publish() {
  echo "Publishing Application"
  
  pushd $appDir
  ProcessErrors
  
  _CreateDirectory $publishDir
  ProcessErrors
  
  Msbuild $projectPath /p:DeployOnBuild=true /p:PublishProfile=$env /p:Configuration=$env /p:AllowUntrustedCertificate=true /p:Password=$publishPassword /p:LangVersion=7.3
  
  ProcessErrors
  popd
}

function Main {
  Try {
    _Publish
  }
  Catch {
    Write-Error $_.Exception
    exit 1
  }
}

Main