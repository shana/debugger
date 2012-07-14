#$/bin/sh
BIN="$HOME/Documents/Code Editor/Assets/Editor/bin/"
mkdir -p "$BIN"
for j in `ls bin/Debug/*.dll|grep -v Cecil\|NRefactory\|nunit`;do cp $j "$BIN";done
