---
description: Generate a commit message based on staged files and project rules
---

# Commit 訊息規範

使用繁體中文產生 commit message，概述功能，不需要檔名路徑，不需要檔案連結。
不要使用 markdown 格式輸出 commit message。
** 使用複製框讓我可以直接複製 **

## 格式

```
type: subject

description (選填)
```

## Type 類別說明

| Type | 說明 |
|------|------|
| `feat` | 新增/修改功能 (feature) |
| `fix` | 修補 bug (bug fix) |
| `docs` | 文件 (documentation) |
| `style` | 格式 (不影響程式碼運行的變動：空白、格式化、缺少分號等) |
| `refactor` | 重構 (既不是新增功能，也不是修補 bug 的程式碼變動) |
| `perf` | 改善效能 (A code change that improves performance) |
| `test` | 增加測試 (when adding missing tests) |
| `chore` | 建構程序或輔助工具的變動 (maintain) |
| `revert` | 撤銷回覆先前的 commit，例如：`revert: type(scope): subject (回覆版本：xxxx)` |

## 範例

單行格式：
```
feat: 新增虛擬搖桿拖曳觸發機制
```

多行格式（含 description）：
```
feat: 新增武器進化系統
- 實作 SWORD_EVOLUTION_TREE 資料結構
- 根據擊殺數自動升級武器
- 武器視覺效果隨等級變化
```

---

# 工作流程步驟

// turbo-all

1. 檢查目前已暫存的變更：
   > [!IMPORTANT]
   > **嚴禁**自動執行 `git add` 或 `git commit`。
   > 只針對使用者**手動**暫存 (Staged) 的檔案產生訊息。
   > 若無暫存檔案，回報錯誤請使用者先手動執行 `git add`。

   ```bash
   git diff --cached --stat
   ```

2. 查看實際程式碼變更：
   ```bash
   git diff --cached
   ```
   - 若輸出過大，可使用 `view_file` 查看關鍵檔案。

3. 根據變更內容與上述規範產生 commit message：
   - 確保訊息符合 `type: subject` 格式
   - 使用繁體中文
   - 概述功能性變更
   - 除非必要，不在 subject 中包含檔案路徑
   - 若有破壞性變更或需詳細說明，加入 description body

4. 將產生的 commit message 以純文字格式呈現給使用者審核。