# Primary workspace

This project and CyberArkEPMRemovalTool.Setup use the primary workspace on
192.168.1.101. From the iy52648 workstation it is Y:\, mapped to
\\192.168.1.101\Users\Keith\source\repos. On that computer, the native
workspace is C:\Users\Keith\source\repos.

The copies under C:\Users\iy52648\source\repos are secondary copies.
For future work, inspect and edit the primary source first, then run
Y:\CyberArkEPMRemovalTool\Sync-LocalCopy.ps1 on the iy52648 workstation.
Compare unexpected differences before overwriting either source copy.
Do not automatically reverse synchronization from the secondary copy.

Use Build-Portable.ps1 in the primary app folder for the self-contained
Windows x64 application. Its output is publish\CyberArkEPMRemovalTool.exe.
Build-Setup.ps1 additionally requires the VS 2026 Installer Projects extension.
See PRIMARY-WORKSPACE.txt in the app folder for the complete build workflow.
Keep bin, obj, .vs, and per-machine IDE settings separate on each computer.
Synchronization is manual; no background or scheduled sync is configured.
