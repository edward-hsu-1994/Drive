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

# 建構前端Angular專案
FROM node as frontendbuild
WORKDIR /frontend
#複製Angular專案原始碼
COPY DriveFrontend .
#安裝套件
RUN npm i 
#建置
RUN npm run build

FROM base AS final
# 預設檔案目錄
VOLUME /data
# 預設檔案目錄
ENV RootDirectory=/data
WORKDIR /app
COPY --from=publish /app .
COPY --from=frontendbuild /frontend/dist/DriveFrontend ./wwwroot
ENTRYPOINT ["dotnet", "Drive.dll"]