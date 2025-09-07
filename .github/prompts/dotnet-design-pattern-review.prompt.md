---
mode: 'agent'
description: 'C#/.NET コードにおけるデザインパターン実装をレビューし、改善点を提案します。'
---
# .NET/C# デザインパターンレビュー

${selection} の C#/.NET コードについて、デザインパターンの実装をレビューし、本ソリューション/プロジェクトに沿った改善提案を行います。コードの変更は行わず、レビューのみ提供します。

## 必須デザインパターン

- Command パターン: ジェネリック基底クラス（`CommandHandler<TOptions>`）、`ICommandHandler<TOptions>` インターフェース、`CommandHandlerOptions` 継承、`static SetupCommand(IHost host)` メソッド
- Factory パターン: 複雑なオブジェクト生成とサービスプロバイダー統合
- 依存性注入: プライマリコンストラクター構文、`ArgumentNullException` による null チェック、インターフェース抽象、適切なライフタイム
- Repository パターン: 非同期データアクセス、接続提供の抽象化
- Provider パターン: 外部サービス（DB/AI等）の抽象化、明確な契約、設定の取り扱い
- Resource パターン: ローカライズメッセージの ResourceManager、.resx 分離（LogMessages, ErrorMessages）

## レビューチェックリスト

- デザインパターン: 利用パターンの特定。Command Handler/Factory/Provider/Repository は適切に実装されているか。導入すべきパターンはないか。
- アーキテクチャ: 名前空間規約（`{Core|Console|App|Service}.{Feature}`）に準拠しているか。Core/Console の分離は適切か。モジュール性と可読性はあるか。
- .NET ベストプラクティス: プライマリコンストラクタ、async/await と Task 戻り、ResourceManager、構造化ロギング、強く型付けされた構成は適用されているか。
- GoF パターン: Command/Factory/Template Method/Strategy は正しく実装されているか。
- SOLID 原則: SRP/OCP/LSP/ISP/DIP の違反はないか。
- パフォーマンス: 適切な async/await、リソース破棄、ConfigureAwait(false)、並列化の機会はあるか。
- 保守性: 関心の分離、例外処理の一貫性、設定の適切な利用はあるか。
- テスタビリティ: 依存の抽象化、モック容易性、非同期テスト、AAA パターン適合性はあるか。
- セキュリティ: 入力検証、資格情報の安全な取扱い、パラメータ化クエリ、安全な例外処理はあるか。
- ドキュメント: パブリック API の XML ドキュメント、引数/戻り値の説明、リソースファイルの整理はあるか。
- コードの明瞭さ: ドメインに沿った意味のある命名、パターンによる意図の明確化、自己説明的な構造か。
- クリーンコード: 一貫したスタイル、適切なメソッド/クラスサイズ、複雑性の最小化、重複排除ができているか。

## 改善に注力すべき領域

- Command ハンドラー: 基底クラスでの検証、一貫したエラーハンドリング、適切なリソース管理
- ファクトリー: 依存構成、サービスプロバイダー統合、破棄パターン
- プロバイダー: 接続管理、非同期パターン、例外処理とロギング
- 設定: データ注釈、検証属性、機密値の安全な取扱い
- AI/ML 連携: Semantic Kernel のパターン、構造化出力、モデル構成

プロジェクトのアーキテクチャと .NET のベストプラクティスに沿った、具体的で実行可能な改善提案を提示します。
