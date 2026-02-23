# FormulaCalculator

他の言語版:  
[中文版本](./README_CN.md) ｜ [English](./README.md)

## 概要

C# および Unity 向けの、高速・ゼロアロケーション・シングルパス式評価ライブラリです。

- **速度重視設計**: デフォルトでは式の検証を行いません。
- ホットパスでの **ヒープアロケーションなし**（Spanベース）。
- **.NET（NuGet）** および **Unity（UPM）** に対応。

> ⚠️ 本ライブラリはパフォーマンスを最優先に設計されています。  
> `Calculate()` を呼び出す前に、式が正しい形式であることを保証してください。  
> 検証が必要な場合は `IsValidFormula()` を使用してください。

---

## インストール

### NuGet (.NET / C#)

NuGet からインストール:

```
dotnet add package foriver4725.FormulaCalculator
```

### Unity (UPM)

UPM（Git URL）からインストール:

```
https://github.com/foriver4725/FormulaCalculator.git?path=Unity/Assets/foriver4725/FormulaCalculator
```

---

## 使用上の注意

.NET と Unity のどちらでも、以下の名前空間を使用してください。

```cs
using foriver4725.FormulaCalculator;
```

Unity で Assembly Definition Files（`.asmdef`）を使用している場合は、  
`foriver4725.FormulaCalculator` アセンブリを参照に追加してください。

---

## クイックスタート

### 計算

```cs
using foriver4725.FormulaCalculator;

double result = "1+2*3/(4-5)".AsSpan().Calculate();

// 出力: -5
```

### 検証（ユーザー入力を扱う場合に推奨）

```cs
using foriver4725.FormulaCalculator;

ReadOnlySpan<char> formula = "1+2*3/(4-5)".AsSpan();

if (!formula.IsValidFormula())
{
return;
}

double result = formula.Calculate();
```

---

## API

### Calculate

```cs
public static double Calculate(this ReadOnlySpan<char> formula)
```

### IsValidFormula

```cs
public static bool IsValidFormula(this ReadOnlySpan<char> formula, byte maxNumberDigit = 8)
```

---

## 構文

本ライブラリは一般的な数学表記ルールに従います。

- 四則演算および括弧をサポート
- 演算子の優先順位・結合規則は標準的な数学規則に準拠
- 空白は無視されます

厳密な挙動やエッジケースについては、以下のテストスクリプトをご参照ください:

- [Test Scripts](./dotnet/FormulaCalculator.Tests/Tests.cs)

---

## パフォーマンス

本ライブラリは高パフォーマンスおよび **ゼロヒープアロケーション** を目的として設計されています。

すべての計算は `Span` ベースで行われ、評価中に GC アロケーションは発生しません。

### ベンチマーク (.NET / BenchmarkDotNet)

BenchmarkDotNet を用いて以下の条件で測定しています:

- 異なる式の複雑度
- 複数のループ回数
- アロケーション追跡（`[MemoryDiagnoser]`）

ベンチマークスクリプトはこちら:

- [Benchmark Script](./dotnet/FormulaCalculator.Benchmarks/Benchmarks.cs)

| Formula                                                 | LoopAmount |       Mean |       Error |    StdDev | Allocated |
|---------------------------------------------------------|-----------:|-----------:|------------:|----------:|----------:|
| 2*4-12/3                                                |     10,000 |   0.272 ms |     2.14 us |   1.90 us |         - |
| 2*4-12/3                                                |    100,000 |   2.699 ms |    14.20 us |  13.28 us |         - |
| 2*4-12/3                                                |  1,000,000 |  26.994 ms |    81.99 us |  68.47 us |         - |
| 1+2^(7-3)*3/(4-5)                                       |     10,000 |   0.617 ms |     7.83 us |   7.32 us |         - |
| 1+2^(7-3)*3/(4-5)                                       |    100,000 |   6.134 ms |    37.50 us |  29.27 us |         - |
| 1+2^(7-3)*3/(4-5)                                       |  1,000,000 |  62.369 ms |   667.25 us | 624.14 us |         - |
| ((125-35)*2^3+(80/4-125)*6-7)*(3-2^2)+(5*(90-3/15)^2-4) |     10,000 |   1.986 ms |    18.52 us |  15.47 us |         - |
| ((125-35)*2^3+(80/4-125)*6-7)*(3-2^2)+(5*(90-3/15)^2-4) |    100,000 |  19.947 ms |   114.18 us | 101.22 us |         - |
| ((125-35)*2^3+(80/4-125)*6-7)*(3-2^2)+(5*(90-3/15)^2-4) |  1,000,000 | 199.573 ms | 1,233.04 us | 962.68 us |         - |

主な特性:

- ✅ 0 B GC Alloc
- ✅ 入力長に比例する線形時間評価
- ✅ ループ回数に対して安定した性能

---

## 技術的背景

本実装は以下の古典的アルゴリズムに着想を得ています:

- Dijkstra の **Shunting Yard algorithm**
- 二つのスタックによる演算子優先順位評価

多くの従来実装ではトークン化を行い、その後逆ポーランド記法へ変換するなど二段階処理を行います。

本ライブラリではより直接的な方式を採用しています:

- シングルパス走査
- 値スタック／演算子スタックによる即時簡約
- 中間トークン配列なし
- AST 構築なし
- ヒープアロケーションなし

これにより、入力式の長さに比例した計算コストを実現しています。

安全性が必要な場合は、`IsValidFormula()` を事前に使用してください。

---

## ライセンス

[MIT](./LICENSE)

---

## 開発者向け

バージョンを更新する際は、以下のファイルの設定を必ず一致させてください。

- [dotnet - FormulaCalculator.csproj](./dotnet/FormulaCalculator/FormulaCalculator.csproj)
- [Unity - package.json](./Unity/Assets/foriver4725/FormulaCalculator/package.json)

公開前にバージョン番号が一致していることを確認してください。

環境ごとの詳細な説明は、以下を参照してください。

- [.NET README](./dotnet/README.md)

<!--- - [Unity README]() --->
