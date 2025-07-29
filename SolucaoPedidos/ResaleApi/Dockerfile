FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src

COPY . .

RUN dotnet restore SolucaoPedidos.sln

RUN dotnet build SolucaoPedidos.sln -c Release --no-restore

FROM build-env AS publish
WORKDIR /src/ResaleApi
RUN dotnet publish "ResaleApi.csproj" -c Release -o /app/publish --no-build /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:80;https://+:443
ENTRYPOINT ["dotnet", "ResaleApi.dll"]
