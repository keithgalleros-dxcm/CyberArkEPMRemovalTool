[CmdletBinding()]
param(
    [string]$PrimaryReposPath = (Split-Path $PSScriptRoot -Parent),
    [string]$LocalReposPath = 'C:\Users\iy52648\source\repos'
)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression.FileSystem
$primary = [IO.Path]::GetFullPath($PrimaryReposPath).TrimEnd('\')
$local = [IO.Path]::GetFullPath($LocalReposPath).TrimEnd('\')
if ($primary -eq $local) { throw 'Primary and local folders must be different. Run this script from the primary workspace.' }
$projects = @('CyberArkEPMRemovalTool', 'CyberArkEPMRemovalTool.Setup')
foreach ($name in $projects) {
    if (!(Test-Path -LiteralPath (Join-Path $primary $name))) { throw ('Primary project missing: ' + $name) }
}
function Get-ProjectFiles([string]$Root) {
    $pending = New-Object 'System.Collections.Generic.Stack[string]'
    $pending.Push($Root)
    while ($pending.Count -gt 0) {
        foreach ($item in Get-ChildItem -LiteralPath $pending.Pop() -Force) {
            if ($item.PSIsContainer) {
                if ($item.Name -notin @('bin','obj','.vs','.git','.codex','.agents','artifacts','Debug','Release')) { $pending.Push($item.FullName) }
            } elseif ($item.Name -notmatch '\.(user|suo)$') { $item }
        }
    }
}
$backupDir = Join-Path $local 'CyberArkEPMRemovalTool\artifacts\sync-backups'
New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
$backupPath = Join-Path $backupDir ('before-sync-' + (Get-Date -Format 'yyyyMMdd-HHmmss-fff') + '.zip')
$backup = [IO.Compression.ZipFile]::Open($backupPath, [IO.Compression.ZipArchiveMode]::Create)
$count = 0
try {
    foreach ($name in $projects) {
        $sourceRoot = Join-Path $primary $name
        $targetRoot = Join-Path $local $name
        foreach ($file in Get-ProjectFiles $sourceRoot) {
            $relative = $file.FullName.Substring($sourceRoot.Length + 1)
            $target = Join-Path $targetRoot $relative
            $sourceHash = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash
            if ((Test-Path -LiteralPath $target) -and ((Get-FileHash -LiteralPath $target -Algorithm SHA256).Hash -eq $sourceHash)) { continue }
            if (Test-Path -LiteralPath $target) {
                [IO.Compression.ZipFileExtensions]::CreateEntryFromFile($backup, $target, ($name + '/' + $relative.Replace('\','/'))) | Out-Null
            }
            New-Item -ItemType Directory -Path (Split-Path $target -Parent) -Force | Out-Null
            Copy-Item -LiteralPath $file.FullName -Destination $target -Force
            if ((Get-FileHash -LiteralPath $target -Algorithm SHA256).Hash -ne $sourceHash) { throw ('Hash mismatch: ' + $target) }
            $count++
        }
    }
} finally { $backup.Dispose() }
Write-Output ('Updated and hash-verified ' + $count + ' files from ' + $primary + ' to ' + $local)
Write-Output ('Replaced files backed up to ' + $backupPath)
Write-Output 'Local-only files and per-machine Visual Studio settings were preserved.'

