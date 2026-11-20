@echo off
rem Builds both RCteensy release hex files: double-click, or drag a folder onto this file.
rem   RCteensy.ino.hex     normal build
rem   RCteensy_ISOBUS.hex  ISOBUS build
rem Output: the folder given, else D:\Temp\RCteensy_release, else %TEMP%\RCteensy_release.
rem Needs Git for Windows (Git Bash), the Arduino IDE with Teensyduino, and the AgIsoStack
rem library (AgIsoStack\MakeLib.bat). Only its own files in the output folder are replaced.
setlocal

set "HERE=%~dp0"
set "SCRIPT=%HERE:\=/%AgIsoStack/build_release.sh"

set "BASH="
if exist "%ProgramFiles%\Git\bin\bash.exe" set "BASH=%ProgramFiles%\Git\bin\bash.exe"
if not defined BASH if exist "%LOCALAPPDATA%\Programs\Git\bin\bash.exe" set "BASH=%LOCALAPPDATA%\Programs\Git\bin\bash.exe"
if not defined BASH (
	echo Git Bash not found. Install Git for Windows from https://git-scm.com and run this again.
	goto done
)

if not "%~1"=="" (
	set "OUT=%~f1"
) else if exist "D:\Temp\" (
	set "OUT=D:\Temp\RCteensy_release"
) else (
	set "OUT=%TEMP%\RCteensy_release"
)

echo Building RCteensy release hex files into %OUT%
echo This takes about 8 minutes.
echo.
"%BASH%" "%SCRIPT%" "%OUT%"
if errorlevel 1 (
	echo.
	echo The build did not finish. See the message above, and normal.log / isobus.log in %OUT%
	goto done
)
explorer "%OUT%"

:done
echo.
pause
