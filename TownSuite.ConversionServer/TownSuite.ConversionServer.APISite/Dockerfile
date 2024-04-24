#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt update && apt upgrade -y
RUN apt install ghostscript -y
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TownSuite.ConversionServer.APISite/TownSuite.ConversionServer.APISite.csproj", "TownSuite.ConversionServer.APISite/"]
COPY ["TownSuite.ConversionServer.Common.Models/TownSuite.ConversionServer.Common.Models.csproj", "TownSuite.ConversionServer.Common.Models/"]
COPY ["TownSuite.ConversionServer.Interfaces/TownSuite.ConversionServer.Interfaces.csproj", "TownSuite.ConversionServer.Interfaces/"]
COPY ["TownSuite.ConversionServer.StandardServices/TownSuite.ConversionServer.StandardServices.csproj", "TownSuite.ConversionServer.StandardServices/"]
COPY ["TownSuite.ConversionServer.Common/TownSuite.ConversionServer.Common.csproj", "TownSuite.ConversionServer.Common/"]
COPY ["TownSuite.ConversionServer.Utilities.GhostScript/TownSuite.ConversionServer.Utilities.GhostScript.csproj", "TownSuite.ConversionServer.Utilities.GhostScript/"]
COPY ["TownSuite.ConversionServer.Utilities.Newtonsoft/TownSuite.ConversionServer.Utilities.Newtonsoft.csproj", "TownSuite.ConversionServer.Utilities.Newtonsoft/"]
COPY ["TownSuite.ConversionServer.Utilities.TownSuiteLogging/TownSuite.ConversionServer.Utilities.TownSuiteLogging.csproj", "TownSuite.ConversionServer.Utilities.TownSuiteLogging/"]
RUN dotnet restore "./TownSuite.ConversionServer.APISite/TownSuite.ConversionServer.APISite.csproj"
COPY . .
WORKDIR "/src/TownSuite.ConversionServer.APISite"
RUN dotnet build "./TownSuite.ConversionServer.APISite.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TownSuite.ConversionServer.APISite.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TownSuite.ConversionServer.APISite.dll"]