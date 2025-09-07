---
mode: 'agent'
description: '仕様ファイル内で未実装の要件に対する GitHub Issue を、feature_request.yml テンプレートで作成します。'
tools: ['codebase', 'search', 'github', 'create_issue', 'search_issues', 'update_issue']
---
# 未充足の仕様要件に対する GitHub Issue の作成

`${file}` の仕様に含まれる未実装の要件ごとに、GitHub Issue を作成してください。

## 手順

1. 仕様ファイルを解析して全要件を抽出
2. 各要件の実装状況をコードベースで確認
3. `search_issues` で既存 Issue を検索し重複を回避
4. 未実装の要件ごとに `create_issue` で新規作成
5. `feature_request.yml` テンプレートを使用（なければデフォルトにフォールバック）

## 要件

- 仕様における未実装要件ごとに1件の Issue
- 要件 ID と説明の明確な対応付け
- 実装ガイダンスと受け入れ基準を含める
- 作成前に既存 Issue との重複を確認

## Issue 内容

- Title: 要件 ID と簡潔な要約
- Description: 詳細な要件、想定実装方法、背景コンテキスト
- Labels: feature, enhancement（適宜）

## 実装確認

- コードベースで関連コードパターンを検索
- `/spec/` ディレクトリの関連仕様を確認
- 要件が部分的に実装済みでないか検証
