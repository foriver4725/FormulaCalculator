# FormulaCalculator

## 概要

Unity上で数式を解析し、計算を行う機能を提供します。  
ヒープアロケーションを発生させず、高速に動作し、オーバーヘッドを最小限に抑えています。  

---

## 導入方法

UPM経由でインストールします：  
```
https://github.com/foriver4725/FormulaCalculator.git?path=Assets/foriver4725/FormulaCalculator
```

アセンブリ名と名前空間はいずれも `foriver4725.FormulaCalculator` です。

---

## 使用例

以下は計算メソッドの完全なシグネチャです：  
```cs
/// <summary>
/// 数式文字列を計算し、結果を double 型で返します。<br/>
/// - 使用可能な文字：0〜9, +, -, *, /, (, ), スペース（無視されます）<br/>
/// </summary>
/// <param name="formula"> 計算対象の数式文字列。 </param>
/// <param name="clampMin"> 結果の下限値。 </param>
/// <param name="clampMax"> 結果の上限値。 </param>
/// <param name="doSkipValidation"> true の場合、構文チェックをスキップします（使用注意）。 </param>
/// <param name="maxNumberDigit"> doSkipValidation が false の場合に有効。<br/>連続した数字として扱う桁数の上限（int範囲内）。 </param>
/// <returns> 結果を double で返します。式が無効または計算エラー（例：0除算）が発生した場合は double.NaN を返します。 </returns>
public static double Calculate(
    this ReadOnlySpan<char> formula,
    double clampMin = short.MinValue,
    double clampMax = short.MaxValue,
    bool doSkipValidation = false,
    byte maxNumberDigit = 8
);
```

この機能は拡張メソッドとして提供されており、もっとも簡単な例は以下の通りです：  
```cs
double result = "1+2*3/(4-5)".AsSpan().Calculate();
```

構文の正しさが保証されている場合、  
`doSkipValidation` フラグを `true` に設定するとパフォーマンスが大幅に向上します：  
```cs
double result = "1+2*3/(4-5)".AsSpan().Calculate(doSkipValidation: true);
```

詳細なパフォーマンス結果については、  
後述の [パフォーマンス](https://github.com/foriver4725/FormulaCalculator/blob/main/README_JP.md#%E3%83%91%E3%83%95%E3%82%A9%E3%83%BC%E3%83%9E%E3%83%B3%E3%82%B9) セクションを参照してください。

---

## 構文ルールの詳細

- 数字と括弧の間には必ず演算子が必要です（省略不可）。  
- 連続する演算子は使用できません（例：`12 * -3` は無効）。  
- `-5` のように負号だけでなく、`+7` のような正号も許可されます。  
- 0による除算は禁止されています。  

詳細なルールは [FormulaCalculator.cs](https://github.com/foriver4725/FormulaCalculator/blob/main/Assets/foriver4725/FormulaCalculator/FormulaCalculator.cs) を参照、  
または後述の [計算手順](https://github.com/foriver4725/FormulaCalculator/blob/main/README_JP.md#%E8%A8%88%E7%AE%97%E6%89%8B%E9%A0%86) を確認してください。

---

## 計算手順

1. 受け取った数式文字列から空白を削除します。  
2. 以下のチェックを行い、構文が正しいか検証します。`doSkipValidation` が true の場合、この検証はスキップされます。  
```cs
// 不正な文字が含まれていないかチェック
private static bool IsWholeOK(ReadOnlySpan<char> formula);

// 括弧の直外に数字がないか、桁数制限を超えていないかをチェック
private static bool IsNumberOK(ReadOnlySpan<char> formula, byte maxNumberDigit);

// 各演算子の前後関係をチェック
private static bool IsOperatorOK(ReadOnlySpan<char> formula);

// 括弧の対応関係・配置・内部要素をチェック
private static bool IsParagraphOK(ReadOnlySpan<char> formula);
```
3. 隣接する数字をまとめて多桁の整数値として評価します。  
   桁数上限（デフォルト8桁）を超えると `int` 範囲を逸脱するため、制限を設けています。  
   記号については [Symbol.cs](https://github.com/foriver4725/FormulaCalculator/blob/main/Assets/foriver4725/FormulaCalculator/Symbol.cs) で定義されたIDにマッピングされます。  
4. 整数値を `double` にキャストし、以降は浮動小数演算を行います。  
5. 括弧内を再帰的に評価し、残らなくなるまで展開します。演算順序は、符号処理 → 乗除 → 加減 の順です。  
6. 計算結果が極端な値にならないよう、上限・下限を指定範囲でクランプします。

---

## パフォーマンス

内部メソッドのほぼすべてに `inline` 属性が付与されており、  
計算はすべて `Span` を使用してヒープアロケーションなしで実行されます。  

以下は、`doSkipValidation` の有無で**100万回**の計算を行った際のエディタ上での計測結果です。  
計測スクリプトは [こちら](https://github.com/foriver4725/FormulaCalculator/blob/main/Assets/foriver4725/Profiling/GameManager.cs) にあります。  

| `doSkipValidation` | GC Alloc | Time ms | Self ms |
| :---: | :---: | :---: | :---: |
| `false` (デフォルト) | 0 B | 674.12 | 674.12 |
| `true` | 0 B | 465.79 | 465.79 |

構文チェックをスキップすることで、**約31%の高速化** が確認されています。

## Pure C# 版
本ライブラリは Pure C# にも移植されています。<br/>
Unity を使用しない環境で利用したい場合は、[こちらのリポジトリ](https://github.com/foriver4725/Formuler) からコンパイル済みの DLL をダウンロードできます。<br/>
