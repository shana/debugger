@echo off

set PROJECT_ROOT=%USERPROFILE%\Documents\Code Editor

set EDITOR_EXTENSIONS_DIR="%PROJECT_ROOT%\Assets\Editor\bin\"

rd /s /q %EDITOR_EXTENSIONS_DIR%
set BIN=bin\Debug
set CODE_EDITOR_LIBS=%BIN%\Boo*.dll %BIN%\CodeEditor.Collections.dll %BIN%\CodeEditor.Composition.dll %BIN%\CodeEditor.Grammars*.dll %BIN%\CodeEditor.IO.dll %BIN%\CodeEditor.Languages.Common.dll %BIN%\CodeEditor.Languages.Boo.dll %BIN%\CodeEditor.Languages.CSharp.dll %BIN%\CodeEditor.Languages.UnityScript.dll %BIN%\CodeEditor.Text.*.dll %BIN%\CodeEditor.Remoting.dll
FOR %%A IN (%CODE_EDITOR_LIBS%) DO xcopy /Y %%A %EDITOR_EXTENSIONS_DIR%

xcopy bin\Debug\CodeEditor.Debugger.Unity.EditorIntegration.dll %EDITOR_EXTENSIONS_DIR% /Y

set RESOURCES_DIR="%PROJECT_ROOT%\CodeEditor"
rd /s /q %RESOURCES_DIR%
mkdir %RESOURCES_DIR%

set DEBUGGER_BIN_DIR="src\CodeEditor.Debugger\Standalone\Build\Debugger_Data\Managed"
FOR %%A IN (%CODE_EDITOR_LIBS%) DO xcopy /Y %%A %DEBUGGER_BIN_DIR%

del src\CodeEditor.Debugger.Unity.Standalone\bin\Debug\UnityEngine.*
xcopy /Y src\CodeEditor.Debugger.Unity.Standalone\bin\Debug\*.dll %DEBUGGER_BIN_DIR%

set DEBUGGER_DIR=%RESOURCES_DIR%\Debugger\
xcopy /S /Y src\CodeEditor.Debugger\Standalone\Build\* %DEBUGGER_DIR%

