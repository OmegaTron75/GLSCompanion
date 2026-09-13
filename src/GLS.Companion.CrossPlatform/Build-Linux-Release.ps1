$ErrorActionPreference = "Stop"

$ProjectRoot = "H:\Dev\GLS.Companion.CrossPlatform"
$Project = Join-Path $ProjectRoot "GLS.Companion.CrossPlatform.csproj"
$ReleaseRoot = Join-Path $ProjectRoot "release"
$LinuxPublish = Join-Path $ReleaseRoot "linux-x64"
$LinuxZip = Join-Path $ReleaseRoot "GLSCompanion-Linux-x64.zip"

Write-Host "=== GLS Companion Linux Release Build ===" -ForegroundColor Cyan

Set-Location $ProjectRoot

if (Test-Path $LinuxPublish) {
    Remove-Item $LinuxPublish -Recurse -Force
}

if (Test-Path $LinuxZip) {
    Remove-Item $LinuxZip -Force
}

New-Item -ItemType Directory -Force -Path $ReleaseRoot | Out-Null

dotnet restore $Project

dotnet publish $Project `
    -c Release `
    -r linux-x64 `
    --self-contained true `
    -o $LinuxPublish

if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed."
}

# Keep the release clean.
Get-ChildItem $LinuxPublish -Filter "*.pdb" -ErrorAction SilentlyContinue |
    Remove-Item -Force

Compress-Archive `
    -Path (Join-Path $LinuxPublish "*") `
    -DestinationPath $LinuxZip `
    -CompressionLevel Optimal

Write-Host ""
Write-Host "Linux release created:" -ForegroundColor Green
Write-Host $LinuxZip
Write-Host ""
Write-Host "Copy this ZIP to the Ubuntu test VM and perform the final clean install test."
