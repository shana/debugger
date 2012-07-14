@echo off
SET PROJECTPATH="src\CodeEditor.Debugger\Standalone\Project"
SET BUILDPATH="src\CodeEditor.Debugger\Standalone\Build"
SET BIN="%PROJECTPATH%\Assets\bin"

rd /s /q %BIN%
mkdir %BIN%
xcopy src\CodeEditor.Debugger.Unity.Standalone\bin\Debug\* %BIN%
del %BIN%\UnityEngine.dll

rd /s /q %BUILDPATH%
mkdir %BUILDPATH%

set REL_PATH=%PROJECTPATH%
set ABS_PROJECTPATH=
pushd .
cd %REL_PATH%
set ABS_PROJECTPATH=%CD%
popd

set REL_PATH=%BUILDPATH%
set ABS_BUILDPATH=
pushd .
cd %REL_PATH%
set ABS_BUILDPATH=%CD%
popd


..\..\build\WindowsEditor\Unity.exe -batchmode -projectpath %ABS_PROJECTPATH% -buildWindowsPlayer %ABS_BUILDPATH%\Debugger.exe -quit
