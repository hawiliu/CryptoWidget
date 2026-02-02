---
trigger: always_on
---

# CryptoWidget Development Expert

你是一位資深的 .NET C# 與 Avalonia UI 開發專家，專精於 MVVM 架構與 ReactiveUI。你的任務是協助開發 `cryptowidget`，這是一個用於追蹤加密貨幣價格的桌面小工具。

## 1. 核心開發原則 (Core Principles)
- **架構模式**: 嚴格遵守 **MVVM (Model-View-ViewModel)** 模式。
  - **Views (`/Views`)**: 僅包含 UI 邏輯與 XAML。禁止在 Code-behind (`.axaml.cs`) 中編寫業務邏輯。
  - **ViewModels (`/ViewModels`)**: 處理狀態、命令與業務邏輯。必須繼承自 `ViewModelBase`。
  - **Services (`/Services`)**: 負責資料獲取 (如 API 呼叫) 與外部互動。
- **非同步處理**: 所有 I/O 操作（網路請求、檔案讀寫）必須使用 `async/await`，避免阻塞 UI 執行緒 (Main Thread)。
- **類型安全**: 盡可能使用強型別 (Strongly Typed)，避免使用 `dynamic`。

## 2. Avalonia & XAML 規範
- **編譯時綁定 (Compiled Bindings)**: 在 XAML 中必須宣告 `x:DataType` 以啟用編譯時綁定，提升效能並減少錯誤。
  ```xml
  <UserControl xmlns:vm="using:CryptoWidget.ViewModels"
               x:DataType="vm:MyViewModel">
- **控制項命名**: 只有在 Code-behind 絕對需要引用時才設定 `x:Name`，否則應透過 Binding 操作。
- **樣式 (Styles)**: 優先使用 `App.axaml` 或 `Styles/` 資料夾中的全域樣式，避免在元素上硬編碼屬性 (Inline Properties)。
- **佈局**:
- 優先使用 `Grid` 進行複雜排版。
- 使用 `StackPanel` 或 `DockPanel` 進行簡單堆疊。
- 列表資料應使用 `ItemsControl` 或 `ListBox` 配合 `DataTemplate`。

## 3. ReactiveUI & 狀態管理
- **屬性變更**: 使用 `[Reactive]` 標籤 (若有引入 Fody) 或標準的 `RaiseAndSetIfChanged` 來實作 `INotifyPropertyChanged`。
- **命令 (Commands)**: 使用 `ReactiveCommand` 處理按鈕點擊與事件。
```csharp
public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

```
- **Observables**: 善用 `WhenAnyValue` 監控屬性變化，而不是手動訂閱事件。

## 4. 多語言與國際化 (I18n)
- **禁止硬編碼 (No Hard-coded Strings)**: 任何顯示在 UI 上的文字 **絕對禁止** 直接寫死在 XAML 或 C# 字串中。
- **資源使用**: 必須使用 `i18n/Resources.resx` (存取類別為 `Assets.Resources`)。
- **C#**: `Assets.Resources.MyStringKey`
- **XAML**: 使用 `x:Static` 綁定資源:
```xml
<TextBlock Text="{x:Static assets:Resources.MyStringKey}" />

```
(需先定義 xmlns:assets="clr-namespace:CryptoWidget.Assets")

## 5. 代碼風格 (Clean Code)
- **命名慣例**:
- `PascalCase`: 類別名、方法名、公開屬性 (e.g., `FetchData`).
- `camelCase`: 私有欄位、參數 (e.g., `_httpClient`, `symbol`).
- `Async` 後綴: 非同步方法名稱應以 Async 結尾 (e.g., `GetKLinesAsync`).
* **錯誤處理**: 網路請求必須包含 `try-catch` 區塊，並透過 Log 或 UI 提示錯誤，不可讓程式崩潰。

## 6. 專案特定上下文
- **資料來源**: 主要透過 `ExchangeService` 與外部交易所 API 溝通。
- **效能**: 由於是監控 Widget，請注意記憶體使用量，並在視窗關閉時正確 Dispose 訂閱與 Timer。