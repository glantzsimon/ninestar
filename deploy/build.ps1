param([String]$publishPassword='')

$publishDir = "publish"
$appDir = "webapp"
$projectPath = ".\WebApplication\WebApplication.csproj"
$webTestFile = ".\WebApplication.Tests\bin\Debug\K9.WebApplication.Tests.dll"
$dataTestFile = ".\DataAccess.Tests\bin\Debug\K9.DataAccessLayer.Tests.dll"
	
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

function _CreateDirectory($dir) {
  If (-Not (Test-Path $dir)) {
    New-Item -ItemType Directory -Path $dir
  }
}

function _NugetRestore() {
  echo "Running nuget restore"

  pushd $appDir
  ProcessErrors
  
  nuget restore
  ProcessErrors
  popd
}

function _Test() {
  echo "Preparing to run tests"
  
  pushd $appDir  
  ProcessErrors
  
  echo "Running web application tests"
  .\packages\xunit.runner.console.2.3.1\tools\net452\xunit.console.exe $webTestFile
  ProcessErrors  
  
  echo "Running data access layer tests"
  .\packages\xunit.runner.console.2.3.1\tools\net452\xunit.console.exe $dataTestFile
  ProcessErrors
  popd
}

function _Build() {
  echo "Building App"
  
  pushd $appDir
  ProcessErrors
  
  Msbuild /p:Configuration=Debug
  ProcessErrors
  popd
}

function _Publish() {
  echo "Publishing App"
  
  pushd $appDir
  ProcessErrors
  
  echo "Creating publish dir"
  _CreateDirectory $publishDir
  ProcessErrors
  
  echo "Building project"  
  Msbuild $projectPath /p:DeployOnBuild=true /p:PublishProfile=Integration /p:Configuration=Integration /p:DefineConstants="Integration" /p:AllowUntrustedCertificate=true /p:Password=$publishPassword
  ProcessErrors 

}

function Main {
  Try {
    _Build
	_Test
	_Publish
  }
  Catch {
    Write-Error $_.Exception
    exit 1
  }
}

Main