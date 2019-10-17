FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Isomorph/Isomorph.csproj", "Isomorph/"]
RUN dotnet restore "Isomorph/Isomorph.csproj"
COPY . .
WORKDIR "/src/Isomorph"
RUN dotnet build "Isomorph.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Isomorph.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Isomorph.dll"]