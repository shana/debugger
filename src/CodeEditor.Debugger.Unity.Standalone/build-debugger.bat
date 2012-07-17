@echo off
SET PROJECTPATH=UnityProject\Project
SET BUILDPATH=UnityProject\Build\Windows
SET BIN=%PROJECTPATH%\Assets\bin

xcopy bin\Debug\*.dll %BIN%
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


..\..\..\..\build\WindowsEditor\Unity.exe -batchmode -projectpath "%ABS_PROJECTPATH%" -buildWindowsPlayer "%ABS_BUILDPATH%\Debugger.exe" -quit

del %BUILDPATH%\Debugger_Data\Managed\CodeEditor.*
del %BUILDPATH%\Debugger_Data\Managed\Mono.Debugger.Soft.dll
del %BUILDPATH%\Debugger_Data\Managed\Mono.Cecil.dll

del %BIN%\*.dll