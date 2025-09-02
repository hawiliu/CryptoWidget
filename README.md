# CryptoWidget

一個輕量級的加密貨幣價格監控小工具，使用 Avalonia UI 框架開發的跨平台桌面應用程式。

![CryptoWidget](Assets/cryptowidget-logo.ico)

## 📱 截圖

![MainWindow](Readme/mainWindow.PNG)

## 🌟 功能特色

### 📊 即時價格監控
- **多個交易所支援**：依照ccxt支援類型

### 🎨 使用者介面
- **透明視窗**：可調整透明度，支援 10%-100% 透明度設定
- **置頂顯示**：可設定視窗保持最上層
- **無邊框設計**：現代化的無邊框視窗設計
- **系統匣支援**：支援最小化到系統匣，雙擊圖示可重新顯示

### ⚙️ 設定功能
- **自訂幣種**：可自由添加/移除監控的加密貨幣
- **交易所選擇**：可選擇不同的交易所作為價格來源
- **關閉行為**：可設定點擊關閉按鈕時隱藏或完全關閉應用程式

### 🔧 技術特色
- **跨平台**：基於 Avalonia UI 框架，支援 Windows、macOS、Linux（Windows以外的作業系統未測試，可能存在多個異常）
- **MVVM 架構**：採用 MVVM 設計模式，程式碼結構清晰

## 📋 系統需求

- **作業系統**：Windows 10/11、macOS 10.15+、Linux (Ubuntu 18.04+)
- **.NET Runtime**：.NET 8.0 或更新版本

## 🚀 執行

### 下載預編譯版本
1. 前往 [Releases](https://github.com/hawiliu/CryptoWidget/releases) 頁面
2. 下載最新版本的執行檔
3. 解壓縮並執行 `CryptoWidget.exe`

**註. Windows以外的作業系統未測試，可能存在多個異常**

## 📖 使用說明

### 基本操作
1. **啟動應用程式**：執行後會顯示主視窗，預設監控 BTC/USDT
2. **查看價格**：價格會每 5 秒自動更新一次
3. **開啟設定**：點擊右上角齒輪圖示開啟設定視窗
4. **最小化**：點擊右上角 X 按鈕會隱藏到系統匣
5. **系統匣操作**：右鍵點擊系統匣圖示可開啟選單，雙擊可重新顯示視窗

### 設定說明
- **視窗透明度**：調整視窗透明度，範圍 10%-100%
- **保持最上層**：勾選後視窗會保持在最上層
- **關閉行為**：選擇點擊關閉按鈕時隱藏或完全關閉
- **交易所選擇**：從下拉選單選擇價格來源交易所
- **新增幣種**：在輸入框中輸入幣種代碼（如 BTC 或 BTC/USDT），點擊 ➕ 按鈕新增
- **移除幣種**：點擊幣種旁的 ✖ 按鈕移除

### 支援的幣種格式
- **簡短格式**：BTC、ETH、ADA（會自動補上 /USDT）
- **完整格式**：BTC/USDT、ETH/USDT、ADA/USDT
- **合約格式**：BTC:USDT、ETH:USDT（部分交易所支援）

## 🔧 技術架構

### 前端框架
- **Avalonia UI**：跨平台 UI 框架
- **MVVM 模式**：Model-View-ViewModel 架構
- **CommunityToolkit.Mvvm**：MVVM 工具庫

### 後端服務
- **Microsoft.Extensions.Hosting**：依賴注入容器
- **AutoMapper**：物件對應工具
- **ccxt**：加密貨幣交易所 API 庫

## 🔧 開發環境設定
1. 安裝 .NET 8.0 SDK
2. 安裝 Visual Studio 2022 或 VS Code
3. 複製專案並開啟解決方案
4. 還原 NuGet 套件
5. 建置並執行專案
6. (可選)執行publish.bat建置各平台執行檔

## 📄 授權條款

本專案採用 MIT 授權條款

## 🙏 致謝

- [Avalonia UI](https://avaloniaui.net/) - 跨平台 UI 框架
- [ccxt](https://github.com/ccxt/ccxt) - 加密貨幣交易所 API 庫
- [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) - MVVM 工具庫

## 🖼️ 圖示授權

應用程式圖示來源於 [icon-icons.com](https://icon-icons.com/icon/usd-crypto-cryptocurrency-cryptocurrencies-cash-money-bank-payment/95103)

**App icon © Christopher Downer** — sourced from icon-icons.com

---

⭐ 如果這個專案對您有幫助，請給我一個星標！ 