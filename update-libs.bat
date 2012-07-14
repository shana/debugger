@echo off

REM NAnt 0.91 (Build 0.91.4312.0; release; 10/22/2011)
nant -f:libs.build -t:net-3.5

cd CodeEditor.Grammars
call build.bat
cd ..
