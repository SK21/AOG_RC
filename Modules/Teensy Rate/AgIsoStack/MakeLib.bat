@echo off
rem Installs the AgIsoStack library that RCteensy ISOBUS builds need. Double-click to run.
rem Does nothing when the installed library is already at the tested commit (PINNED in
rem make_lib.sh). Otherwise gets AgIsoStack++ at that commit into D:\Temp\AgIsoStack-plus-plus
rem (or %TEMP%\AgIsoStack-plus-plus when D:\Temp doesn't exist), installs the library into the
rem Arduino sketchbook's libraries folder, and deletes the download again.
rem   MakeLib.bat /force          reinstall even when the installed library is current
rem   MakeLib.bat [/force] <dir>  use this AgIsoStack++ checkout instead (it is not deleted)
rem Needs Git for Windows (Git Bash) and the Arduino IDE.
setlocal

rem before the shift below, which also moves %0
set "HERE=%~dp0"
set "SCRIPT=%HERE:\=/%make_lib.sh"

set "FORCE="
if /i "%~1"=="/force" (
	set "FORCE=1"
	shift
)

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
set "LIB=%SKETCHBOOK%\libraries\AgIsoStack"

rem the commit the installed library was made from: library.properties "sentence=AgIsoStack++ <commit> ..."
set "INSTALLED="
if exist "%LIB%\library.properties" (
	for /f "tokens=2" %%C in ('findstr /b "sentence=" "%LIB%\library.properties"') do set "INSTALLED=%%C"
)
if "%INSTALLED%"=="%PINNED%" if not defined FORCE (
	echo AgIsoStack %PINNED% is already installed in %LIB%
	echo Nothing to do. Use MakeLib.bat /force to reinstall it.
	goto done
)

rem a checkout given by the user is kept; the default download is deleted after the install
set "OWNSRC="
if not "%~1"=="" (
	set "SRC=%~f1"
) else (
	set "OWNSRC=1"
	if exist "D:\Temp\" (
		set "SRC=D:\Temp\AgIsoStack-plus-plus"
	) else (
		set "SRC=%TEMP%\AgIsoStack-plus-plus"
	)
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

echo Installing the library into %LIB%
"%BASH%" "%SCRIPT%" "%SRC:\=/%" "%LIB:\=/%"
if errorlevel 1 (
	echo The library was not installed. See the message above.
	goto done
)

if defined OWNSRC (
	echo Deleting the download %SRC%
	rmdir /s /q "%SRC%"
)
echo.
echo Done. Restart the Arduino IDE or Visual Micro to pick up the library.

:done
echo.
pause
