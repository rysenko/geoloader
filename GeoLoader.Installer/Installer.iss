#define AppName "GeoLoader"
#define AppFileName "GeoLoader.exe"
#define SrcDir "..\GeoLoader\bin\Release\"
#define SrcApp SrcDir + AppFileName
#define AppVerStr GetFileVersion(SrcApp)

[Setup]
AppName={#AppName}
AppVersion={#AppVerStr}
AppVerName={#AppName} {#AppVerStr}
UninstallDisplayName={#AppName} {#AppVerStr}
VersionInfoVersion={#AppVerStr}
VersionInfoTextVersion={#AppVerStr}
OutputBaseFilename=GeoLoader-{#AppVerStr}
DefaultDirName={pf}\GeoLoader
DisableReadyPage=yes
DisableProgramGroupPage=yes
Compression=lzma2
SolidCompression=yes
UninstallDisplayIcon={app}\{#AppFileName}
AppPublisher=Alexander Rysenko
AppPublisherURL=http://rysenko.com/

[Files]
Source: {#SrcApp}; DestDir: "{app}"

[Icons]
Name: "{commonprograms}\GeoLoader"; Filename: "{app}\{#AppFileName}"; WorkingDir: "{app}"

[Languages]
Name: "ru"; MessagesFile: "compiler:Languages\Russian.isl"