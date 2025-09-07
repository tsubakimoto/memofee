---
description: 'Blazor コンポーネントとアプリケーションのパターン'
applyTo: '**/*.razor, **/*.razor.cs, **/*.razor.css'
---

## Blazor のコードスタイルと構成

- 典型的で効率的な Blazor および C# のコードを書きます。
- .NET と Blazor の規約に従います。
- コンポーネントベースの UI 開発には Razor コンポーネントを適切に使用します。
- 小さなコンポーネントではインライン関数を優先し、複雑なロジックはコードビハインドやサービスクラスへ分離します。
- UI をブロックしないため、適用できる箇所では async/await を用います。

## 命名規則

- コンポーネント名、メソッド名、パブリックメンバーには PascalCase を使用します。
- プライベートフィールドやローカル変数には camelCase を使用します。
- インターフェイス名には接頭辞「I」を付けます（例: IUserService）。

## Blazor と .NET 固有のガイドライン

- コンポーネントのライフサイクルに Blazor の組み込み機能を活用します（例: OnInitializedAsync, OnParametersSetAsync）。
- @bind によるデータバインディングを効果的に使います。
- Blazor のサービスには依存性注入（DI）を活用します。
- 関心の分離（Separation of Concerns）に従ってコンポーネントとサービスを構成します。
- 常に最新の C# を使用します。現在は C# 13 のレコード型、パターンマッチング、グローバル using などの機能を想定します。

## エラーハンドリングと検証

- Blazor ページや API 呼び出しに適切なエラーハンドリングを実装します。
- バックエンドのエラー追跡にはログを使用し、Blazor では ErrorBoundary などを用いて UI レベルのエラー捕捉も検討します。
- フォームでは FluentValidation または DataAnnotations による検証を実装します。

## Blazor の API とパフォーマンス最適化

- 要件に応じて Blazor Server と WebAssembly を最適に使い分けます。
- メインスレッドをブロックし得る API 呼び出しや UI 操作には非同期メソッド（async/await）を使用します。
- 不要な再描画を減らし、StateHasChanged() を効率的に使って Razor コンポーネントを最適化します。
- 必要な場合のみ再描画するよう ShouldRender() を適切に用い、レンダーツリーを最小化します。
- ユーザー操作の処理には EventCallback を使用し、イベント発火時には最小限のデータのみを渡します。

## キャッシング戦略

- 頻繁に使用するデータにはインメモリキャッシュを実装します（特に Blazor Server アプリ）。軽量なキャッシュには IMemoryCache を使用します。
- Blazor WebAssembly では、ユーザーセッション間でアプリケーション状態を保持するために localStorage や sessionStorage を活用します。
- 複数ユーザーやクライアント間で共有状態が必要な大規模アプリでは、Redis や SQL Server Cache などの分散キャッシュを検討します。
- 変更頻度が低いデータの API 応答をキャッシュして重複呼び出しを避け、ユーザー体験を向上させます。

## 状態管理ライブラリ

- 基本的なコンポーネント間の状態共有には、Blazor の Cascading Parameter と EventCallback を使用します。
- アプリが複雑化してきたら、Fluxor や BlazorState といったライブラリで高度な状態管理を実装します。
- Blazor WebAssembly のクライアント側で状態の永続化が必要な場合は、Blazored.LocalStorage や Blazored.SessionStorage の利用を検討します。
- Blazor Server では、スコープドサービスや StateContainer パターンでユーザーセッション内の状態を管理しつつ、再描画を最小限に抑えます。

## API 設計と統合

- 外部 API や自前のバックエンドとの通信には HttpClient など適切なサービスを使用します。
- API 呼び出しには try-catch によるエラーハンドリングを実装し、UI で適切なユーザーフィードバックを提供します。

## Visual Studio でのテストとデバッグ

- 単体テストと結合テストは Visual Studio Enterprise で実施します。
- Blazor のコンポーネントやサービスは xUnit、NUnit、または MSTest でテストします。
- テスト時の依存関係のモックには Moq か NSubstitute を使用します。
- ブラウザーの開発者ツールや Visual Studio のデバッガーで UI の問題を調査し、バックエンドやサーバーサイドの問題も同様にデバッグします。
- パフォーマンスのプロファイリングと最適化には Visual Studio の診断ツールを活用します。

## セキュリティと認証

- 必要に応じて Blazor アプリに認証・認可を実装します。API 認証には ASP.NET Identity または JWT トークンを使用します。
- すべての通信は HTTPS を使用し、適切な CORS ポリシーを構成します。

## API ドキュメントと Swagger

- バックエンドの API サービスには Swagger/OpenAPI を用いてドキュメントを提供します。
- モデルや API メソッドには XML ドキュメントコメントを付与し、Swagger のドキュメント性を高めます。
