#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["HlidacStatu.ClassificationRepair/HlidacStatu.ClassificationRepair.csproj", "HlidacStatu.ClassificationRepair/"]
COPY ["HlidacStatu.Q.Messages/HlidacStatu.Q.Messages.csproj", "HlidacStatu.Q.Messages/"]
COPY ["HlidacStatu.Q/HlidacStatu.Q.csproj", "HlidacStatu.Q/"]
RUN dotnet restore "HlidacStatu.ClassificationRepair/HlidacStatu.ClassificationRepair.csproj"
COPY "HlidacStatu.Q.Messages" "HlidacStatu.Q.Messages"
COPY "HlidacStatu.Q" "HlidacStatu.Q"
COPY "HlidacStatu.ClassificationRepair" "HlidacStatu.ClassificationRepair"
WORKDIR "/src/HlidacStatu.ClassificationRepair"
COPY ["HlidacStatu.ClassificationRepair/appsettings.json.sample", "appsettings.json"]
RUN dotnet build "HlidacStatu.ClassificationRepair.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HlidacStatu.ClassificationRepair.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HlidacStatu.ClassificationRepair.dll"]