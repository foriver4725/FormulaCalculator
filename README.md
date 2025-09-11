# FormulaCalculator

## Description
Provides formula parsing and calculation functionality in Unity.<br/>
It runs fast without heap allocations and keeps overhead to a minimum.<br/>

## How to Use
Install via UPM: 
```
https://github.com/foriver4725/FormulaCalculator.git?path=Assets/foriver4725/FormulaCalculator
```
The names of both the assembly and the namespace are `foriver4725.FormulaCalculator`.

## Usage Example
The following shows the complete signature of the calculation method.
```cs
/// <summary>
/// Calculate the formula given as a string and return the result as a double.<br/>
/// - Supported characters: 0-9, +, -, *, /, (, ), and space (used for ignoring).<br/>
/// </summary>
/// <param name="formula"> The formula to be calculated. </param>
/// <param name="clampMin"> The minimum value to clamp the result to. </param>
/// <param name="clampMax"> The maximum value to clamp the result to. </param>
/// <param name="doSkipValidation"> If true, skips validation checks. (Be careful!) </param>
/// <param name="maxNumberDigit"> Valid when doSkipValidation is false.<br/>The maximum number of digits allowed when concatenating numbers (as long as it remains within the range of int). </param>
/// <returns> The result of the calculation as a double. If the formula is invalid or an error occurs during calculation (such as division by zero), returns double.NaN. </returns>
public static double Calculate(
    this string formula,
    double clampMin = short.MinValue,
    double clampMax = short.MaxValue,
    bool doSkipValidation = false,
    byte maxNumberDigit = 8
);
```

<br/>

The functionality is provided as extension methods, and in the simplest example, you can use it as follows:
```cs
double result = "1+2*3/(4-5)".Calculate();
```

<br/>

If you can guarantee that the formula is in the correct format,<br/>
setting the `doSkipValidation` flag to `true` can significantly improve performance.
```cs
double result = "1+2*3/(4-5)".Calculate(doSkipValidation: true);
```

<br/>

For details on how much the performance can be improved, please refer to the [Performance](https://github.com/foriver4725/FormulaCalculator#performance) section below.

## Details of Parsing Rules
- An operator is required between numbers and parentheses; omission is not allowed.
- In no case are consecutive operators allowed. For example, the following formula is invalid: `12 * -3`.
- Just like `-5`, you can also add a positive sign, such as `+7`.
- Division by zero is not allowed.

For detailed rules not described here, please refer directly to the source code in [FormulaCalculator.cs](https://github.com/foriver4725/FormulaCalculator/blob/main/Assets/foriver4725/FormulaCalculator/FormulaCalculator.cs), or check the calculation procedure explained in the [Calculation Procedure](https://github.com/foriver4725/FormulaCalculator#calculation-procedure) section below.

## Calculation Procedure
1. Removes and trims the whitespace from the given formula.
2. Performs the following validations in order to ensure that the formula syntax is in the correct format. If the `doSkipValidation` flag is set to `true`, this process will be skipped.
```cs
// Check if the formula does not contain any invalid characters
private static bool IsWholeOK(ReadOnlySpan<char> formula);

// Check if a number does not come immediately outside the parentheses
// Check if there is no consecutive numbers exceeding a certain length
private static bool IsNumberOK(ReadOnlySpan<char> formula, byte maxNumberDigit);

// Check for operators "+", "-" that "the previous element exists and is either a number, '(', or ')', or the previous element does not exist" and "the next element exists and is either a number or '('"
// Check for operators other than "+", "-" that "the previous element exists and is either a number or ')'" and "the next element exists and is either a number or '('"
private static bool IsOperatorOK(ReadOnlySpan<char> formula);

// Check if all parentheses match and are in the correct order
// Check if there is at least one number inside the parentheses
// Check if the arrangement of ")(" does not exist
private static bool IsParagraphOK(ReadOnlySpan<char> formula);
```
3. Adjacent digits are combined and evaluated as multi-digit integer values. To ensure the result does not exceed the range representable by `int`, the maximum number of digits that can be combined is limited to 8. This limit can also be explicitly specified through the method’s arguments.<br/>In addition, for symbols, integer values that are unlikely to appear in the calculation are assigned as identifier IDs. For details on this mapping, please refer to [Symbol.cs](https://github.com/foriver4725/FormulaCalculator/blob/main/Assets/foriver4725/FormulaCalculator/Symbol.cs).
4. Casts integer values of type `int` to type `double`. From this point on, calculation errors may occur.
5. Executes the core calculation logic. It recursively evaluates the contents of parentheses until none remain.<br/>In the arithmetic stage, it first processes unary signs (positive/negative), then performs multiplication and division, and finally addition and subtraction.
6. To prevent results from becoming abnormally large or small, such as when dividing by values close to zero, the calculation results are clamped to an appropriate range. The upper and lower limits of this range can also be explicitly specified through the method’s arguments.

## Performance
Almost all internal methods are marked with the inline attribute, and all calculations are performed using `Span` without any heap allocations.<br/>
The following shows the performance results measured in the editor by executing **one million** runs with the `doSkipValidation` flag turned on and off, respectively.<br/>
The script used for this performance measurement can be found [here](https://github.com/foriver4725/FormulaCalculator/blob/main/Assets/foriver4725/Profiling/GameManager.cs).

| `doSkipValidation` flag | GC Alloc | Time ms | Self ms |
| :---: | :---: | :---: | :---: |
| `false` (default) | 0 B | 674.12 | 674.12 |
| `true` | 0 B | 465.79 | 465.79 |

In this case, skipping formula validation reduces the execution time by **31%**.
