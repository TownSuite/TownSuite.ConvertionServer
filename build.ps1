#!/usr/bin/env pwsh
$ErrorActionPreference = "Stop"
$CURRENTPATH=$pwd.Path

# Must install powershell:  https://learn.microsoft.com/en-us/powershell/scripting/install/install-ubuntu?view=powershell-7.2


rm -rf build
mkdir -p build
$GITHASH="$(git rev-parse --short HEAD)"
echo "$GITHASH" >> build/githash.txt

Write-Host "Building townsuite/conversionserver:latest" -ForegroundColor Green
cd "$CURRENTPATH/TownSuite.ConversionServer"
docker build --progress plain -f "$CURRENTPATH/TownSuite.ConversionServer/TownSuite.ConversionServer.APISite/Dockerfile" -t townsuite/conversionserver$GITHASH -m 4GB .
cd $CURRENTPATH


Write-Host "Building conversionserver.tar" -ForegroundColor Green
docker save townsuite/conversionserver$GITHASH -o "$CURRENTPATH/build/conversionserver.tar"
docker rmi "townsuite/conversionserver$GITHASH"
Write-Host "Finished conversionserver.tar" -ForegroundColor Green

