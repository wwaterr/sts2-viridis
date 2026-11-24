param(
    [string]$Sts2Dir = $env:STS2_DATA_DIR,
    [string]$ModsDir = "",
    [switch]$Deploy
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path

if (-not $Sts2Dir) {
    throw 'Pass -Sts2Dir or set STS2_DATA_DIR.'
}
$Sts2Dir = (Resolve-Path -LiteralPath $Sts2Dir).Path

if ($Deploy -and -not $ModsDir) {
    $ModsDir = Join-Path (Split-Path -Parent $Sts2Dir) 'mods'
}

# Locate a dotnet SDK: env override -> workspace-local SDK -> PATH
$dotnet = $env:STS2_DOTNET
if (-not $dotnet -or -not (Test-Path $dotnet)) {
    $local = Join-Path $root '..\..\.dotnet\dotnet.exe'
    if (Test-Path $local) {
        $dotnet = (Resolve-Path $local).Path
    } else {
        $dotnet = 'dotnet'
    }
}

$buildArgs = @(
    'build', (Join-Path $root 'EmptyCharacterMod.csproj'),
    '-c', 'Release',
    '-v', 'm'
)
if ($Sts2Dir) { $buildArgs += "-p:Sts2Dir=$Sts2Dir" }
if ($ModsDir) { $buildArgs += "-p:ModsDir=$ModsDir" }
if ($Deploy)  { $buildArgs += '-p:DeployMod=true' }

Write-Host "dotnet: $dotnet"
& $dotnet @buildArgs
if ($LASTEXITCODE -ne 0) { throw "build failed with exit code $LASTEXITCODE" }

$dll = Join-Path $root 'bin\Release\net9.0\EmptyCharacterMod.dll'
Write-Host ""
Write-Host "Built: $dll"
if ($Deploy) {
    $target = $ModsDir
    Write-Host "Deployed to: $target\EmptyCharacterMod"
} else {
    Write-Host "Not deployed. Re-run with -Deploy to copy into the game's mods folder."
}