---
mode: 'agent'
description: '.NET/C# コードがこのソリューション/プロジェクトのベストプラクティスに適合しているかを確認します。'
---
# .NET/C# ベストプラクティス

${selection} の .NET/C# コードが、このソリューション/プロジェクト特有のベストプラクティスに従っているかを確認し、改善します。対象は以下を含みます。

## ドキュメントと構造

- すべての public クラス/インターフェース/メソッド/プロパティに XML ドキュメントコメントを付与
- パラメータと戻り値の説明を XML コメントに含める
- 既定の名前空間規約 `{Core|Console|App|Service}.{Feature}` に従う

## デザインパターンとアーキテクチャ

- 依存性注入にはプライマリコンストラクター構文（例: `public class MyClass(IDependency dependency)`）を使用
- ジェネリック基底クラスによる Command Handler パターン（例: `CommandHandler<TOptions>`）を実装
- インターフェース分離と明確な命名規則（I 接頭辞）
- 複雑なオブジェクト生成には Factory パターンを適用

## 依存性注入とサービス

- コンストラクターDIと `ArgumentNullException` による null チェック
- ライフタイムに応じてサービス登録（Singleton/Scoped/Transient）
- Microsoft.Extensions.DependencyInjection のパターンを使用
- テスタビリティのためサービスはインターフェースで抽象化

## リソース管理とローカライズ

- メッセージ/エラー文言は ResourceManager を使用
- LogMessages と ErrorMessages の .resx を分離
- リソースアクセスは `_resourceManager.GetString("MessageKey")`

## Async/Await のパターン

- すべての I/O と長時間処理は async/await を使用
- 非同期メソッドは Task / Task<T> を返却
- 適切な箇所では ConfigureAwait(false) を使用
- 非同期例外を適切に処理

## テスト基準

- アサーションに FluentAssertions を用いた MSTest などのフレームワークを使用
- AAA パターン（Arrange, Act, Assert）を遵守
- 依存のモックに Moq を使用
- 成功/失敗の両シナリオをテスト
- null パラメータ検証のテストを含める

## 構成と設定

- 強く型付けされた設定クラスとデータ注釈を使用
- 検証属性（Required, NotEmptyOrWhitespace）を実装
- 設定は IConfiguration バインドで取り込む
- appsettings.json に対応

## Semantic Kernel と AI 連携

- AI 処理に Microsoft.SemanticKernel を使用
- 適切なカーネル構成とサービス登録を実装
- モデル設定（ChatCompletion, Embedding など）を取り扱う
- 構造化出力パターンで安定した AI 応答を得る

## 例外処理とロギング

- Microsoft.Extensions.Logging による構造化ロギング
- 意味のあるコンテキスト付きスコープドロギング
- 説明的メッセージを持つ特定例外を送出
- 予期される失敗シナリオには try-catch を使用

## パフォーマンスとセキュリティ

- 適用可能な箇所で C# 12+ と .NET 8 の最適化を活用
- 入力の適切な検証とサニタイズ
- データベース操作はパラメータ化クエリを使用
- AI/ML におけるセキュアコーディングを遵守

## コード品質

- SOLID 原則の順守
- 基底クラス・ユーティリティで重複を排除
- ドメインを反映した意味のある命名
- メソッドは凝集度を保ち単一責務に
- リソースに対して適切な破棄パターンを実装
