#define MyAppName "r4dio"
#define MyAppPublisher "Torstein Auensen"
#define MyAppURL "http://www.torsh.net/blog"
#define MyAppExeName "r4dio.exe"

#define SetupBaseName "r4dio_setup_"
#define AppVersion      GetFileVersion("..\src\Torshify.Radio\bin\Release\r4dio.exe")
#define AVF1            Copy(AppVersion, 1, Pos(".", AppVersion) - 1) + "_" + Copy(AppVersion, Pos(".", AppVersion) + 1)
#define AVF2            Copy(AVF1,       1, Pos(".", AVF1      ) - 1) + "_" + Copy(AVF1      , Pos(".", AVF1      ) + 1)
#define AppVersionFile  Copy(AVF2,       1, Pos(".", AVF2      ) - 1) + "_" + Copy(AVF2      , Pos(".", AVF2      ) + 1)

#include "dotnet4.iss"

[Setup]
AppId={{1189D461-E642-4C47-9690-2AF1469688F5}
AppName={#MyAppName}
AppVersion={#AppVersionFile}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
SetupIconFile=..\src\Torshify.Radio\Resources\r4dio_app.ico
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
Compression=lzma
SolidCompression=yes
PrivilegesRequired=poweruser
UninstallDisplayIcon={app}\r4dio.exe
OutputBaseFilename={#SetupBaseName + AppVersionFile}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Components]
Name: "main"; Description: "Main Files"; Types: full compact custom; Flags: fixed
Name: "spotify"; Description: "Spotify support"; Types: full

[Files]
Source: "..\src\Torshify.Radio\bin\Release\r4dio.exe"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\src\Torshify.Radio\bin\Release\r4dio.exe.config"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\src\Torshify.Radio\bin\Release\*.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\src\Torshify.Radio\bin\Release\Resources\*.*"; DestDir: "{app}\Resources\"; Components: main; Flags: ignoreversion recursesubdirs
Source: "..\src\Torshify.Radio\Resources\*.ico"; DestDir: "{app}\Resources\"; Components: main; Flags: ignoreversion recursesubdirs
Source: "..\src\Torshify.Radio\bin\Release\Modules\8tracks\*.*"; Excludes: "*.pdb, *.xml"; DestDir: "{app}\Modules\8tracks"; Components: main; Flags: ignoreversion recursesubdirs
Source: "..\src\Torshify.Radio\bin\Release\Modules\Core\*.*"; Excludes: "*.pdb, *.xml"; DestDir: "{app}\Modules\Core"; Components: main; Flags: ignoreversion recursesubdirs
Source: "..\src\Torshify.Radio\bin\Release\Modules\Database\*.*"; Excludes: "*.pdb, *.xml"; DestDir: "{app}\Modules\Database"; Components: main; Flags: ignoreversion recursesubdirs
Source: "..\src\Torshify.Radio\bin\Release\Modules\EchoNest\*.*"; Excludes: "*.pdb, *.xml"; DestDir: "{app}\Modules\EchoNest"; Components: main; Flags: ignoreversion recursesubdirs
Source: "..\src\Torshify.Radio\bin\Release\Modules\Grooveshark\*.*"; Excludes: "*.pdb"; DestDir: "{app}\Modules\Grooveshark"; Components: main; Flags: ignoreversion recursesubdirs
Source: "..\src\Torshify.Radio\bin\Release\Modules\Spotify\*.*"; Excludes: "*.pdb, *.xml"; DestDir: "{app}\Modules\Spotify"; Components: spotify; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\Resources\r4dio_app.ico"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; IconFilename: "{app}\Resources\r4dio_app.ico"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\Resources\r4dio_app.ico"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\Resources\r4dio_app.ico"; Tasks: quicklaunchicon

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
