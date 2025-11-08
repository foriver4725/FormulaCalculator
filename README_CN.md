# FormulaCalculator

## 概述

在 Unity 中提供公式解析与计算功能。  
运行时不会产生堆内存分配，性能极高，开销极低。  

---

## 安装方法

通过 UPM 安装：  
```
https://github.com/foriver4725/FormulaCalculator.git?path=Assets/foriver4725/FormulaCalculator
```

程序集名称与命名空间均为 `foriver4725.FormulaCalculator`。

---

## 使用示例

以下展示了计算方法的完整签名：  
```cs
/// <summary>
/// 解析字符串形式的公式并返回 double 类型的结果。<br/>
/// - 支持的字符：0-9, +, -, *, /, (, ), 以及空格（将被忽略）。<br/>
/// </summary>
/// <param name="formula"> 要计算的公式字符串。 </param>
/// <param name="clampMin"> 结果的最小值。 </param>
/// <param name="clampMax"> 结果的最大值。 </param>
/// <param name="doSkipValidation"> 若为 true，则跳过语法验证（请谨慎使用）。 </param>
/// <param name="maxNumberDigit"> 当 doSkipValidation 为 false 时生效。<br/>限制连续数字的最大长度（需在 int 范围内）。 </param>
/// <returns> 若公式有效，则返回 double 结果；若无效或发生计算错误（如除以零），返回 double.NaN。 </returns>
public static double Calculate(
    this ReadOnlySpan<char> formula,
    double clampMin = short.MinValue,
    double clampMax = short.MaxValue,
    bool doSkipValidation = false,
    byte maxNumberDigit = 8
);
```

此功能以扩展方法形式提供，最简单的用法如下：  
```cs
double result = "1+2*3/(4-5)".AsSpan().Calculate();
```

如果可以确保公式格式正确，  
将 `doSkipValidation` 设置为 `true` 可显著提升性能：  
```cs
double result = "1+2*3/(4-5)".AsSpan().Calculate(doSkipValidation: true);
```

有关性能差异的详细说明，请参考下方 [性能](https://github.com/foriver4725/FormulaCalculator#performance) 部分。

---

## 解析规则说明

- 数字与括号之间必须有运算符，不可省略。  
- 不允许连续运算符，例如：`12 * -3` 是无效的。  
- 与 `-5` 类似，也可以使用正号，如 `+7`。  
- 不允许出现除以零的情况。  

更详细的规则可参考源码 [FormulaCalculator.cs](https://github.com/foriver4725/FormulaCalculator/blob/main/Assets/foriver4725/FormulaCalculator/FormulaCalculator.cs)，  
或参阅后文 [计算流程](https://github.com/foriver4725/FormulaCalculator#calculation-procedure) 章节。

---

## 计算流程

1. 去除并修剪公式中的空格。  
2. 依次执行以下语法验证；若 `doSkipValidation` 为 true，则跳过此步骤。  
```cs
// 检查是否包含非法字符
private static bool IsWholeOK(ReadOnlySpan<char> formula);

// 检查括号外是否紧邻数字，或连续数字是否超出限制
private static bool IsNumberOK(ReadOnlySpan<char> formula, byte maxNumberDigit);

// 检查各类运算符的前后关系是否合法
private static bool IsOperatorOK(ReadOnlySpan<char> formula);

// 检查括号是否成对、顺序是否正确，内部是否有数字
private static bool IsParagraphOK(ReadOnlySpan<char> formula);
```
3. 将相邻数字组合为多位整数（默认最多8位）。  
   过大的数字会超出 `int` 范围，因此限制了最大位数。  
   符号在内部被映射为标识 ID，可参考 [Symbol.cs](https://github.com/foriver4725/FormulaCalculator/blob/main/Assets/foriver4725/FormulaCalculator/Symbol.cs)。  
4. 将整数转换为 `double` 类型并开始浮点计算。  
5. 递归计算括号中的内容，直到不再存在括号。计算顺序为：符号 → 乘除 → 加减。  
6. 为防止结果数值过大（例如除以接近零的值），结果会根据指定范围进行限制。

---

## 性能

几乎所有内部方法都标记为 `inline`，  
并使用 `Span` 进行运算，**完全避免了堆分配**。  

以下为在编辑器中执行 **100 万次** 计算时，`doSkipValidation` 开启与关闭的性能对比结果。  
用于测试的脚本可在 [此处](https://github.com/foriver4725/FormulaCalculator/blob/main/Assets/foriver4725/Profiling/GameManager.cs) 查看。  

| `doSkipValidation` | GC Alloc | Time ms | Self ms |
| :---: | :---: | :---: | :---: |
| `false`（默认） | 0 B | 674.12 | 674.12 |
| `true` | 0 B | 465.79 | 465.79 |

结果显示，跳过验证可使执行时间缩短约 **31%**。
