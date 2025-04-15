@echo off
echo Copying ImpleoTV DLLs to output directory...

setlocal

REM Get the directory where this batch file is located
set "SCRIPT_DIR=%~dp0"
set "SOURCE_DIR=%SCRIPT_DIR%ImpleoTV"
set "DEST_DIR=%SCRIPT_DIR%bin\x64\Debug\net7.0-windows"

REM Create destination directory if it doesn't exist
if not exist "%DEST_DIR%" mkdir "%DEST_DIR%"

REM Copy all files from ImpleoTV to the output directory
xcopy "%SOURCE_DIR%\*.*" "%DEST_DIR%" /E /Y /I

echo Copy operation completed successfully!

endlocal 