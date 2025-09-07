---
mode: 'agent'
description: 'ソリューションの新しい仕様ファイルを作成します。生成AIでの活用に最適化されています。'
tools: ['changes', 'codebase', 'editFiles', 'extensions', 'fetch', 'githubRepo', 'openSimpleBrowser', 'problems', 'runTasks', 'search', 'searchResults', 'terminalLastCommand', 'terminalSelection', 'testFailure', 'usages', 'vscodeAPI']
---
# 仕様の新規作成

`${input:SpecPurpose}` のための新しい仕様ファイルを作成してください。

仕様ファイルは、ソリューションコンポーネントの要件・制約・インターフェイスを、生成AIが効果的に扱えるよう明確かつ曖昧さのない構造で定義してください。既存の文書標準に従い、機械可読かつ自己完結であることを保証します。

## AI 対応仕様のベストプラクティス

- 正確・明示的・非曖昧な言語を使用
- 要件・制約・推奨の区別を明確化
- 見出し・リスト・表などの構造化フォーマットを使用
- 慣用句・比喩・文脈依存の表現は避ける
- すべての頭字語・専門用語を定義
- 必要に応じて例やエッジケースを含める
- 外部文脈に依存せず自己完結にする

仕様は [/spec/](/spec/) ディレクトリに保存し、命名規則は `spec-[a-z0-9-]+.md` を用います。名前は内容を表し、先頭は [schema, tool, data, infrastructure, process, architecture, design] のいずれかの高レベル目的で開始してください。

仕様ファイルは整形式の Markdown で記述します。

以下のテンプレートに従い、すべてのセクションを適切に記入してください。Markdown のフロントマターは次の例にならいます。

```md
---
title: [仕様の焦点を簡潔に示すタイトル]
version: [任意: 例 1.0, 日付]
date_created: [YYYY-MM-DD]
last_updated: [任意: YYYY-MM-DD]
owner: [任意: この仕様の責任チーム/担当者]
tags: [任意: 関連タグ（例: infrastructure, process, design, app など）]
---

# Introduction

[本仕様の概要と目的を簡潔に記述]

## 1. Purpose & Scope

[本仕様の目的と適用範囲、読者、前提などを明確に記述]

## 2. Definitions

[本仕様で使用する頭字語・略語・専門用語を一覧・定義]

## 3. Requirements, Constraints & Guidelines

[要件・制約・ルール・ガイドラインを明示的に列挙。箇条書きや表で明確に]

- **REQ-001**: Requirement 1
- **SEC-001**: Security Requirement 1
- **[3 LETTERS]-001**: Other Requirement 1
- **CON-001**: Constraint 1
- **GUD-001**: Guideline 1
- **PAT-001**: Pattern to follow 1

## 4. Interfaces & Data Contracts

[インターフェース、API、データコントラクト、連携ポイントの記述。スキーマや例は表/コードブロックで]

## 5. Acceptance Criteria

[各要件の明確で検証可能な受け入れ基準。必要に応じ Given-When-Then を使用]

- **AC-001**: Given [context], When [action], Then [expected outcome]
- **AC-002**: The system shall [specific behavior] when [condition]
- **AC-003**: [Additional acceptance criteria as needed]

## 6. Test Automation Strategy

[テストアプローチ、フレームワーク、自動化要件を定義]

- **Test Levels**: Unit, Integration, End-to-End
- **Frameworks**: MSTest, FluentAssertions, Moq (.NET アプリ向け)
- **Test Data Management**: [テストデータの作成/クリーンアップ方針]
- **CI/CD Integration**: [GitHub Actions での自動テスト]
- **Coverage Requirements**: [最小カバレッジ閾値]
- **Performance Testing**: [負荷・性能テストの方針]

## 7. Rationale & Context

[要件・制約・ガイドラインの背景と理由、設計判断のコンテキスト]

## 8. Dependencies & External Integrations

[本仕様に必要な外部システム・サービス・アーキテクチャ依存を定義。「どう実装するか」ではなく「何が必要か」に注力。バージョン固定はアーキ制約の場合のみ]

### External Systems
- **EXT-001**: [外部システム名] - [目的と連携種別]

### Third-Party Services
- **SVC-001**: [サービス名] - [必要な機能と SLA 要件]

### Infrastructure Dependencies
- **INF-001**: [インフラ要素] - [要件と制約]

### Data Dependencies
- **DAT-001**: [外部データソース] - [フォーマット、頻度、アクセス要件]

### Technology Platform Dependencies
- **PLT-001**: [プラットフォーム/ランタイム要件] - [バージョン制約と理由]

### Compliance Dependencies
- **COM-001**: [規制/コンプライアンス要件] - [実装への影響]

注: この節はアーキテクチャ/ビジネス依存に焦点を当て、特定パッケージ名の列挙は避けます（例: 「OAuth 2.0 認証ライブラリ」とし、特定のパッケージとバージョンは記載しない）。

## 9. Examples & Edge Cases

```code
// ガイドラインの正しい適用例（エッジケースを含む）
```

## 10. Validation Criteria

[本仕様への適合に必要な基準やテストを列挙]

## 11. Related Specifications / Further Reading

[関連仕様へのリンク]
[関連する外部ドキュメントへのリンク]

```
