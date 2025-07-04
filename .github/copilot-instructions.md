# Copilot Instructions for Node.js & Express Project

## コーディング規約
- セミコロンは必ず付ける
- シングルクォート `'` を使用
- 2スペースインデント
- ES6 構文（`const`/`let`、アロー関数など）を優先
- 必要に応じて JSDoc コメントを記述

## Express 設計方針
- ルーティングは `src/routes/` に分離
- コントローラーは `src/controllers/` に分離
- ミドルウェアは `src/middlewares/` に分離
- モデルは `src/models/` に分離
- 設定値は `src/config/` にまとめる
- ユーティリティ関数は `src/utils/` にまとめる
- サービス層は `src/services/` にまとめる

## その他
- エラーハンドリングを徹底する
- 必要に応じて async/await を使う
- 依存パッケージは `package.json` に明記
- コード例は実行可能な形で記述

---

# English (for Copilot)
- Always use semicolons
- Use single quotes
- 2-space indentation
- Prefer ES6 syntax (const/let, arrow functions)
- Add JSDoc comments when appropriate
- Separate routes/controllers/middlewares/models/config/utils/services as per src/ structure
- Handle errors properly
- Use async/await as needed
- List dependencies in package.json
- Code samples should be runnable
