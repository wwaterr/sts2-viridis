param(
    [string]$Sts2Dir = $env:STS2_DATA_DIR,
    [string]$RitsuLibDir = $env:STS2_RITSULIB_DIR,
    [string]$ModsDir = "",
    [switch]$Deploy
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$repo = Resolve-Path (Join-Path $root '..\..')

if (-not $Sts2Dir) {
    throw 'Pass -Sts2Dir or set STS2_DATA_DIR.'
}
$Sts2Dir = (Resolve-Path -LiteralPath $Sts2Dir).Path

if (-not $RitsuLibDir) {
    throw 'Pass -RitsuLibDir or set STS2_RITSULIB_DIR.'
}
$RitsuLibDir = (Resolve-Path -LiteralPath $RitsuLibDir).Path

if ($Deploy -and -not $ModsDir) {
    $ModsDir = Join-Path (Split-Path -Parent $Sts2Dir) 'mods'
}

# Locate a dotnet SDK: env override -> workspace-local SDK -> PATH
$dotnet = $env:STS2_DOTNET
if (-not $dotnet -or -not (Test-Path $dotnet)) {
    $local = Join-Path $repo '.dotnet\dotnet.exe'
    if (Test-Path $local) { $dotnet = $local } else { $dotnet = 'dotnet' }
}

$buildArgs = @('build', (Join-Path $root 'Viridis.csproj'), '-c', 'Release', '-v', 'm')
if ($Sts2Dir)     { $buildArgs += "-p:Sts2Dir=$Sts2Dir" }
if ($RitsuLibDir) { $buildArgs += "-p:RitsuLibDir=$RitsuLibDir" }
if ($ModsDir)     { $buildArgs += "-p:ModsDir=$ModsDir" }
if ($Deploy)      { $buildArgs += '-p:DeployMod=true' }

& $dotnet @buildArgs
if ($LASTEXITCODE -ne 0) { throw "build failed ($LASTEXITCODE)" }

# Pack localization assets into Viridis.pck
$packer = Join-Path $repo 'tools\pck_pack.py'
$pck = Join-Path $root 'bin\Viridis.pck'
& python $packer $pck 'Viridis' (Join-Path $root 'assets')
if ($LASTEXITCODE -ne 0) { throw "pck packing failed ($LASTEXITCODE)" }

if ($Deploy) {
    $target = $ModsDir
    New-Item -ItemType Directory -Force (Join-Path $target 'Viridis') | Out-Null
    Copy-Item $pck (Join-Path $target 'Viridis\Viridis.pck') -Force
    Write-Host "Deployed to: $target\Viridis"
}

Write-Host "Done. Output: $pck"