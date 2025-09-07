---
mode: 'agent'
description: '実装計画の各フェーズから GitHub Issue を作成します。feature_request.yml または chore_request.yml テンプレートを使用します。'
tools: ['codebase', 'search', 'github', 'create_issue', 'search_issues', 'update_issue']
---
# 実装計画から GitHub Issue を作成

`${file}` にある実装計画に基づき、GitHub Issue を作成してください。

## 手順

1. 計画ファイルを解析してフェーズを特定
2. `search_issues` で既存 Issue を確認
3. 各フェーズにつき `create_issue` で新規作成、または `update_issue` で既存を更新
4. `feature_request.yml` または `chore_request.yml` を使用（なければデフォルトにフォールバック）

## 要件

- フェーズごとに1件の Issue
- 明確で構造化されたタイトルと説明
- 計画が要求する変更のみを含める
- 作成前に既存 Issue と重複しないことを確認

## Issue 内容

- Title: 実装計画のフェーズ名
- Description: フェーズの詳細、要件、背景コンテキスト
- Labels: 課題種別に応じたラベル（feature/chore）
