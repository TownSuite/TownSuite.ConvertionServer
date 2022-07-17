#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
RUN apt update && apt upgrade -y
RUN apt install ghostscript -y
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TownSuite.ConversionServer.APISite/TownSuite.ConversionServer.APISite.csproj", "TownSuite.ConversionServer.APISite/"]
COPY ["TownSuite.ConversionServer.Common.Models/TownSuite.ConversionServer.Common.Models.csproj", "TownSuite.ConversionServer.Common.Models/"]
COPY ["TownSuite.ConversionServer.StandardServices/TownSuite.ConversionServer.StandardServices.csproj", "TownSuite.ConversionServer.StandardServices/"]
COPY ["TownSuite.ConversionServer.Utilities.GhostScript/TownSuite.ConversionServer.Utilities.GhostScript.csproj", "TownSuite.ConversionServer.Utilities.GhostScript/"]
COPY ["TownSuite.ConversionServer.Interfaces/TownSuite.ConversionServer.Interfaces.csproj", "TownSuite.ConversionServer.Interfaces/"]
COPY ["TownSuite.ConversionServer.Utilities.Newtonsoft/TownSuite.ConversionServer.Utilities.Newtonsoft.csproj", "TownSuite.ConversionServer.Utilities.Newtonsoft/"]
COPY ["TownSuite.ConversionServer.Utilities.TownSuiteLogging/TownSuite.ConversionServer.Utilities.TownSuiteLogging.csproj", "TownSuite.ConversionServer.Utilities.TownSuiteLogging/"]
COPY ["TownSuite.ConversionServer.Common/TownSuite.ConversionServer.Common.csproj", "TownSuite.ConversionServer.Common/"]
RUN dotnet restore "TownSuite.ConversionServer.APISite/TownSuite.ConversionServer.APISite.csproj"
COPY . .
WORKDIR "/src/TownSuite.ConversionServer.APISite"
RUN dotnet build "TownSuite.ConversionServer.APISite.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TownSuite.ConversionServer.APISite.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TownSuite.ConversionServer.APISite.dll"]