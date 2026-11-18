[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    & dotnet publish (Join-Path $PSScriptRoot 'CyberArkEPMRemovalTool.csproj') -c Release -p:PublishProfile=FolderProfile1 --nologo
    if ($LASTEXITCODE -ne 0) { throw 'Portable publish failed.' }
    $exe = Join-Path $PSScriptRoot 'publish\CyberArkEPMRemovalTool.exe'
    if (!(Test-Path -LiteralPath $exe)) { throw 'Published executable is missing.' }
    $hash = (Get-FileHash -LiteralPath $exe -Algorithm SHA256).Hash
    ($hash + '  CyberArkEPMRemovalTool.exe') | Set-Content -LiteralPath (Join-Path $PSScriptRoot 'publish\SHA256SUMS.txt') -Encoding ascii
    Write-Output ('Portable application: ' + $exe)
} finally { Pop-Location }
