docker build -t conversion-server -f .\Dockerfile ..\TownSuite.ConversionServer
docker run --rm --name townsuite.conversion-server conversion-server