@echo off
rem StopTaskController.bat - stops AOG-TaskController (it has no window).
rem Started by RunTaskController.bat.

tasklist /fi "imagename eq AOG-TaskController.exe" | find /i "AOG-TaskController.exe" >nul
if errorlevel 1 (
	echo AOG-TaskController is not running.
	goto done
)

taskkill /im AOG-TaskController.exe >nul 2>&1
timeout /t 2 /nobreak >nul
tasklist /fi "imagename eq AOG-TaskController.exe" | find /i "AOG-TaskController.exe" >nul
if errorlevel 1 (
	echo AOG-TaskController stopped.
	goto done
)

rem it has no window to close, so it may need to be forced
taskkill /f /im AOG-TaskController.exe >nul 2>&1
timeout /t 1 /nobreak >nul
tasklist /fi "imagename eq AOG-TaskController.exe" | find /i "AOG-TaskController.exe" >nul
if errorlevel 1 (
	echo AOG-TaskController stopped.
) else (
	echo Could not stop AOG-TaskController.
)

:done
pause
