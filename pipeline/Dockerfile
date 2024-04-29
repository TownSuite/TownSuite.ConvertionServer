FROM mcr.microsoft.com/dotnet/sdk:8.0
RUN apt-get update && apt-get upgrade -y
RUN apt-get install ghostscript -y
WORKDIR /app

COPY . .

RUN dotnet publish "TownSuite.ConversionServer.Tests.GeneralTests/TownSuite.ConversionServer.Tests.GeneralTests.csproj" -c Debug -o /app/publish

ENTRYPOINT ["dotnet", "test", "/app/publish/TownSuite.ConversionServer.Tests.GeneralTests.dll", "--logger:nunit;LogFileName=result.xml"]