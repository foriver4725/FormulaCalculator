# FormulaCalculator

Available in other languages:  
[中文版本](./README_CN.md) ｜ [日本語版](./README_JP.md)

## Overview

A fast, allocation-free, single-pass formula evaluator for C# and Unity.

- **Focus on speed**: calculation does **not** validate the formula by default.
- **No heap allocations** in the hot path (Span-based).
- Works in **.NET** (via NuGet) and **Unity** (via UPM).

> ⚠️ This library prioritizes performance.  
> Make sure your formula is well-formed **before** calling `Calculate()`.  
> Use `IsValidFormula()` when you need validation.

---

## Installation

### NuGet (.NET / C#)

Install from NuGet:

```
dotnet add package foriver4725.FormulaCalculator
```

### Unity (UPM)

Install via UPM (Git URL):

```
https://github.com/foriver4725/FormulaCalculator.git?path=Assets/foriver4725/FormulaCalculator
```

The names of both the assembly and the namespace are:

- Assembly: `foriver4725.FormulaCalculator`
- Namespace: `foriver4725.FormulaCalculator`

---

## Quick Start

### Calculate

```cs
using foriver4725.FormulaCalculator;

double result = "1+2*3/(4-5)".AsSpan().Calculate();

// Output: -5
```

### Validate (recommended when input is user-generated)

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

## Syntax

This library follows standard mathematical expression rules.

- Supports common arithmetic operators and parentheses.
- Operator precedence and associativity follow conventional math rules.
- Spaces are ignored.

If you need precise and authoritative behavior details,  
please refer to the test scripts in this repository:

- [Test Scripts](./dotnet/FormulaCalculator.Tests/Tests.cs)

---

## Performance

This library is designed for high performance and **zero heap allocations** in the hot path.

All calculations are performed using `Span`-based processing, and no GC allocations occur during evaluation.

### Benchmark (.NET / BenchmarkDotNet)

Performance is measured using BenchmarkDotNet with:

- Different expression complexities
- Multiple loop counts
- Allocation tracking (`[MemoryDiagnoser]`)

You can find the full benchmark script here:

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

Typical characteristics:

- ✅ 0 B GC Alloc
- ✅ Linear-time evaluation
- ✅ Stable performance across different loop scales

## Notes

The implementation is inspired by classical expression evaluation techniques such as:

- Dijkstra’s **Shunting Yard algorithm**
- Two-stack operator-precedence evaluation

Many traditional implementations tokenize the input first and then perform
a second pass (e.g., converting to Reverse Polish Notation before evaluation).

This library takes a more direct approach:

- Single-pass scanning
- Immediate reduction using value/operator stacks
- No intermediate token list
- No AST construction
- No heap allocations

The goal is to minimize overhead while preserving standard mathematical behavior.

If safety is required, validate the expression first using `IsValidFormula()`.

This design keeps the evaluation cost proportional to the length of the input expression.

---

## License

[MIT](./LICENSE)