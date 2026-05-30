```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.5 (25F71) [Darwin 25.5.0]
Apple M3, 1 CPU, 8 logical and 8 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a


```
| Method                        | Input                | Case                 | Char Count | Mean       | Error     | StdDev    | Allocated |
|------------------------------ |--------------------- |--------------------- |-----------:|-----------:|----------:|----------:|----------:|
| Calculate                     | Len08_Mod_Pow        | Len08_Mod_Pow        |          7 |  33.252 ns | 0.5347 ns | 0.4465 ns |         - |
| Calculate                     | Len08_Decimal_AddMul | Len08_Decimal_AddMul |          8 |  17.841 ns | 0.3593 ns | 0.3689 ns |         - |
| Calculate                     | Len08_Whitespace     | Len08_Whitespace     |          8 |  19.005 ns | 0.0526 ns | 0.0439 ns |         - |
| Calculate                     | Len16_Mod_Decimal    | Len16_Mod_Decimal    |         14 |  40.973 ns | 0.2759 ns | 0.2446 ns |         - |
| Calculate                     | Len16_Pow_Decimal    | Len16_Pow_Decimal    |         15 |  38.453 ns | 0.2866 ns | 0.2681 ns |         - |
| Calculate                     | Len16_Whitespace     | Len16_Whitespace     |         15 |  39.978 ns | 0.6000 ns | 0.4685 ns |         - |
| Calculate                     | Len64_Mixed_02       | Len64_Mixed_02       |         57 | 170.588 ns | 1.7754 ns | 1.5738 ns |         - |
| Calculate                     | Len64_Mixed_03       | Len64_Mixed_03       |         57 | 145.722 ns | 2.0217 ns | 1.7922 ns |         - |
| Calculate                     | Len64_Mixed_01       | Len64_Mixed_01       |         64 | 200.181 ns | 3.7449 ns | 3.5029 ns |         - |
| IsValidFormula                | Len08_Mod_Pow        | Len08_Mod_Pow        |          7 |  11.820 ns | 0.1849 ns | 0.1639 ns |         - |
| IsValidFormula                | Len08_Decimal_AddMul | Len08_Decimal_AddMul |          8 |   7.236 ns | 0.0219 ns | 0.0194 ns |         - |
| IsValidFormula                | Len08_Whitespace     | Len08_Whitespace     |          8 |   9.399 ns | 0.0280 ns | 0.0249 ns |         - |
| IsValidFormula                | Len16_Mod_Decimal    | Len16_Mod_Decimal    |         14 |  19.864 ns | 0.0496 ns | 0.0387 ns |         - |
| IsValidFormula                | Len16_Pow_Decimal    | Len16_Pow_Decimal    |         15 |  18.550 ns | 0.0692 ns | 0.0647 ns |         - |
| IsValidFormula                | Len16_Whitespace     | Len16_Whitespace     |         15 |  20.157 ns | 0.0860 ns | 0.0763 ns |         - |
| IsValidFormula                | Len64_Mixed_02       | Len64_Mixed_02       |         57 |  84.507 ns | 0.7267 ns | 0.6068 ns |         - |
| IsValidFormula                | Len64_Mixed_03       | Len64_Mixed_03       |         57 |  81.659 ns | 0.9581 ns | 0.8962 ns |         - |
| IsValidFormula                | Len64_Mixed_01       | Len64_Mixed_01       |         64 |  98.666 ns | 0.5266 ns | 0.4668 ns |         - |
| Calculate_With_IsValidFormula | Len08_Mod_Pow        | Len08_Mod_Pow        |          7 |  44.732 ns | 0.1811 ns | 0.1694 ns |         - |
| Calculate_With_IsValidFormula | Len08_Decimal_AddMul | Len08_Decimal_AddMul |          8 |  25.210 ns | 0.1623 ns | 0.1439 ns |         - |
| Calculate_With_IsValidFormula | Len08_Whitespace     | Len08_Whitespace     |          8 |  29.158 ns | 0.5911 ns | 0.7037 ns |         - |
| Calculate_With_IsValidFormula | Len16_Mod_Decimal    | Len16_Mod_Decimal    |         14 |  62.350 ns | 1.2767 ns | 2.3979 ns |         - |
| Calculate_With_IsValidFormula | Len16_Pow_Decimal    | Len16_Pow_Decimal    |         15 |  57.650 ns | 0.7354 ns | 0.6519 ns |         - |
| Calculate_With_IsValidFormula | Len16_Whitespace     | Len16_Whitespace     |         15 |  59.610 ns | 0.6384 ns | 0.5331 ns |         - |
| Calculate_With_IsValidFormula | Len64_Mixed_02       | Len64_Mixed_02       |         57 | 262.686 ns | 5.1942 ns | 7.4494 ns |         - |
| Calculate_With_IsValidFormula | Len64_Mixed_03       | Len64_Mixed_03       |         57 | 234.512 ns | 4.6677 ns | 4.5843 ns |         - |
| Calculate_With_IsValidFormula | Len64_Mixed_01       | Len64_Mixed_01       |         64 | 313.203 ns | 6.0734 ns | 6.9942 ns |         - |
