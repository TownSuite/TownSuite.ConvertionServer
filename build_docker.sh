#!/bin/bash
set -e # exit on first error
set -u # exit on using unset variable

DOCKER_REGISTRY=$1
echo "Using docker registry $DOCKER_REGISTRY"
echo "Building townsuite/ConversionServer:latest"
cd TownSuite.ConversionServer
docker build -f ./TownSuite.ConversionServer.APISite/Dockerfile -t townsuite/conversionserver --rm=true -m 2GB .
cd ..

GITHASH="$(git rev-parse --short HEAD)"

echo 'Tagging latest'
docker tag townsuite/conversionserver $DOCKER_REGISTRY/townsuite/conversionserver:latest
echo "Tagging $GITHASH"
docker tag townsuite/conversionserver $DOCKER_REGISTRY/townsuite/conversionserver:$GITHASH

echo 'Pushing'
docker push $DOCKER_REGISTRY/townsuite/conversionserver:latest
docker push $DOCKER_REGISTRY/townsuite/conversionserver:$GITHASH
