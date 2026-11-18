[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot 'Build-Portable.ps1')
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (!(Test-Path -LiteralPath $vswhere)) { throw 'Install Visual Studio 2026 with the .NET desktop development workload and Installer Projects extension.' }
$vsRoot = & $vswhere -latest -products '*' -version '[18.0,19.0)' -requires Microsoft.VisualStudio.Workload.ManagedDesktop -property installationPath
if (!$vsRoot) { throw 'Visual Studio 2026 with the .NET desktop development workload was not found.' }
$devenv = Join-Path $vsRoot 'Common7\IDE\devenv.com'
$logDir = Join-Path $PSScriptRoot 'artifacts'
New-Item -ItemType Directory -Path $logDir -Force | Out-Null
$log = Join-Path $logDir ('setup-build-' + (Get-Date -Format 'yyyyMMdd-HHmmss') + '.log')
$buildStartedUtc = [DateTime]::UtcNow
Push-Location $PSScriptRoot
try {
    & $devenv (Join-Path $PSScriptRoot 'CyberArkEPMRemovalTool.slnx') /Rebuild Release /Project CyberArkEPMRemovalTool.Setup /ProjectConfig Release /Out $log
    if ($LASTEXITCODE -ne 0) { throw ('Setup build failed. Review ' + $log) }
    $msi = Join-Path (Split-Path $PSScriptRoot -Parent) 'CyberArkEPMRemovalTool.Setup\Release\CyberArkEPMRemovalTool.Setup.msi'
    if (!(Test-Path -LiteralPath $msi)) { throw 'Setup build did not produce the MSI.' }
    if ((Get-Item -LiteralPath $msi).LastWriteTimeUtc -lt $buildStartedUtc) { throw ('Setup was skipped or no new MSI was produced. Check Installer Projects and ' + $log) }
    Write-Output ('Installer: ' + $msi)
} finally { Pop-Location }



