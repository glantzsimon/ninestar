$appName = "ninestar"
$publishDir = "publish"
$appDir = "../webapp"
$testDir = "../webapp/WebApplication.Tests"

function ProcessErrors(){
  if($? -eq $false)
  {
    throw "The previous command failed (see above)";
  }
}

function _DeleteFile($fileName) {
  If (Test-Path $fileName) {
    Write-Host "Deleting '$fileName'"
    Remove-Item $fileName
  } else {
    "'$fileName' not found. Nothing deleted"
  }
}

function _Clean() {
  echo "Cleaning old content"

  _DeleteFile "$appName.zip"
}

function NugetRestore() {
  echo "Running nuget restore"

  pushd $appDir
  nuget restore
  ProcessErrors
}

function _DotnetTest() {
  echo "Running dotnet test"
  pushd $testDir
  ProcessErrors

  dotnet test
  ProcessErrors

  popd
}

function _Build() {
  echo "Building App"
  pushd $appPath
  ProcessErrors

  Msbuild -p Configuration=Release -p DeployOnBuild=true -p PublishProfile=IntegrationLocal
  ProcessErrors

  popd
}

function Main {
  Try {
    _Clean
    _NugetRestore
    _Test
    _Build
  }
  Catch {
    Write-Error $_.Exception
    exit 1
  }
}

Main