# FormulaCalculator

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
https://github.com/foriver4725/FormulaCalculator.git?path=Unity/Assets/foriver4725/FormulaCalculator
```

---

## Usage Notes

For both .NET and Unity, use the following namespace:

```cs
using foriver4725.FormulaCalculator;
```

If you are using Assembly Definition Files (`.asmdef`) in Unity,
make sure to add a reference to the `foriver4725.FormulaCalculator` assembly.

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

Performance is evaluated using BenchmarkDotNet.

The benchmarks measure expression evaluation performance across
different formula complexities while tracking memory allocations.

- [Benchmark Script](./dotnet/FormulaCalculator.Benchmarks/Benchmarks.cs)
- [Benchmark Results](./dotnet/BenchmarkResult.md)

Key characteristics:

- ✅ 0 B GC allocations
- ✅ Allocation-free evaluation pipeline
- ✅ Stable performance across varying expression complexity

## Design

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

---

## For Developers

When updating the version, make sure to update it consistently in:

- [dotnet - FormulaCalculator.csproj](./dotnet/FormulaCalculator/FormulaCalculator.csproj)
- [Unity - package.json](./Unity/Assets/foriver4725/FormulaCalculator/package.json)

Keep the version numbers aligned before publishing.

Additional documentation for each environment:

- [.NET README](./dotnet/README.md)

<!--- - [Unity README]() --->
