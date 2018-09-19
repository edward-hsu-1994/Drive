# 使用ASP.net Core執行階段環境，命名為base
FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
# 切換與建立工作目錄/app
WORKDIR /app
# 對外80 Port
EXPOSE 80

# 使用.Net Core 2.1 SDK環境，命名為build
FROM microsoft/dotnet:2.1-sdk AS build
# 切換與建立工作目錄/src
WORKDIR /src
# 複製專案檔
COPY ["Drive/Drive.csproj", "Drive/"]
# 還原套件
RUN dotnet restore "Drive/Drive.csproj"
# 複製原始碼文件
COPY . .
# 切換與建立工作目錄/src/Drive
WORKDIR "/src/Drive"
# 建置Drive專案
RUN dotnet build "Drive.csproj" -c Release -o /app

# 使用build環境，命名為publish
FROM build AS publish
# 發行Drive專案
RUN dotnet publish "Drive.csproj" -c Release -o /app

# 使用node環境，命名為frontendbuild
FROM node as frontendbuild
# 切換與建立工作目錄/frontend
WORKDIR /frontend
#複製Angular專案原始碼
COPY DriveFrontend .
# 安裝套件
RUN npm i 
# 建置
RUN npm run build

# 使用base環境，命名為final
FROM base AS final
# 宣告Volume作為預設資料目錄
VOLUME /data
# 預設檔案目錄環境變數
ENV RootDirectory=/data
# 切換與建立工作目錄/app
WORKDIR /app
# 自publish環境複製發行結果
COPY --from=publish /app .
# 自frontendbuild環境複製Angular專案建置結果至wwwroot目錄
COPY --from=frontendbuild /frontend/dist/DriveFrontend ./wwwroot
# 容器進入點
ENTRYPOINT ["dotnet", "Drive.dll"]
