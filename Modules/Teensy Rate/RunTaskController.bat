@echo off
rem Runs AOG-TaskController (the ISOBUS Task Controller) on the PCAN-USB adapter, with its
rem output in this window and a log file. Double-click to run; Ctrl+C stops it.
rem The PCAN channel can be shared: the RC app can stay connected on the same adapter.
rem   RunTaskController.bat [path to AOG-TaskController.exe]
rem Default exe: F:\Documents\GitHub\AOG-TaskController\build-pcan\AOG-TaskController.exe
rem Logs: %APPDATA%\AOG-TaskController\logs (opened when it stops).
setlocal

set "EXE=F:\Documents\GitHub\AOG-TaskController\build-pcan\AOG-TaskController.exe"
if not "%~1"=="" set "EXE=%~f1"
if not exist "%EXE%" (
	echo AOG-TaskController.exe not found:
	echo   %EXE%
	echo Build it, or give its path: RunTaskController.bat "C:\path\AOG-TaskController.exe"
	goto done
)

rem it has no window: a copy started by double-clicking the exe runs unseen
tasklist /fi "imagename eq AOG-TaskController.exe" 2>nul | find /i "AOG-TaskController.exe" >nul
if not errorlevel 1 (
	echo AOG-TaskController is already running, possibly without a window.
	echo End it in Task Manager first, then run this again.
	goto done
)

echo Starting AOG-TaskController on PCAN-USB channel 1. Press Ctrl+C to stop.
echo.
rem from its own folder, where PCANBasic.dll is
for %%F in ("%EXE%") do pushd "%%~dpF"
"%EXE%" --can_adapter=peak-pcan --can_channel=1 --log2file
popd

echo.
echo AOG-TaskController stopped.
if exist "%APPDATA%\AOG-TaskController\logs\" explorer "%APPDATA%\AOG-TaskController\logs"

:done
echo.
pause
