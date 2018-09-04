FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Drive/Drive.csproj", "Drive/"]
RUN dotnet restore "Drive/Drive.csproj"
COPY . .
WORKDIR "/src/Drive"
RUN dotnet build "Drive.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Drive.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Drive.dll"]