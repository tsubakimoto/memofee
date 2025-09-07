---
description: 'Blazor component and application patterns'
applyTo: '**/*.razor, **/*.razor.cs, **/*.razor.css'
---

## Blazor コードスタイルと構造

- Blazor および C# コードは慣習に則り効率的に記述する。
- .NET および Blazor の規約に従う。
- コンポーネントベースの UI 開発には Razor コンポーネントを適切に使用する。
- 小規模なコンポーネントにはインライン関数を優先するが、複雑なロジックはコードビハインドまたはサービスクラスに分離する。
- 非同期処理が必要な箇所では Async/await を使用し、UI 操作がブロックされないようにする。

## 命名規則

- コンポーネント名、メソッド名、パブリックメンバーには PascalCase を採用する。
- プライベートフィールドとローカル変数には camelCase を使用する。
- インターフェース名には 「I」 を接頭辞として付ける（例: IUserService）。

## Blazorおよび.NET固有のガイドライン

- コンポーネントライフサイクルにはBlazorの組み込み機能（例：OnInitializedAsync、OnParametersSetAsync）を活用する。
- @bindによるデータバインディングを効果的に使用する。
- Blazorのサービスには依存性注入を活用する。
- Blazorコンポーネントとサービスは関心事の分離（Separation of Concerns）に従って構成する。
- 常に最新版のC#を使用すること（現在はC# 13の機能：レコード型、パターンマッチング、グローバルusingなど）。

## エラー処理とバリデーション

- BlazorページおよびAPI呼び出しに対して適切なエラー処理を実装すること。
- バックエンドでのエラー追跡にはロギングを使用し、BlazorではErrorBoundaryなどのツールでUIレベルのエラーを捕捉することを検討すること。
- フォームではFluentValidationまたはDataAnnotationsを使用したバリデーションを実装すること。

## Blazor APIとパフォーマンス最適化

- プロジェクト要件に基づき、BlazorサーバーサイドまたはWebAssemblyを最適に活用する。
- API呼び出しやメインスレッドをブロックする可能性のあるUI操作には非同期メソッド（async/await）を使用する。
- 不要なレンダリングを削減しStateHasChanged()を効率的に使用してRazorコンポーネントを最適化する。
- 必要ない限り再レンダリングを避け、適切な箇所でShouldRender()を使用しコンポーネントのレンダリングツリーを最小化する。
- ユーザー操作の効率的な処理にはEventCallbacksを使用し、イベント発生時には最小限のデータのみを送信する。

## キャッシュ戦略

- 頻繁に使用されるデータ（特にBlazor Serverアプリ）にはインメモリキャッシュを実装する。軽量なキャッシュソリューションにはIMemoryCacheを使用する。
- Blazor WebAssemblyでは、ユーザーセッション間でアプリケーション状態をキャッシュするためにlocalStorageまたはsessionStorageを利用する。
- 複数のユーザーやクライアント間で状態を共有する必要がある大規模アプリケーションでは、分散キャッシュ戦略（Redis や SQL Server Cache など）を検討する。
- データが変更される可能性が低い場合、API 呼び出しのレスポンスを保存してキャッシュし、冗長な呼び出しを回避することでユーザーエクスペリエンスを向上させる。

## 状態管理ライブラリ

- コンポーネント間で基本的な状態を共有するには、Blazor の組み込み機能であるカスケードパラメータと EventCallbacks を使用する。
- アプリケーションの複雑化に伴い、FluxorやBlazorStateなどのライブラリを用いた高度な状態管理ソリューションを実装する。
- Blazor WebAssemblyにおけるクライアントサイドの状態永続化には、ページ再読み込み間での状態維持にBlazored.LocalStorageまたはBlazored.SessionStorageの利用を検討する。
- サーバーサイドBlazorでは、ユーザーセッション内の状態管理と再レンダリングの最小化にスコープ付きサービスとStateContainerパターンを活用する。

## API設計と統合

- 外部APIや自社バックエンドとの通信にはHttpClientまたは適切なサービスを使用する。
- API呼び出しのエラー処理をtry-catchで実装し、UI上で適切なユーザーフィードバックを提供する。

## Visual Studioでのテストとデバッグ

- すべてのユニットテストおよび統合テストはVisual Studio Enterpriseで実施する。
- BlazorコンポーネントとサービスのテストにはxUnit、NUnit、またはMSTestを使用する。
- テスト中の依存関係モックにはMoqまたはNSubstituteを使用する。
- Blazor UIの問題はブラウザ開発者ツールで、バックエンド/サーバーサイドの問題はVisual Studioのデバッグツールで調査する。
- パフォーマンスプロファイリングと最適化にはVisual Studioの診断ツールを活用する。

## セキュリティと認証

- Blazorアプリに必要な認証/認可を実装する（API認証にはASP.NET IdentityまたはJWTトークンを使用）。
- すべてのWeb通信にHTTPSを使用し、適切なCORSポリシーが実装されていることを確認する。

## APIドキュメントとSwagger

- バックエンドAPIサービスのドキュメントにはSwagger/OpenAPIを使用する。
- Swaggerドキュメントを充実させるため、モデルとAPIメソッドのXMLドキュメントを必ず用意する。
