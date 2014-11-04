@ECHO OFF

REM This build script was copied from OnlineVideos and then later changed.
REM All credit for this script goes to OnlineVideos.
REM Thank you!
REM https://code.google.com/p/mp-onlinevideos2/

REM detect if BUILD_TYPE should be release or debug
if not %1!==Debug! goto RELEASE
:DEBUG
set BUILD_TYPE=Debug
goto START
:RELEASE
set BUILD_TYPE=Release
goto START


:START
REM Select program path based on current machine environment
set progpath=%ProgramFiles%
if not "%ProgramFiles(x86)%".=="". set progpath=%ProgramFiles(x86)%


REM set logfile where the infos are written to, and clear that file
mkdir Logs
set LOG=Logs\MP2_Build_%BUILD_TYPE%.log
echo. > %LOG%


echo.
echo Building AtmoLight
echo Build mode: %BUILD_TYPE%
echo.

"%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBUILD.exe" /target:Rebuild /property:VisualStudioVersion=12.0;Configuration=%BUILD_TYPE% "..\AtmoLight.MediaPortal2.sln" >> %LOG%

if %1!==Debug! goto END

echo Copying files to ..\Release\MP2\AtmoLight\

mkdir "..\Release"
mkdir "..\Release\MP2"
mkdir "..\Release\MP2\AtmoLight"
mkdir "..\Release\MP2\AtmoLight\Language"

copy "..\AtmoLight.Core\bin\Release\AtmoLight.Core.dll" "..\Release\MP2\AtmoLight\AtmoLight.Core.dll"
copy "..\AtmoLight.Core\bin\Release\Google.ProtocolBuffers.dll" "..\Release\MP2\AtmoLight\Google.ProtocolBuffers.dll"
copy "..\AtmoLight.MediaPortal2\bin\x86\Release\AtmoLight.MediaPortal2.dll" "..\Release\MP2\AtmoLight\AtmoLight.MediaPortal2.dll"
copy "..\AtmoLight.MediaPortal2\plugin.xml" "..\Release\MP2\AtmoLight\plugin.xml"
copy "..\AtmoLight.MediaPortal2\Resources\Language\strings_en.xml" "..\Release\MP2\AtmoLight\Language\strings_en.xml"

:END