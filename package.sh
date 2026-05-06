#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")"

VERSION=$(jq -r '.version' modinfo.json)
NAME=$(jq -r '.modid' modinfo.json)
OUT="dist/${NAME}-${VERSION}.zip"

dotnet build -c Release --nologo --verbosity quiet

mkdir -p dist
rm -f "$OUT"
zip -j "$OUT" "bin/Release/${NAME}.dll" modinfo.json

echo "Packaged $OUT"
