---
mode: 'agent'
tools: ['changes', 'codebase', 'editFiles', 'problems', 'search']
description: 'データ駆動テストを含む XUnit の単体テストに関するベストプラクティスを取得します'
---

# XUnit のベストプラクティス

XUnit を用いた効果的な単体テストの作成を支援します。標準的なテストとデータ駆動テストの両方を対象とします。

## プロジェクトセットアップ

- テストプロジェクトは `[ProjectName].Tests` の命名規則で分離
- Microsoft.NET.Test.Sdk、xunit、xunit.runner.visualstudio パッケージを参照
- テスト対象クラスに対応するテストクラスを作成（例: `Calculator` に対して `CalculatorTests`）
- .NET SDK のテストコマンドを使用（実行は `dotnet test`）

## テスト構造

- MSTest/NUnit と異なり、テストクラス属性は不要
- 単純なテストは `[Fact]` を使用
- Arrange-Act-Assert (AAA) パターンに従う
- テスト名は `MethodName_Scenario_ExpectedBehavior` のパターン
- セットアップはコンストラクター、後処理は `IDisposable.Dispose()`
- クラス内の共有コンテキストは `IClassFixture<T>` を使用
- 複数クラス間の共有コンテキストは `ICollectionFixture<T>` を使用

## 標準テスト

- テストは単一の振る舞いに集中
- 1つのテストメソッドで複数の振る舞いを検証しない
- 意図が伝わる明確なアサーションを使用
- 検証に必要な最小限のアサーションのみを含める
- テストは独立かつ順序に依存しない（どの順でも通る）
- テスト間の相互依存を避ける

## データ駆動テスト

- `[Theory]` と各種データ属性を組み合わせる
- `InlineData` でインラインのテストデータ
- `MemberData` でメソッド由来のテストデータ
- `ClassData` でクラス由来のテストデータ
- `DataAttribute` を実装してカスタムデータ属性を作成可能
- 意味のあるパラメータ名を使用

## アサーション

- 値の等価は `Assert.Equal`
- 参照の等価は `Assert.Same`
- 真偽の検証は `Assert.True`/`Assert.False`
- コレクションは `Assert.Contains`/`Assert.DoesNotContain`
- 正規表現は `Assert.Matches`/`Assert.DoesNotMatch`
- 例外検証は `Assert.Throws<T>` または `await Assert.ThrowsAsync<T>`
- 可読性向上のため Fluent Assertions 等のライブラリの使用を検討

## モックと分離

- XUnit と併用して Moq や NSubstitute を検討
- 依存をモックしてテスト対象を分離
- モック容易化のためインターフェースを活用
- 複雑なセットアップでは DI コンテナの利用を検討

## テストの整理

- 機能やコンポーネント単位でグルーピング
- 分類には `[Trait("Category", "CategoryName")]` を使用
- 共有依存にはコレクションフィクスチャを活用
- 診断には `ITestOutputHelper` を検討
- 条件付きスキップは `[Fact(Skip = "reason")]` / `[Theory(Skip = "reason")]` を使用
