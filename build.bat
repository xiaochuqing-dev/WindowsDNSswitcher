@echo off
setlocal

set "DOTNET_EXE=dotnet"
if exist "%~dp0work\.dotnet\dotnet.exe" set "DOTNET_EXE=%~dp0work\.dotnet\dotnet.exe"

if exist "%~dp0dist" rmdir /s /q "%~dp0dist"
"%DOTNET_EXE%" publish "%~dp0src\WindowsDnsSwitcher.WinForms\WindowsDnsSwitcher.WinForms.csproj" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false -o "%~dp0dist"
if errorlevel 1 exit /b %errorlevel%

endlocal


