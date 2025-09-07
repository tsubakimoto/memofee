---
mode: 'agent'
description: '仕様ファイルに基づく機能要望の GitHub Issue を、feature_request.yml テンプレートで作成します。'
tools: ['codebase', 'search', 'github', 'create_issue', 'search_issues', 'update_issue']
---
# 仕様から GitHub Issue を作成

`${file}` にある仕様に対して GitHub Issue を作成してください。

## 手順

1. 仕様ファイルを解析して要件を抽出
2. `search_issues` で既存 Issue を確認
3. `create_issue` で新規作成、または `update_issue` で既存を更新
4. `feature_request.yml` テンプレートを使用（なければデフォルトにフォールバック）

## 要件

- 仕様全体につき Issue は1件
- 仕様を特定できる明確なタイトル
- 仕様が要求する変更のみを含める
- 作成前に既存 Issue と重複しないことを確認

## Issue 内容

- Title: 仕様内の機能名
- Description: 課題の説明、提案解決策、背景コンテキスト
- Labels: feature, enhancement（適宜）
