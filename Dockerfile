#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster as build
WORKDIR /src
COPY ["identity-server4-localdev.csproj", "src/"]
RUN dotnet restore "src/identity-server4-localdev.csproj"
COPY . .
WORKDIR "/src/"
RUN rm -rf ./config

FROM build AS publish
RUN dotnet publish -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "identity-server4-localdev.dll"]