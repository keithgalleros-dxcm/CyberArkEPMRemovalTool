CyberArk EPM Agent Removal Utility 1.1.0
Portable Windows x64 release

DESCRIPTION
Administrative utility that detects registered CyberArk EPM agents, requests
full removal through Windows Installer, and checks whether the selected
product registration remains.

DEVELOPER
Name: Keith Galleros
Title: Staff Cybersecurity Engineer
Department: IT-Infosec-Manila
Company: DexCom Philippines Inc

RUN
Extract the ZIP, then run CyberArkEPMRemovalTool.exe and accept the Windows
administrator prompt. No application installer or separate .NET Desktop
Runtime installation is required. The bundled native runtime extracts to
the current user's temporary .net directory at startup.

Review the detected agent and product GUID before choosing Full Uninstall.
Supply a valid secure token when agent self-defense requires one. The
confirmation defaults to No. Windows Installer is invoked with /norestart;
restart Windows after successful removal before reinstalling the agent.

The About button shows the application description and developer details.
Logs are written to %ProgramData%\CyberArkEPMRemovalTool\Logs.
This is a portable executable, but it still writes logs and runtime cache
files to the computer.

VERIFICATION LIMITS
The utility checks 32-bit and 64-bit machine uninstall registry views.
If multiple products match, it displays the first valid MSI registration.
Review that selection before uninstalling. Removal verification checks the
selected registration only; it does not prove that every file, service,
driver, or other EPM product has been removed.

The main window has a fixed 1050 x 990 client area and needs a sufficiently
large display at the configured Windows scaling level.

BUILD
Install the .NET 10 SDK (10.0.403 or a later stable 10.0 feature band).
From the project folder, run:
  dotnet publish CyberArkEPMRemovalTool.csproj -p:PublishProfile=FolderProfile1

Output: publish\CyberArkEPMRemovalTool.exe

RELEASE CHANGES
- Added About with the developer details and application description.
- Updated company and developer assembly metadata; version is now 1.1.0.
- Both publish profiles now produce a self-contained single executable.
- Rescanning clears the previously selected agent before discovery starts.
- The secure-token input is cleared even when uninstalling raises an error.

PRIMARY AND SECONDARY WORKSPACES
See PRIMARY-WORKSPACE.txt for the Y: primary folder, Visual Studio 2026
build commands, and the command to update the local secondary copies.

