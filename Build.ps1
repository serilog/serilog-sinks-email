Push-Location $PSScriptRoot

if(Test-Path .\artifacts) { Remove-Item .\artifacts -Force -Recurse }

& dotnet restore --no-cache

$branch = $(git symbolic-ref --short -q HEAD)
$revision = @{ $true = $env:APPVEYOR_BUILD_NUMBER; $false = "local" }[$env:APPVEYOR_BUILD_NUMBER -ne $NULL];
$suffix = @{ $true = ""; $false = "$branch-$revision"}[$branch -eq "master" -and $revision -ne "local"]

foreach ($src in ls src/Serilog.*) {
    Push-Location $src

    & dotnet pack -c Release -o ..\..\.\artifacts --version-suffix=$suffix
    if($LASTEXITCODE -ne 0) { exit 1 }    

    Pop-Location
}

foreach ($test in ls test/Serilog.*.Tests) {
    Push-Location $test

    & dotnet test -c Release
    if($LASTEXITCODE -ne 0) { exit 2 }

    Pop-Location
}

Pop-Location
