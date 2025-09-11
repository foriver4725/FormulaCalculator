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

## Parsing Rules
Now Creating!

## Performance
Now Creating!
