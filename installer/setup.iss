#define MyAppName "r4dio"
#define MyAppVersion "0.1"
#define MyAppPublisher "Torstein Auensen"
#define MyAppURL "http://www.torsh.net/blog"
#define MyAppExeName "Torshify.Radio.exe"

#include "dotnet4.iss"

[Setup]
AppId={{1189D461-E642-4C47-9690-2AF1469688F5}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputBaseFilename=setup
Compression=lzma
SolidCompression=yes
PrivilegesRequired=poweruser

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Dirs]
Name: "{app}\Modules"
Name: "{app}\Modules\EchoNest"
Name: "{app}\Modules\EightTracks"
Name: "{app}\Modules\Grooveshark"
Name: "{app}\Modules\Spotify"

[Files]
Source: "..\src\Torshify.Radio\bin\Release\Torshify.Radio.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\Torshify.Radio\bin\Release\Torshify.Radio.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\Torshify.Radio\bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\Torshify.Radio\bin\Release\Resources\LargeIcons\*.*"; DestDir: "{app}\Resources\LargeIcons"; Flags: ignoreversion recursesubdirs
Source: "..\src\Torshify.Radio\bin\Release\Modules\EchoNest\*.*"; Excludes: "*.pdb, *.xml"; DestDir: "{app}\Modules\EchoNest"; Flags: ignoreversion recursesubdirs
Source: "..\src\Torshify.Radio\bin\Release\Modules\EightTracks\*.*"; Excludes: "*.pdb, *.xml"; DestDir: "{app}\Modules\EightTracks"; Flags: ignoreversion recursesubdirs
Source: "..\src\Torshify.Radio\bin\Release\Modules\Grooveshark\*.*"; Excludes: "*.pdb, *.xml"; DestDir: "{app}\Modules\Grooveshark"; Flags: ignoreversion recursesubdirs
Source: "..\src\Torshify.Radio\bin\Release\Modules\Spotify\*.*"; Excludes: "*.pdb, *.xml"; DestDir: "{app}\Modules\Spotify"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, "&", "&&")}}"; Flags: nowait postinstall skipifsilent runascurrentuser

[Code]
function InitializeSetup(): Boolean;
begin
    if not IsDotNetDetected('v4\Full', 0) then begin
        MsgBox('Torshify requires Microsoft .NET Framework 4.0.'#13#13
            'Please use install this version,'#13
            'and then re-run the Torshify setup program.', mbInformation, MB_OK);
        result := false;
    end else
        result := true;
end;
