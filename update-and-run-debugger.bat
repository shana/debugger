@echo off
set BUILD="src\CodeEditor.Debugger\Standalone\Build"
set BIN="%BUILD%\Debugger_Data\Managed"
del %BIN%\CodeEditor*.dll
xcopy src\CodeEditor.Debugger\bin\Debug\CodeEditor*.dll %BIN%
%BUILD%\Debugger.exe