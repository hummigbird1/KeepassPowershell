$binaryFolder = "$PSScriptRoot\..\KeepassPSCmdlets\bin\Release"
$buildOutputFolder = "$PSScriptRoot\..\..\build"

$null = New-Item -Path $buildOutputFolder -Force -ItemType Directory

Compress-Archive -Path "$binaryFolder\*.dll" -CompressionLevel Optimal -DestinationPath "$buildOutputFolder\KeepassPSCmdlets.zip" -Force

Copy-Item -Path "$PSScriptRoot\Installer.ps1"  -Destination $buildOutputFolder

Compress-Archive -Path "$buildOutputFolder\*.*" -CompressionLevel Optimal -DestinationPath "$buildOutputFolder\KeepassPSCmdlets_Release.zip" -Force