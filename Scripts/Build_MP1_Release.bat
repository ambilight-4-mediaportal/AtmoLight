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
set LOG=Logs\MP1_Build_%BUILD_TYPE%.log
echo. > %LOG%


echo.
echo Building AtmoLight
echo Build mode: %BUILD_TYPE%
echo.

@"%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBUILD.exe" NuGet/RestorePackages.targets

"%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBUILD.exe" /target:Rebuild /property:VisualStudioVersion=12.0;Configuration=%BUILD_TYPE% "..\AtmoLight.MediaPortal1.sln" >> %LOG%

if %1!==Debug! goto END

echo Building MPEI
copy "..\MPEI\AtmoLight.xmp2" "..\MPEI\AtmoLight_COPY.xmp2"
"%progpath%\Team MediaPortal\MediaPortal\MpeMaker.exe" "..\MPEI\AtmoLight_COPY.xmp2" /B >> %LOG%
del "..\MPEI\AtmoLight_COPY.xmp2"

:END