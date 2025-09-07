---
mode: 'agent'
description: '仕様ファイルに基づく機能要望の GitHub Pull Request を、pull_request_template.md を用いて作成します。'
tools: ['codebase', 'search', 'github', 'create_pull_request', 'update_pull_request', 'get_pull_request_diff']
---
# 仕様から GitHub Pull Request を作成

`${workspaceFolder}/.github/pull_request_template.md` に基づき、仕様のための Pull Request を作成します。

## 手順

1. 'search' ツールで `${workspaceFolder}/.github/pull_request_template.md` のテンプレートを解析し、必要情報を抽出。
2. 'create_pull_request' ツールで `${input:targetBranch}` にドラフト PR を作成。既に同ブランチの PR があるか `get_pull_request` で確認し、存在する場合は手順4へ（手順3はスキップ）。
3. 'get_pull_request_diff' ツールで PR の変更点を取得・分析。
4. 'update_pull_request' ツールで本文とタイトルを更新。手順1で取得したテンプレート情報を反映。
5. 'update_pull_request' ツールでドラフトからレビュー可状態へ切り替え。
6. 'get_me' で PR 作成者のユーザー名を取得し、`update_issue` ツールでアサイン。
7. 作成された PR の URL をユーザーに返す。

## 要件
- 仕様全体につき PR は1件
- 仕様を特定できる明確なタイトル/pull_request_template.md
- pull_request_template.md に必要情報を十分に記載
- 作成前に既存 PR と重複しないことを確認
