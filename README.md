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
public static bool IsValidFormula(this ReadOnlySpan<char> formula)
```

---

## Syntax

This library follows standard mathematical expression rules.

- Supports common arithmetic operators and parentheses.
- Operator precedence and associativity follow conventional math rules.
- Spaces are ignored.

If you need precise and authoritative behavior details,  
please refer to the test scripts in this repository:

- [Test Scripts](https://github.com/foriver4725/FormulaCalculator/blob/main/dotnet/FormulaCalculator.Tests/Tests.cs)

---

## Performance

This library is designed for high performance and **zero heap allocations**
in the hot path.

All evaluations are performed using `Span`-based processing,
and no GC allocations occur during calculation or validation.

### Benchmark (.NET / BenchmarkDotNet)

Performance measurements are taken with BenchmarkDotNet on .NET 8.

Two types of benchmarks are provided:

1. **Method Benchmarks**  
   Benchmarks for the methods provided by FormulaCalculator, measuring execution time and memory allocations for
   formulas of different expression lengths.

2. **Library Comparison Benchmarks**  
   Benchmarks comparing FormulaCalculator with other expression evaluation libraries.  
   Since some libraries do not perform strict validation or support the exact same syntax, this comparison focuses only
   on simple calculation performance.  
   Expressions that depend on syntax available in only some libraries are excluded.

#### Method Benchmarks

- [Benchmark Script](https://github.com/foriver4725/FormulaCalculator/blob/main/dotnet/FormulaCalculator.Benchmarks/Benchmarks.cs)
- [Result Markdown](https://github.com/foriver4725/FormulaCalculator/blob/main/dotnet/BenchmarkResults/BenchmarkResult.md)

**Execution Time**
![Method Benchmark Time Graph](https://github.com/foriver4725/FormulaCalculator/blob/main/dotnet/BenchmarkResults/BenchmarkResultMeanGraph.png)

**Memory Allocations**
![Method Benchmark Allocation Graph](https://github.com/foriver4725/FormulaCalculator/blob/main/dotnet/BenchmarkResults/BenchmarkResultAllocatedGraph.png)

#### Library Comparison Benchmarks

- [Benchmark Script](https://github.com/foriver4725/FormulaCalculator/blob/main/dotnet/FormulaCalculator.Benchmarks.LibraryComparison/Benchmarks.cs)
- [Result Markdown](https://github.com/foriver4725/FormulaCalculator/blob/main/dotnet/BenchmarkResults/BenchmarkLibraryComparisonResult.md)

**Execution Time**
![Library Comparison Time Graph](https://github.com/foriver4725/FormulaCalculator/blob/main/dotnet/BenchmarkResults/BenchmarkLibraryComparisonResultMeanGraph.png)

**Memory Allocations**
![Library Comparison Allocation Graph](https://github.com/foriver4725/FormulaCalculator/blob/main/dotnet/BenchmarkResults/BenchmarkLibraryComparisonResultAllocatedGraph.png)

### Characteristics

- ✅ **0 B GC allocations**
- ✅ Allocation-free evaluation pipeline
- ✅ Execution time scales roughly with expression length
- ✅ Validation remains lightweight compared to evaluation

`IsValidFormula()` can be used when safety is required,
while `Calculate()` alone provides the fastest possible execution path.

## Supported Scope

This library intentionally focuses on **core arithmetic expression evaluation**.

The following kinds of function calls are **not supported**:

- `sqrt(x)`
- `sin(x)`
- `cos(x)`
- `tan(x)`
- other function-style mathematical extensions

This is a deliberate design choice.

Once support for function-style syntax is introduced, the evaluator starts
shifting away from a lightweight arithmetic parser and toward a more
general-purpose expression engine. That direction usually brings additional
branching, token classification, parsing complexity, and runtime overhead.

As more rarely used features accumulate, the baseline cost of evaluating
all expressions tends to rise — including simple formulas that only need
basic arithmetic.

This library avoids that tradeoff on purpose.

Another reason is that expressions such as `sin(x)` or `sqrt(x)` are
somewhat different in character from traditional arithmetic notation built
from operators like `+`, `-`, `*`, and `/`. They are closer to a function-
application style of notation: extensible, expressive, and powerful, but
also conceptually separate from the minimal arithmetic core this library
is designed to handle.

In many cases, such function-style extensions are better implemented by the
consumer side, where they can be added selectively and only when needed.

For example, projects can preprocess custom function calls before passing
the final expression to the evaluator:

```cs
using System;
using System.Globalization;
using foriver4725.FormulaCalculator;

static string PreprocessSin(ReadOnlySpan<char> formula)
{
    const string functionName = "sin(";

    int index = formula.IndexOf(functionName, StringComparison.OrdinalIgnoreCase);
    if (index < 0)
    {
        return formula.ToString();
    }

    int argumentStart = index + functionName.Length;

    int depth = 0;
    int argumentEnd = -1;

    for (int i = argumentStart; i < formula.Length; i++)
    {
        char c = formula[i];

        if (c == '(')
        {
            depth++;
        }
        else if (c == ')')
        {
            if (depth == 0)
            {
                argumentEnd = i;
                break;
            }

            depth--;
        }
    }

    if (argumentEnd < 0)
    {
        throw new FormatException("Missing closing parenthesis for sin().");
    }

    ReadOnlySpan<char> inner = formula.Slice(argumentStart, argumentEnd - argumentStart);

    // recursive preprocessing
    string processedInner = PreprocessSin(inner);

    double value = processedInner.AsSpan().Calculate();
    string replacement = Math.Sin(value).ToString(CultureInfo.InvariantCulture);

    string before = formula.Slice(0, index).ToString();
    string after = formula.Slice(argumentEnd + 1).ToString();

    string combined = before + replacement + after;

    // continue processing remaining string
    return PreprocessSin(combined.AsSpan());
}

string formula = "sin(1+sin(2*3))*2";
string preprocessed = PreprocessSin(formula.AsSpan());
double result = preprocessed.AsSpan().Calculate();
```

This preprocessing can also be implemented using ReadOnlySpan<char>,
keeping allocations minimal while extending the syntax outside
the evaluator.

Because such calls are explicitly wrapped in parentheses, precedence is also
easy to reason about during preprocessing. This approach keeps the evaluator
itself fast and minimal, while allowing each project to extend the syntax
in a way that matches its own requirements.

The goal of this library is to remain:

- fast
- allocation-free
- simple
- proportional to expression length

This library is intended to be a fast arithmetic evaluator,
not a fully extensible symbolic math engine.

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

[MIT](https://github.com/foriver4725/FormulaCalculator/blob/main/LICENSE)
