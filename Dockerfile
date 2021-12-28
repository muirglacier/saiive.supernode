FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
LABEL vendor="p3-software & line-of-code"
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY src .
RUN dotnet restore
COPY . .
WORKDIR "/src/Saiive.Supernode"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

EXPOSE 5000
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Saiive.SuperNode.dll"]