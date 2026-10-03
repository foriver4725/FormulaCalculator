```

BenchmarkDotNet v0.15.8, macOS 27.0.1 (26A434) [Darwin 27.0.0]
Apple M3, 1 CPU, 8 logical and 8 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a


```
| Method                      | Input   | Case    | Char Count | Mean         | Error      | StdDev     | Gen0   | Gen1   | Allocated |
|---------------------------- |-------- |-------- |-----------:|-------------:|-----------:|-----------:|-------:|-------:|----------:|
| Calculate_FormulaCalculator | Len08_A | Len08_A |          8 |     14.21 ns |   0.294 ns |   0.260 ns |      - |      - |         - |
| Calculate_FormulaCalculator | Len08_B | Len08_B |          9 |     14.27 ns |   0.072 ns |   0.067 ns |      - |      - |         - |
| Calculate_FormulaCalculator | Len08_C | Len08_C |          9 |     14.29 ns |   0.310 ns |   0.319 ns |      - |      - |         - |
| Calculate_FormulaCalculator | Len16_A | Len16_A |         16 |     29.21 ns |   0.480 ns |   0.449 ns |      - |      - |         - |
| Calculate_FormulaCalculator | Len16_B | Len16_B |         16 |     28.18 ns |   0.302 ns |   0.268 ns |      - |      - |         - |
| Calculate_FormulaCalculator | Len16_C | Len16_C |         17 |     25.65 ns |   0.506 ns |   0.497 ns |      - |      - |         - |
| Calculate_FormulaCalculator | Len64_A | Len64_A |         64 |    159.21 ns |   3.113 ns |   3.937 ns |      - |      - |         - |
| Calculate_FormulaCalculator | Len64_B | Len64_B |         65 |    125.71 ns |   1.785 ns |   1.582 ns |      - |      - |         - |
| Calculate_FormulaCalculator | Len64_C | Len64_C |         67 |    141.49 ns |   2.693 ns |   2.645 ns |      - |      - |         - |
| Calculate_ClosedXml         | Len08_A | Len08_A |          8 |    840.02 ns |  10.464 ns |   9.788 ns | 0.2050 |      - |    1720 B |
| Calculate_ClosedXml         | Len08_B | Len08_B |          9 |    876.03 ns |  12.693 ns |  11.873 ns | 0.2041 |      - |    1720 B |
| Calculate_ClosedXml         | Len08_C | Len08_C |          9 |    853.12 ns |  16.397 ns |  13.693 ns | 0.2050 |      - |    1720 B |
| Calculate_ClosedXml         | Len16_A | Len16_A |         16 |  1,114.07 ns |   5.264 ns |   4.924 ns | 0.2403 |      - |    2016 B |
| Calculate_ClosedXml         | Len16_B | Len16_B |         16 |  1,119.17 ns |   3.837 ns |   3.589 ns | 0.2403 | 0.0019 |    2016 B |
| Calculate_ClosedXml         | Len16_C | Len16_C |         17 |  1,124.61 ns |   6.029 ns |   5.345 ns | 0.2403 |      - |    2016 B |
| Calculate_ClosedXml         | Len64_A | Len64_A |         64 |  3,885.53 ns |   6.675 ns |   5.917 ns | 0.5112 |      - |    4336 B |
| Calculate_ClosedXml         | Len64_B | Len64_B |         65 |  3,514.76 ns |  22.778 ns |  21.306 ns | 0.4883 |      - |    4096 B |
| Calculate_ClosedXml         | Len64_C | Len64_C |         67 |  3,721.01 ns |  14.763 ns |  13.087 ns | 0.4959 |      - |    4176 B |
| Calculate_DataTable         | Len08_A | Len08_A |          8 |    390.18 ns |   4.570 ns |   4.275 ns | 0.3133 | 0.0005 |    2624 B |
| Calculate_DataTable         | Len08_B | Len08_B |          9 |    419.08 ns |   4.351 ns |   3.857 ns | 0.3152 | 0.0005 |    2640 B |
| Calculate_DataTable         | Len08_C | Len08_C |          9 |    396.95 ns |   6.490 ns |   6.071 ns | 0.3152 | 0.0005 |    2640 B |
| Calculate_DataTable         | Len16_A | Len16_A |         16 |    577.48 ns |   4.733 ns |   4.427 ns | 0.3548 |      - |    2992 B |
| Calculate_DataTable         | Len16_B | Len16_B |         16 |    606.75 ns |   3.901 ns |   3.649 ns | 0.3586 |      - |    3000 B |
| Calculate_DataTable         | Len16_C | Len16_C |         17 |    642.07 ns |   3.529 ns |   3.301 ns | 0.3586 |      - |    3016 B |
| Calculate_DataTable         | Len64_A | Len64_A |         64 |  2,444.56 ns |  14.479 ns |  13.544 ns | 0.8011 | 0.0038 |    6704 B |
| Calculate_DataTable         | Len64_B | Len64_B |         65 |  2,304.38 ns |  18.936 ns |  17.713 ns | 0.7324 |      - |    6152 B |
| Calculate_DataTable         | Len64_C | Len64_C |         67 |  2,394.17 ns |  13.074 ns |  12.229 ns | 0.7477 |      - |    6344 B |
| Calculate_IronPython        | Len08_A | Len08_A |          8 | 10,419.76 ns | 179.299 ns | 167.716 ns | 5.0964 | 0.4578 |   42683 B |
| Calculate_IronPython        | Len08_B | Len08_B |          9 | 10,165.54 ns | 117.421 ns | 104.090 ns | 5.1117 | 0.3815 |   42836 B |
| Calculate_IronPython        | Len08_C | Len08_C |          9 | 10,040.54 ns |  90.909 ns |  80.588 ns | 5.1117 | 0.3967 |   42828 B |
| Calculate_IronPython        | Len16_A | Len16_A |         16 | 10,629.35 ns |  69.152 ns |  61.301 ns | 5.1270 | 0.3662 |   43108 B |
| Calculate_IronPython        | Len16_B | Len16_B |         16 | 10,064.47 ns | 107.696 ns | 100.739 ns | 5.0049 | 0.4272 |   42044 B |
| Calculate_IronPython        | Len16_C | Len16_C |         17 | 10,549.71 ns |  84.904 ns |  79.420 ns | 5.1727 | 0.3815 |   43284 B |
| Calculate_IronPython        | Len64_A | Len64_A |         64 | 18,736.42 ns | 125.046 ns | 110.850 ns | 6.8359 | 0.4883 |   57486 B |
| Calculate_IronPython        | Len64_B | Len64_B |         65 | 17,140.62 ns | 130.215 ns | 101.663 ns | 6.4697 | 0.4883 |   54845 B |
| Calculate_IronPython        | Len64_C | Len64_C |         67 | 17,525.94 ns | 106.950 ns | 100.041 ns | 6.5918 | 0.4883 |   55197 B |
| Calculate_NCalc             | Len08_A | Len08_A |          8 |    201.49 ns |   2.103 ns |   1.967 ns | 0.1223 |      - |    1024 B |
| Calculate_NCalc             | Len08_B | Len08_B |          9 |    190.76 ns |   0.801 ns |   0.749 ns | 0.1194 |      - |    1000 B |
| Calculate_NCalc             | Len08_C | Len08_C |          9 |    189.57 ns |   0.825 ns |   0.731 ns | 0.1194 |      - |    1000 B |
| Calculate_NCalc             | Len16_A | Len16_A |         16 |    270.29 ns |   1.379 ns |   1.290 ns | 0.1545 |      - |    1296 B |
| Calculate_NCalc             | Len16_B | Len16_B |         16 |    260.37 ns |   1.138 ns |   1.065 ns | 0.1516 |      - |    1272 B |
| Calculate_NCalc             | Len16_C | Len16_C |         17 |    245.89 ns |   0.841 ns |   0.746 ns | 0.1488 |      - |    1248 B |
| Calculate_NCalc             | Len64_A | Len64_A |         64 |  1,286.80 ns |   4.075 ns |   3.612 ns | 0.6371 | 0.0019 |    5344 B |
| Calculate_NCalc             | Len64_B | Len64_B |         65 |  1,137.75 ns |   1.263 ns |   1.120 ns | 0.5226 | 0.0019 |    4384 B |
| Calculate_NCalc             | Len64_C | Len64_C |         67 |  1,095.91 ns |   1.877 ns |   1.756 ns | 0.5589 | 0.0019 |    4680 B |
| Calculate_xFunc             | Len08_A | Len08_A |          8 |    341.39 ns |   1.310 ns |   1.226 ns | 0.0420 |      - |     352 B |
| Calculate_xFunc             | Len08_B | Len08_B |          9 |    341.81 ns |   1.557 ns |   1.456 ns | 0.0420 |      - |     352 B |
| Calculate_xFunc             | Len08_C | Len08_C |          9 |    342.83 ns |   0.801 ns |   0.710 ns | 0.0420 |      - |     352 B |
| Calculate_xFunc             | Len16_A | Len16_A |         16 |    568.09 ns |   2.037 ns |   1.805 ns | 0.0572 |      - |     480 B |
| Calculate_xFunc             | Len16_B | Len16_B |         16 |    575.56 ns |   3.808 ns |   3.375 ns | 0.0572 |      - |     480 B |
| Calculate_xFunc             | Len16_C | Len16_C |         17 |    558.64 ns |   1.087 ns |   0.908 ns | 0.0572 |      - |     480 B |
| Calculate_xFunc             | Len64_A | Len64_A |         64 |  2,932.22 ns |   6.443 ns |   6.027 ns | 0.2708 |      - |    2272 B |
| Calculate_xFunc             | Len64_B | Len64_B |         65 |  2,509.77 ns |   5.845 ns |   4.881 ns | 0.2251 |      - |    1888 B |
| Calculate_xFunc             | Len64_C | Len64_C |         67 |  2,653.66 ns |   8.056 ns |   7.535 ns | 0.2403 |      - |    2016 B |
| Calculate_ExprTk            | Len08_A | Len08_A |          8 | 41,457.57 ns | 584.448 ns | 546.693 ns |      - |      - |         - |
| Calculate_ExprTk            | Len08_B | Len08_B |          9 | 42,847.31 ns | 833.751 ns | 779.891 ns |      - |      - |         - |
| Calculate_ExprTk            | Len08_C | Len08_C |          9 | 45,267.98 ns | 191.078 ns | 178.734 ns |      - |      - |         - |
| Calculate_ExprTk            | Len16_A | Len16_A |         16 | 42,323.03 ns | 773.344 ns | 723.386 ns |      - |      - |         - |
| Calculate_ExprTk            | Len16_B | Len16_B |         16 | 45,434.93 ns | 274.418 ns | 243.264 ns |      - |      - |         - |
| Calculate_ExprTk            | Len16_C | Len16_C |         17 | 49,734.07 ns | 202.675 ns | 179.666 ns |      - |      - |         - |
| Calculate_ExprTk            | Len64_A | Len64_A |         64 | 69,381.37 ns | 478.763 ns | 447.835 ns |      - |      - |         - |
| Calculate_ExprTk            | Len64_B | Len64_B |         65 | 64,574.17 ns | 786.426 ns | 656.701 ns |      - |      - |         - |
| Calculate_ExprTk            | Len64_C | Len64_C |         67 | 66,630.75 ns | 823.367 ns | 770.178 ns |      - |      - |         - |
