```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.3.1 (25D2128) [Darwin 25.3.0]
Apple M3, 1 CPU, 8 logical and 8 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a


```
| Method                        | Input                | Case                 | Char Count | Mean       | Error     | StdDev    | Allocated |
|------------------------------ |--------------------- |--------------------- |-----------:|-----------:|----------:|----------:|----------:|
| Calculate                     | Len08_Mod_Pow        | Len08_Mod_Pow        |          7 |  40.377 ns | 0.6925 ns | 0.6801 ns |         - |
| Calculate                     | Len08_Decimal_AddMul | Len08_Decimal_AddMul |          8 |  21.963 ns | 0.4302 ns | 0.3813 ns |         - |
| Calculate                     | Len08_Whitespace     | Len08_Whitespace     |          8 |  23.632 ns | 0.4451 ns | 0.3945 ns |         - |
| Calculate                     | Len16_Mod_Decimal    | Len16_Mod_Decimal    |         14 |  48.186 ns | 0.3603 ns | 0.2813 ns |         - |
| Calculate                     | Len16_Pow_Decimal    | Len16_Pow_Decimal    |         15 |  45.963 ns | 0.1839 ns | 0.1536 ns |         - |
| Calculate                     | Len16_Whitespace     | Len16_Whitespace     |         15 |  46.346 ns | 0.3232 ns | 0.2699 ns |         - |
| Calculate                     | Len64_Mixed_02       | Len64_Mixed_02       |         57 | 198.759 ns | 3.9878 ns | 7.0883 ns |         - |
| Calculate                     | Len64_Mixed_03       | Len64_Mixed_03       |         57 | 168.119 ns | 3.3034 ns | 7.4563 ns |         - |
| Calculate                     | Len64_Mixed_01       | Len64_Mixed_01       |         64 | 223.264 ns | 4.4610 ns | 5.4785 ns |         - |
| IsValidFormula                | Len08_Mod_Pow        | Len08_Mod_Pow        |          7 |  12.681 ns | 0.0526 ns | 0.0439 ns |         - |
| IsValidFormula                | Len08_Decimal_AddMul | Len08_Decimal_AddMul |          8 |   8.576 ns | 0.1661 ns | 0.1387 ns |         - |
| IsValidFormula                | Len08_Whitespace     | Len08_Whitespace     |          8 |  11.777 ns | 0.0418 ns | 0.0349 ns |         - |
| IsValidFormula                | Len16_Mod_Decimal    | Len16_Mod_Decimal    |         14 |  20.222 ns | 0.0641 ns | 0.0568 ns |         - |
| IsValidFormula                | Len16_Pow_Decimal    | Len16_Pow_Decimal    |         15 |  18.378 ns | 0.1467 ns | 0.1301 ns |         - |
| IsValidFormula                | Len16_Whitespace     | Len16_Whitespace     |         15 |  22.046 ns | 0.2055 ns | 0.1605 ns |         - |
| IsValidFormula                | Len64_Mixed_02       | Len64_Mixed_02       |         57 |  81.461 ns | 0.6471 ns | 0.5403 ns |         - |
| IsValidFormula                | Len64_Mixed_03       | Len64_Mixed_03       |         57 |  80.978 ns | 0.8057 ns | 0.7143 ns |         - |
| IsValidFormula                | Len64_Mixed_01       | Len64_Mixed_01       |         64 |  99.041 ns | 0.2959 ns | 0.2471 ns |         - |
| Calculate_With_IsValidFormula | Len08_Mod_Pow        | Len08_Mod_Pow        |          7 |  52.891 ns | 0.2190 ns | 0.1829 ns |         - |
| Calculate_With_IsValidFormula | Len08_Decimal_AddMul | Len08_Decimal_AddMul |          8 |  30.132 ns | 0.0540 ns | 0.0479 ns |         - |
| Calculate_With_IsValidFormula | Len08_Whitespace     | Len08_Whitespace     |          8 |  34.625 ns | 0.1502 ns | 0.1255 ns |         - |
| Calculate_With_IsValidFormula | Len16_Mod_Decimal    | Len16_Mod_Decimal    |         14 |  67.294 ns | 0.2999 ns | 0.2659 ns |         - |
| Calculate_With_IsValidFormula | Len16_Pow_Decimal    | Len16_Pow_Decimal    |         15 |  64.553 ns | 0.7803 ns | 0.6917 ns |         - |
| Calculate_With_IsValidFormula | Len16_Whitespace     | Len16_Whitespace     |         15 |  68.886 ns | 1.3862 ns | 1.2288 ns |         - |
| Calculate_With_IsValidFormula | Len64_Mixed_02       | Len64_Mixed_02       |         57 | 282.921 ns | 4.6718 ns | 3.9012 ns |         - |
| Calculate_With_IsValidFormula | Len64_Mixed_03       | Len64_Mixed_03       |         57 | 244.959 ns | 3.7601 ns | 3.3332 ns |         - |
| Calculate_With_IsValidFormula | Len64_Mixed_01       | Len64_Mixed_01       |         64 | 323.554 ns | 4.1409 ns | 3.6708 ns |         - |
