FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WebAppServer/WebAppServer.csproj", "WebAppServer/"]
RUN dotnet restore "WebAppServer/WebAppServer.csproj"
COPY . .
WORKDIR "/src/WebAppServer"
RUN dotnet build "WebAppServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebAppServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebAppServer.dll"]
