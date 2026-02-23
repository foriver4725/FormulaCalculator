# FormulaCalculator

其他语言版本:  
[English](./README.md) ｜ [日本語版](./README_JP.md)

## 概述

一个适用于 C# 与 Unity 的高速、零分配、单次扫描表达式计算库。

- **以性能为优先**：默认不进行表达式合法性验证。
- 热路径中 **无堆内存分配**（基于 Span）。
- 支持 **.NET（NuGet）** 与 **Unity（UPM）**。

> ⚠️ 本库以性能为优先。  
> 在调用 `Calculate()` 之前，请确保表达式格式正确。  
> 如需安全验证，请使用 `IsValidFormula()`。

---

## 安装

### NuGet (.NET / C#)

通过 NuGet 安装：

```
dotnet add package foriver4725.FormulaCalculator
```

### Unity (UPM)

通过 UPM（Git URL）安装：

```
https://github.com/foriver4725/FormulaCalculator.git?path=Assets/foriver4725/FormulaCalculator
```

程序集名称与命名空间：

- Assembly: `foriver4725.FormulaCalculator`
- Namespace: `foriver4725.FormulaCalculator`

---

## 快速开始

### 计算

```cs
using foriver4725.FormulaCalculator;

double result = "1+2*3/(4-5)".AsSpan().Calculate();

// 输出: -5
```

### 验证（推荐用于用户输入）

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

## 语法规则

本库遵循标准数学表达式规则：

- 支持常见算术运算符与括号
- 运算符优先级与结合律符合数学规则
- 空格将被忽略

如需了解精确行为与边界情况，请参阅测试脚本：

- [Test Scripts](./dotnet/FormulaCalculator.Tests/Tests.cs)

---

## 性能

本库为高性能设计，计算过程中 **零堆内存分配**。

所有计算均基于 `Span` 实现，在表达式求值过程中不会产生 GC 分配。

### 基准测试 (.NET / BenchmarkDotNet)

基准测试包含：

- 不同复杂度表达式
- 多种循环次数
- 内存分配追踪（`[MemoryDiagnoser]`）

完整基准脚本：

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

主要特性：

- ✅ 0 B GC Alloc
- ✅ 线性时间复杂度
- ✅ 在不同循环规模下性能稳定

---

## 技术说明

实现灵感来源于经典表达式求值算法：

- Dijkstra 的 **Shunting Yard algorithm**
- 双栈运算符优先级算法

传统实现通常先进行词法分析并转换为逆波兰表达式（两步处理）。

本库采用更直接的方式：

- 单次扫描
- 值栈与运算符栈即时归约
- 无中间 token 列表
- 无 AST 构建
- 无堆分配

该设计保证计算成本与输入表达式长度成正比。

如需更高安全性，请先调用 `IsValidFormula()`。

---

## 许可证

[MIT](./LICENSE)