@echo off
rem Installs the AgIsoStack library that RCteensy ISOBUS builds need. Double-click to run.
rem Gets AgIsoStack++ at the tested commit (PINNED in make_lib.sh) into
rem D:\Temp\AgIsoStack-plus-plus (or %TEMP%\AgIsoStack-plus-plus when D:\Temp doesn't exist),
rem or uses the AgIsoStack++ checkout given as an argument, and installs the library into the
rem Arduino sketchbook's libraries folder. Needs Git for Windows (Git Bash) and the Arduino IDE.
setlocal

set "HERE=%~dp0"
set "SCRIPT=%HERE:\=/%make_lib.sh"

set "GITDIR="
if exist "%ProgramFiles%\Git\bin\bash.exe" set "GITDIR=%ProgramFiles%\Git"
if not defined GITDIR if exist "%LOCALAPPDATA%\Programs\Git\bin\bash.exe" set "GITDIR=%LOCALAPPDATA%\Programs\Git"
if not defined GITDIR (
	echo Git for Windows not found. Install it from https://git-scm.com and run this again.
	goto done
)
set "BASH=%GITDIR%\bin\bash.exe"
set "GIT=%GITDIR%\cmd\git.exe"

rem the tested AgIsoStack++ commit, from make_lib.sh
set "PINNED="
for /f "tokens=2 delims==	 " %%P in ('findstr /b "PINNED=" "%HERE%make_lib.sh"') do set "PINNED=%%P"
if not defined PINNED (
	echo Could not read PINNED from make_lib.sh.
	goto done
)

rem the Arduino sketchbook, from the Arduino IDE's preferences
set "SKETCHBOOK="
for /f "tokens=1,* delims==" %%A in ('findstr /b "sketchbook.path=" "%LOCALAPPDATA%\Arduino15\preferences.txt" 2^>nul') do set "SKETCHBOOK=%%B"
if not defined SKETCHBOOK (
	echo Arduino sketchbook not found. Open the Arduino IDE once, then run this again.
	goto done
)

if not "%~1"=="" (
	set "SRC=%~f1"
) else if exist "D:\Temp\" (
	set "SRC=D:\Temp\AgIsoStack-plus-plus"
) else (
	set "SRC=%TEMP%\AgIsoStack-plus-plus"
)

if not exist "%SRC%\.git\" (
	echo Getting AgIsoStack++ into %SRC% ...
	"%GIT%" clone https://github.com/Open-Agriculture/AgIsoStack-plus-plus.git "%SRC%"
	if errorlevel 1 (
		echo Could not get AgIsoStack++. Check the internet connection.
		goto done
	)
) else (
	"%GIT%" -C "%SRC%" fetch --quiet
)
"%GIT%" -C "%SRC%" -c advice.detachedHead=false checkout --quiet %PINNED%
if errorlevel 1 (
	echo Could not check out AgIsoStack++ %PINNED% in %SRC%.
	goto done
)

set "LIB=%SKETCHBOOK%\libraries\AgIsoStack"
echo Installing the library into %LIB%
"%BASH%" "%SCRIPT%" "%SRC:\=/%" "%LIB:\=/%"
if errorlevel 1 (
	echo The library was not installed. See the message above.
	goto done
)
echo.
echo Done. Restart the Arduino IDE or Visual Micro to pick up the library.

:done
echo.
pause
