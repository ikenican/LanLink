@echo off
rem Uses the compiler built into Windows, no SDK needed.
cd /d "%~dp0"
set F=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319
"%F%\csc.exe" /nologo /codepage:65001 /optimize /target:winexe /out:LanLink.exe /win32icon:app.ico /resource:Main.xaml,Main.xaml ^
  /r:"%F%\WPF\PresentationFramework.dll" /r:"%F%\WPF\PresentationCore.dll" /r:"%F%\WPF\WindowsBase.dll" ^
  /r:System.Xaml.dll /r:System.Windows.Forms.dll /r:System.Core.dll Program.cs
if errorlevel 1 exit /b 1

start /wait "" LanLink.exe --selftest
if errorlevel 1 (echo SELFTEST FAILED & exit /b 1)
echo Build OK: LanLink.exe
