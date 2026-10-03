# FormulaCalculator

A fast, allocation-free arithmetic evaluator for C# and Unity.
Single-pass evaluation with `ReadOnlySpan<char>`, optional validation, and reusable buffers.

## Installation

### NuGet

```bash
dotnet add package foriver4725.FormulaCalculator
```

### Unity 6+ (UPM)

Git URL:

```text
https://github.com/foriver4725/FormulaCalculator.git?path=Unity/Assets/foriver4725/FormulaCalculator
```

The package includes a .NET Standard 2.1 DLL with **Auto Reference** enabled.
If your `.asmdef` uses **Override References**, add `FormulaCalculator.dll`
to **Assembly References**.

## Usage

```cs
using System;
using foriver4725.FormulaCalculator;

double result = "1+2*3/(4-5)".AsSpan().Calculate(); // -5
```

`Calculate()` expects valid syntax. Validate user input before evaluating it:

```cs
ReadOnlySpan<char> formula = "1+2*3/(4-5)".AsSpan();

if (formula.IsValidFormula())
{
    double validatedResult = formula.Calculate();
}
```

### Reusable buffers

The default overload uses stack storage. For long formulas or repeated calls,
supply your own buffers:

```cs
// Allocate once for formulas up to 256 characters long.
double[] valuesBuffer = new double[256];
char[] operatorsBuffer = new char[256];

double first = "1+2*3".AsSpan().Calculate(valuesBuffer, operatorsBuffer); // 7
double second = "(1+2)*3".AsSpan().Calculate(
    valuesBuffer, operatorsBuffer, throwsOnBufferShortage: true); // 9
```

## API

```cs
public static double Calculate(this ReadOnlySpan<char> formula);

public static double Calculate(
    this ReadOnlySpan<char> formula,
    Span<double> valuesBuffer,
    Span<char> operatorsBuffer,
    bool throwsOnBufferShortage = false);

public static bool IsValidFormula(this ReadOnlySpan<char> formula);
```

`Calculate()` returns `double.NaN` for empty input or invalid arithmetic operations.
`IsValidFormula()` checks syntax only.

For the buffer overload:

- Each buffer must have at least `formula.Length` elements.
- Insufficient capacity returns `double.NaN`; `throwsOnBufferShortage: true` throws `ArgumentException`.
- Buffers are scratch storage and need no clearing before reuse, including after `double.NaN`.
- Input and buffers must not overlap; overlap is not checked. Use separate buffers for concurrent calls.

## Syntax

| Feature | Syntax |
| --- | --- |
| Numbers | Integers and decimals, such as `12` and `0.5` |
| Operators | `+`, `-`, `*`, `/`, `%`, `^` |
| Precedence | `^`, then `*` / `/` / `%`, then `+` / `-` |
| Associativity | `^` is right-associative; other operators are left-associative |
| Parentheses | `(1+2)*3` |
| Unary signs | After `(`, with optional spaces: `(-3)`, `(+ 3)`, `(-(1+2))` |
| Spaces | ASCII spaces between tokens; none within number literals |

`%` requires positive integer-valued operands. A zero base requires a positive
exponent; negative bases require integer exponents.

Arithmetic only: functions such as `sin()` and `sqrt()`, variables, symbolic math,
and expression trees are outside the API. Handle custom syntax before evaluation.

[Test cases](https://github.com/foriver4725/FormulaCalculator/blob/main/dotnet/FormulaCalculator.Tests/Tests.cs)

## Benchmarks

Measured with BenchmarkDotNet on .NET 8.

### Methods

[Source](https://github.com/foriver4725/FormulaCalculator/blob/main/dotnet/FormulaCalculator.Benchmarks/Benchmarks.cs)
· [Results](https://github.com/foriver4725/FormulaCalculator/blob/main/dotnet/BenchmarkResults/BenchmarkResult.md)

**Execution time**

![Method execution time](https://raw.githubusercontent.com/foriver4725/FormulaCalculator/main/dotnet/BenchmarkResults/BenchmarkResultMeanGraph.png)

**Managed allocations**

![Method managed allocations](https://raw.githubusercontent.com/foriver4725/FormulaCalculator/main/dotnet/BenchmarkResults/BenchmarkResultAllocatedGraph.png)

### Library comparison

Comparisons use basic arithmetic shared by all evaluators. FormulaCalculator
runs `Calculate()` without a separate validation pass.

| Library | Evaluation path |
| --- | --- |
| [ClosedXML](https://github.com/ClosedXML/ClosedXML) | Excel cell formulas |
| [DataTable.Compute](https://learn.microsoft.com/dotnet/api/system.data.datatable.compute) | Built-in .NET expression evaluation |
| [IronPython](https://github.com/IronLanguages/ironpython3) | Python `eval` |
| [NCalc](https://github.com/ncalc/ncalc) | .NET expression evaluator |
| [xFunc](https://github.com/sys27/xFunc) | Mathematical expression evaluator |
| [ExprTk](https://github.com/ArashPartow/exprtk) | Native C++ expression evaluator |

[Source](https://github.com/foriver4725/FormulaCalculator/blob/main/dotnet/FormulaCalculator.Benchmarks.LibraryComparison/Benchmarks.cs)
· [Results](https://github.com/foriver4725/FormulaCalculator/blob/main/dotnet/BenchmarkResults/BenchmarkLibraryComparisonResult.md)

**Execution time**

![Library comparison execution time](https://raw.githubusercontent.com/foriver4725/FormulaCalculator/main/dotnet/BenchmarkResults/BenchmarkLibraryComparisonResultMeanGraph.png)

**Managed allocations**

![Library comparison managed allocations](https://raw.githubusercontent.com/foriver4725/FormulaCalculator/main/dotnet/BenchmarkResults/BenchmarkLibraryComparisonResultAllocatedGraph.png)

BenchmarkDotNet measures managed allocations only. ExprTk's native allocations
are measured separately below.

### ExprTk native allocations

[Native allocation chart](https://github.com/user-attachments/assets/15bbac7a-dcd5-4b15-9d18-3fa165f31f44)

<details>
<summary>Measurement scope</summary>

A standalone C++ tool counts requested bytes from successful global `new` and
`new[]` allocations, averaged over 1,000 evaluations after 10 warmup calls.
Each evaluation constructs the input string, symbol table, parser, and compiled
expression, evaluates it, and destroys them.

Totals include allocations freed during evaluation; they are not peak or retained
memory. Direct `malloc` / `calloc` / `realloc`, other paths that bypass global
`new`, C# interop, and string marshaling are excluded. These figures are not
directly comparable to managed allocation totals.

</details>

## Design

A two-stack operator-precedence evaluator scans the input once and reduces
operators as it goes. No intermediate token list, RPN buffer, or AST is built.

## Development

- [Build, test, and release commands](https://github.com/foriver4725/FormulaCalculator/blob/main/dotnet/README.md)
- [Repository architecture and workflow](https://github.com/foriver4725/FormulaCalculator/wiki/Repository-Architecture-and-Workflow)

## License

[MIT](https://github.com/foriver4725/FormulaCalculator/blob/main/LICENSE)
