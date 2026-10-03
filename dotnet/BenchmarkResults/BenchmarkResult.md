```

BenchmarkDotNet v0.15.8, macOS 27.0.1 (26A434) [Darwin 27.0.0]
Apple M3, 1 CPU, 8 logical and 8 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a


```
| Method                        | Input                | Case                 | Char Count | Mean       | Error     | StdDev    | Allocated |
|------------------------------ |--------------------- |--------------------- |-----------:|-----------:|----------:|----------:|----------:|
| Calculate                     | Len08_Mod_Pow        | Len08_Mod_Pow        |          7 |  25.671 ns | 0.3219 ns | 0.2853 ns |         - |
| Calculate                     | Len08_Decimal_AddMul | Len08_Decimal_AddMul |          8 |  14.917 ns | 0.1458 ns | 0.1292 ns |         - |
| Calculate                     | Len08_Whitespace     | Len08_Whitespace     |          8 |  16.053 ns | 0.0249 ns | 0.0221 ns |         - |
| Calculate                     | Len16_Mod_Decimal    | Len16_Mod_Decimal    |         14 |  40.061 ns | 0.1629 ns | 0.1524 ns |         - |
| Calculate                     | Len16_Pow_Decimal    | Len16_Pow_Decimal    |         15 |  31.930 ns | 0.3934 ns | 0.3285 ns |         - |
| Calculate                     | Len16_Whitespace     | Len16_Whitespace     |         15 |  35.275 ns | 0.5241 ns | 0.4646 ns |         - |
| Calculate                     | Len64_Mixed_02       | Len64_Mixed_02       |         57 | 164.178 ns | 3.2034 ns | 3.6890 ns |         - |
| Calculate                     | Len64_Mixed_03       | Len64_Mixed_03       |         57 | 132.367 ns | 0.8327 ns | 0.7789 ns |         - |
| Calculate                     | Len64_Mixed_01       | Len64_Mixed_01       |         64 | 181.325 ns | 3.5479 ns | 3.1451 ns |         - |
| IsValidFormula                | Len08_Mod_Pow        | Len08_Mod_Pow        |          7 |  11.670 ns | 0.0525 ns | 0.0491 ns |         - |
| IsValidFormula                | Len08_Decimal_AddMul | Len08_Decimal_AddMul |          8 |   7.214 ns | 0.0315 ns | 0.0263 ns |         - |
| IsValidFormula                | Len08_Whitespace     | Len08_Whitespace     |          8 |   9.445 ns | 0.0956 ns | 0.0847 ns |         - |
| IsValidFormula                | Len16_Mod_Decimal    | Len16_Mod_Decimal    |         14 |  20.376 ns | 0.3388 ns | 0.2829 ns |         - |
| IsValidFormula                | Len16_Pow_Decimal    | Len16_Pow_Decimal    |         15 |  19.144 ns | 0.3879 ns | 0.3629 ns |         - |
| IsValidFormula                | Len16_Whitespace     | Len16_Whitespace     |         15 |  20.390 ns | 0.3468 ns | 0.3074 ns |         - |
| IsValidFormula                | Len64_Mixed_02       | Len64_Mixed_02       |         57 |  86.924 ns | 1.5002 ns | 2.2454 ns |         - |
| IsValidFormula                | Len64_Mixed_03       | Len64_Mixed_03       |         57 |  81.124 ns | 0.7950 ns | 0.7047 ns |         - |
| IsValidFormula                | Len64_Mixed_01       | Len64_Mixed_01       |         64 |  98.536 ns | 0.2344 ns | 0.1830 ns |         - |
| Calculate_With_IsValidFormula | Len08_Mod_Pow        | Len08_Mod_Pow        |          7 |  38.090 ns | 0.2295 ns | 0.2034 ns |         - |
| Calculate_With_IsValidFormula | Len08_Decimal_AddMul | Len08_Decimal_AddMul |          8 |  21.836 ns | 0.1106 ns | 0.0924 ns |         - |
| Calculate_With_IsValidFormula | Len08_Whitespace     | Len08_Whitespace     |          8 |  26.082 ns | 0.4000 ns | 0.3546 ns |         - |
| Calculate_With_IsValidFormula | Len16_Mod_Decimal    | Len16_Mod_Decimal    |         14 |  57.646 ns | 0.4676 ns | 0.4145 ns |         - |
| Calculate_With_IsValidFormula | Len16_Pow_Decimal    | Len16_Pow_Decimal    |         15 |  52.581 ns | 1.0335 ns | 0.8630 ns |         - |
| Calculate_With_IsValidFormula | Len16_Whitespace     | Len16_Whitespace     |         15 |  56.113 ns | 0.7693 ns | 0.6424 ns |         - |
| Calculate_With_IsValidFormula | Len64_Mixed_02       | Len64_Mixed_02       |         57 | 247.505 ns | 3.4558 ns | 3.2325 ns |         - |
| Calculate_With_IsValidFormula | Len64_Mixed_03       | Len64_Mixed_03       |         57 | 215.421 ns | 4.1510 ns | 5.0978 ns |         - |
| Calculate_With_IsValidFormula | Len64_Mixed_01       | Len64_Mixed_01       |         64 | 288.428 ns | 5.7067 ns | 7.2172 ns |         - |
