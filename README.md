# Drive

[![](https://images.microbadger.com/badges/version/xupeiyao/drive.svg)](https://microbadger.com/images/xupeiyao/drive 'Get your own version badge on microbadger.com')

基於 Docker 技術的簡易檔案管理器

## 佈署

預設系統管理者帳號密碼如下

ID: `admin`

PASSWORD: `admin`

### Docker

```shell
docker run -d -p 80:80 xupeiyao/drive:1.0.1 mydrive
```

備註: 可以使用 volume 連結`/data`

## 前言

隨 VPS 在現在資訊系統環境部屬上的普及應用，日常中有許多服務大部分轉移使用 VPS
，在伺服器系統中常見的有 Windows 以及 Linux，在使用 Linux VPS 時在部屬服務時
需要使用許多方式將程式執行檔搬移至 VPS 上執行，常見的方式有架設 FTP 等方法，或
者直接安裝 XRDP，這在程式發布或上傳相關設定檔(如憑證)上較為不便。

## 規劃源起

本專案目標在於透過在 VPS 中使用 Docker 技術與其中的 Volume 串接技術，快速的部屬
基於 Web 的檔案管理器，提供服務相關設定檔案上傳與下載，由於 Docker 開箱即用的特
性，可以比過去使用 wget、curl 或 FTP 傳輸檔案的方式更加簡便。

## 規劃目標

本專案系統建置目標如下幾點：

1. 基於 Docker 佈署
2. 提供使用者管理，支援多使用者與兩種權限(一般、管理員)
3. 提供檔案目錄管理功能
4. 提供基於 Web 的使用者介面
5. 提供多檔上傳與檔案搬移

## 架構與開發環境

1. 系統架構
   1. 採用三層式(3-tier)架構
      1. 使用者介面層: 優先支援 Chrome 瀏覽器，採 Angular 進行開發前端介面。
      2. 運算邏輯層: 採用 ASP.net Core 技術運行，與使用者介面層的溝通採用 RESTful API 並搭配 Swagger 輔助開發，另外搭配 EntityFrameworkCore 技術與資料服務層溝通。
      3. 資料服務層: 採用 Sqlite，本專案目標為目標單一的檔案瀏覽器，並無規劃其他功能，但由於資料邏輯層與本層的界皆使用 EntityFrameworkCore，在未來更換資料庫時可以僅更換 Database Provider。
   2. 佈署環境
      本專案基於 Docker 技術，在佈署服務時僅需取得相應的 Docker 映像啟動後即可運作。
      另外，作業系統支援部分，如非 X64 架構環境需另行建置映像檔。
   3. 使用者驗證
      採用 JWT 驗證。
2. 開發環境
   本專案採用前後端分離並且透過 RESTful API 進行溝通的架構，故以下分為前後端兩點說明。
   1. 前端
      1. Visual Studio Code
      1. Node.js
      1. Angular & Angular CLI
   2. 後端
      1. .Net Core SDK 2.1.400
      2. Visual Studio Community 2017
3. 專案架構
   1. 後端專案架構:
      1. Drive: 後端主程式
      2. Drive.Base: 擴充方法與基礎類別
      3. Drive.FileSystem: 檔案系統操作類別庫
      4. Drive.Logic: 資料服務層操作類別庫
   2. 前端專案架構: Angular 專案

## 功能說明

1. 會員管理系統:
   包含使用者的登入、建立、變更、刪除操作。
2. 檔案管理系統:
   包含檔案與目錄的列表取得、搜尋、上傳、下載、搬移操作。

## 流程圖

1. 登入

![Imgur](https://i.imgur.com/flKNWPD.png)

2. 會員管理功能

![Imgur](https://i.imgur.com/IUulfki.png)

3. 新增使用者

![Imgur](https://i.imgur.com/EGFVWSK.png)

4. 變更使用者

![Imgur](https://i.imgur.com/GVn839R.png)

5. 刪除使用者

![Imgur](https://i.imgur.com/BmPFvg9.png)

6. 瀏覽目錄檔案列表

![Imgur](https://i.imgur.com/rUXcAOV.png)

7. 檔案搜尋

![Imgur](https://i.imgur.com/C4KWwTl.png)

8. 檔案上傳

![Imgur](https://i.imgur.com/7WMmA2s.png)

9. 檔案刪除

![Imgur](https://i.imgur.com/NSp1qK9.png)

10. 檔案搬移

![Imgur](https://i.imgur.com/DTV6DBP.png)

11. 檔案下載

![Imgur](https://i.imgur.com/ofBh7OF.png)
