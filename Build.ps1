# fixes error: "System.DllNotFoundException: Unable to load DLL 'Microsoft.DiaSymReader.Native.x86.dll': The specified module could not be found. (Exception from HRESULT: 0x8007007E)"
$env:Path = ";C:\Program Files\dotnet\sdk\1.0.0-preview1-002702\runtimes\win-x86\native;" + $env:Path

Push-Location $PSScriptRoot

if(Test-Path .\artifacts) { Remove-Item .\artifacts -Force -Recurse }

& dotnet restore

$revision = @{ $true = $env:APPVEYOR_BUILD_NUMBER; $false = 1 }[$env:APPVEYOR_BUILD_NUMBER -ne $NULL];

Push-Location src/Serilog.Sinks.Email

& dotnet pack -c Release -o ..\..\.\artifacts --version-suffix=$revision
if($LASTEXITCODE -ne 0) { exit 1 }    

#Pop-Location
#Push-Location test/Serilog.Sinks.Email.Tests

#& dotnet test -c Release
#if($LASTEXITCODE -ne 0) { exit 2 }

Pop-Location
Pop-Location
