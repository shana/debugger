@echo off

set DEBUGGER_BIN_DIR="..\..\..\..\build\WindowsEditor\Data\Tools\Debugger\Debugger_Data\Managed\"

set BIN=..\CodeEditor.Debugger.Unity.Standalone.Dependencies\bin\Debug

del %BIN%\UnityEngine.*
del %BIN%\UnityEditor.*
xcopy /Y %BIN%\*.dll %DEBUGGER_BIN_DIR%