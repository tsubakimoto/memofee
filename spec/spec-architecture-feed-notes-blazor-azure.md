---
title: 個人用フィード記事へのノート付与 Web アプリ（.NET 10 / Blazor / Azure App Service Easy Auth）
version: 1.0
date_created: 2025-09-07
last_updated: 2025-09-07
owner: repo: tsubakimoto/memofee
tags: [architecture, design, app, blazor, azure, security]
---

# Introduction

本仕様は、外部のフィード（RSS/Atom）記事に個人ノートを付与・検索できる単一ユーザー向け Web アプリを、.NET 10 と Blazor Web App で実装し、Azure Web App（App Service）にデプロイするための要件・制約・インターフェイスを定義する。認証は Azure App Service Authentication（Easy Auth）を用い、特定の 1 ユーザーのみが利用できる。

## 1. Purpose & Scope

- 目的: 単一ユーザー（オーナー）によるフィード記事の収集・閲覧・ノート付与・検索を可能にする最小構成の Web アプリを安全かつ運用容易に提供する。
- 適用範囲: アプリの機能要件、非機能要件、セキュリティ、データコントラクト、運用（CI/CD・監視）に関するアーキテクチャ仕様。
- 読者: アプリ開発者、レビュアー、運用担当。
- 前提: 
  - デプロイ先は Azure Web App（Linux/Windows いずれか、特記ない限り Linux を推奨）。
  - 認証は Easy Auth（Microsoft アカウント/Azure AD）で実施。アプリ側でユーザー制限（単一ユーザー）を強制。
  - 外部フィードへの HTTP アクセスが許可されている。

## 2. Definitions

- RSS/Atom: Web フィードの標準形式。記事のタイトル、URL、公開日時、概要等を配信。
- Easy Auth: Azure App Service Authentication。リバースプロキシ型で認証を終えたクレーム情報をアプリにヘッダー経由で伝搬。
- UPN: User Principal Name。ユーザー一意識別子として使用（例: user@example.com）。
- Article: フィードから取得した記事エンティティ。
- Note: 単一ユーザーが記事に付与するテキストメモ（任意のタグ/ブックマーク状態を含む）。
- Durable storage: アプリ再起動やスケール操作でも永続されるストレージ（App Service の一時ローカルディスクは不可）。
- PII: 個人を特定可能な情報。本アプリでは最小限（UPN/oid の比較・記録最小化）。

## 3. Requirements, Constraints & Guidelines

機能要件
- REQ-001: ユーザーはフィード URL を登録できる（複数可）。
- REQ-002: アプリは登録フィードから記事を取得・更新（手動更新＋任意でバックグラウンド定期更新）。
- REQ-003: ユーザーは任意の記事にノート（本文、任意タグ、ピン/スター等のフラグ）を付与/編集/削除できる。
- REQ-004: 記事とノートはタイトル/本文/タグ/URLで検索・フィルターできる。
- REQ-005: UI は Blazor Web App（.NET 10）で提供する。
- REQ-006: ノートは記事 URL または安定 ID に紐づく。記事の重複取得時も同一記事として扱えるキーを用意する。

セキュリティ要件
- SEC-001: 認証は Azure App Service Easy Auth を用いる。
- SEC-002: アプリは Easy Auth のクレームから認証済みユーザーの UPN（または oid）を取得し、許可ユーザー（構成値）と厳密一致する場合のみアクセス許可する。
- SEC-003: 許可ユーザー以外のアクセスは 403 Forbidden とする。
- SEC-004: すべての機密設定は環境変数/アプリ設定で注入し、ソース管理しない。
- SEC-005: すべての通信は HTTPS を強制。HTTP からのアクセスは 301/308 リダイレクト。
- SEC-006: 書き込み API は CSRF/偽装対策を実施（Blazor サーバー側の認証/認可と antiforgery 相当の保護）。

非機能要件
- NFR-001: 可用性: 単一インスタンス想定。ただしスケールアウト時にも整合性を保つ設計とする。
- NFR-002: 性能: 1000 記事程度の一覧/検索で 1 秒以内の応答（P95）を目標。
- NFR-003: 監視: 重要操作のログと基本メトリクス（リクエスト数/エラー率/レイテンシ）収集。
- NFR-004: 運用: IaC/手動いずれでもよいが、GitHub Actions による CI/CD を用意。

データ/ストレージ要件
- DAT-001: ノート・フィード登録は耐久ストレージに保存する（App Service の一時ディスク不可）。
<!-- - DAT-002: Azure Storage（Tables/Blob）またはマネージド NoSQL を使用。RDB 必須要件はない。 -->
- DAT-002: Azure SQL Database を使用。
- DAT-003: データ保持ポリシー: 明示削除以外は保持。エクスポート（JSON）を提供（任意）。

制約
- CON-001: ランタイムは .NET 10。フレームワークは Blazor Web App を使用する。
- CON-002: デプロイ先は Azure Web App（App Service）。
- CON-003: 外部依存は最小限とし、言語/フレームワーク標準と Azure マネージド機能を優先。

ガイドライン
- GUD-001: API は小さくシンプルに（CRUD + 検索）。
- GUD-002: 外部フィードは失敗に強い実装（タイムアウト/リトライ/サーキットブレーカ）。
- GUD-003: ログには個人情報・トークン・クッキーを出力しない。

パターン
- PAT-001: 認証はリバースプロキシ（Easy Auth）後段のクレーム検証で単一ユーザー許可。
- PAT-002: 記事キーは（feed_url, normalized_entry_id|link_url のハッシュ）で安定化。

## 4. Interfaces & Data Contracts

<!-- エンドポイント（例; 実装は Minimal APIs でも Controller でも可） -->
エンドポイント（例; 実装は Minimal APIs を使用）
- GET /api/feeds: 登録済みフィード一覧を返す。
- POST /api/feeds: フィードを追加する。Body: { url }
- DELETE /api/feeds/{id}: フィードを削除する。
- POST /api/feeds/{id}/refresh: 当該フィードの再取得をトリガー。
- GET /api/articles: クエリで記事を検索/一覧（q, tag, starred, feedId, page, pageSize）。
- GET /api/articles/{id}: 記事詳細を返す（記事本文は外部リンク参照を基本）。
- GET /api/articles/{id}/notes: 記事ノート一覧（単一ユーザー前提だが API として配列）。
- PUT /api/articles/{id}/notes: ノートを作成/上書き。
- DELETE /api/articles/{id}/notes/{noteId}: ノートを削除。

要求ヘッダー（Easy Auth）
- X-MS-CLIENT-PRINCIPAL（Base64 JSON）または X-MS-CLIENT-PRINCIPAL-NAME（UPN）: 認証済ユーザー情報。
- アプリは構成値 ALLOWED_UPN（または ALLOWED_OID）と一致を確認。

データコントラクト（JSON）
- Feed
  {
    "id": "string",          // 内部ID
    "url": "https://...",    // フィードURL
    "title": "string?",      // 取得時に判明すれば保存
    "createdAt": "ISO-8601"
  }
- Article
  {
    "id": "string",              // 安定キー: hash(feed_url, entry_id|link)
    "feedId": "string",
    "title": "string",
    "linkUrl": "https://...",
    "publishedAt": "ISO-8601?",
    "summary": "string?",
    "author": "string?",
    "tags": ["string"],
    "fetchedAt": "ISO-8601"
  }
- Note
  {
    "id": "string",              // noteId（記事+ユーザーで一意、単一ユーザーのため articleId と同値でも可）
    "articleId": "string",
    "body": "string",
    "tags": ["string"],
    "starred": true,
    "createdAt": "ISO-8601",
    "updatedAt": "ISO-8601"
  }

エラー
- 401: 未認証（Easy Auth が外れている/無効）
- 403: 許可ユーザー不一致
- 400: 不正入力
- 429/503: 外部フィードのスロットリング/一時障害

## 5. Acceptance Criteria

- AC-001 (REQ-001): Given 認証済かつ許可ユーザー, When フィードURLを登録, Then /api/feeds が新規フィードを返し、重複URLは 409 を返す。
- AC-002 (REQ-002): Given 登録済フィード, When refresh を呼出, Then 記事が最新化され重複記事は再生成されない。
- AC-003 (REQ-003): Given 記事, When ノートを PUT, Then Note が保存され GET で取得できる。本文・タグ・スターが反映される。
- AC-004 (REQ-004): Given ノート付き記事が複数, When q=キーワード で検索, Then タイトル/本文/タグに一致した記事が返る。
- AC-005 (SEC-002): Given UPN が ALLOWED_UPN と不一致, When いずれの API を呼出, Then 403 を返し本文は詳細を含まない。
- AC-006 (DAT-001): Given アプリの再起動, When 直前に作成したノートを再取得, Then データは失われない。
- AC-007 (NFR-002): Given 1000 記事, When クエリなし一覧, Then P95 応答 < 1s（計測方法は 10 回の平均、外形監視で確認）。

## 6. Test Automation Strategy

- Test Levels: Unit（ドメイン/ユーティリティ）, Integration（API, ストレージ, Easy Auth ヘッダー）, E2E（最小）
<!-- - Frameworks: MSTest + FluentAssertions + Moq（.NET） -->
- Frameworks: xUnit + FluentAssertions + Moq（.NET）
- Test Data: In-memory ストレージ実装またはローカルエミュレータ（可能なら）。テストごとに分離しクリーンアップ。
- CI/CD: GitHub Actions でビルド・テスト・静的解析を実行。main へは PR 必須（任意）。
- Coverage: ステートメント 70% 以上（ドメイン・サービス層 80% 以上を目標）。
- Performance: 簡易負荷（k6 など）で記事 1000 件・検索 10 パターンを 1 分間測定。

## 7. Rationale & Context

- 単一ユーザー用途のため、認証は Easy Auth + 許可 UPN 比較で十分。アプリ内のユーザー管理は不要。
- フィードデータは読み取り中心で RDB が必須ではない。コストと運用の軽さから Azure Storage/NoSQL を採用。
- Blazor Web App により UI/サーバを .NET で統一し、実装・保守の一貫性を確保。

## 8. Dependencies & External Integrations

### External Systems
- EXT-001: 外部フィード（RSS/Atom） - 記事取得（HTTP GET）。

### Third-Party Services
- SVC-001: Azure App Service Authentication（Easy Auth） - 認証/クレーム提供。

### Infrastructure Dependencies
- INF-001: Azure Web App（App Service, Linux 推奨） - ホスティングと TLS 終端。
<!-- - INF-002: 永続ストレージ（Azure Storage Tables/Blob またはマネージド NoSQL） - ノート/フィード永続化。 -->
- INF-002: 永続ストレージ（Azure SQL Database） - ノート/フィード永続化。
- INF-003: アプリ監視（Application Insights 相当） - ログ/メトリクス収集。

### Data Dependencies
- DAT-001: 外部フィードデータ - RSS/Atom 形式（XML）。

### Technology Platform Dependencies
- PLT-001: .NET 10 / ASP.NET Core / Blazor Web App。

### Compliance Dependencies
- COM-001: 個人情報最小化 - 許可 UPN/oid の比較以外の PII を保存しない。

## 9. Examples & Edge Cases

```code
// 正常系
- 同一フィード内で同一リンクURLの記事は 1 件として扱う。
- 別フィードに同一記事があってもキーは feed_url を含むため別記事として扱う。

// エッジケース
- フィード取得失敗（404/5xx/タイムアウト）: リトライし、一定回数超で失敗記録のみ。
- フィードの重複 URL 登録: 409。
- 記事の公開日時欠落: publishedAt を null とし並び替えは fetchedAt を代用。
- Easy Auth 無効化/ヘッダー欠落: 401（または 403）でブロック。
- スケールアウト時: ストレージは外部のため整合性維持。ローカルキャッシュ前提にしない。
```

## 10. Validation Criteria

- 仕様準拠チェックリスト（主要要件）
  - REQ-001〜006: 実装・ユニット/統合テストの存在
  - SEC-001〜006: 未認証/不許可テスト、HTTPS 強制確認
  - DAT-001〜003: 外部ストレージ利用確認、再起動耐性テスト
  - NFR-001〜004: パフォーマンス測定、ログ/メトリクス送出確認、CI/CD 実行
<!-- - ドキュメント: README に構成/設定項目を明記（ALLOWED_UPN/ALLOWED_OID, STORAGE 接続, APPINSIGHTS 他）。 -->
- ドキュメント: README に構成/設定項目を明記（ALLOWED_UPN/ALLOWED_OID, DATABASE 接続, APPINSIGHTS 他）。

## 11. Related Specifications / Further Reading

- Azure App Service Authentication（Easy Auth） docs
- ASP.NET Core Minimal APIs / Blazor Web App docs
- RSS/Atom 仕様（RFC4287 等）
<!-- - Azure Storage（Tables/Blob）サービス ドキュメント -->
- Azure SQL Database ドキュメント
