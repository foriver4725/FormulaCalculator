```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.3.1 (25D2128) [Darwin 25.3.0]
Apple M3, 1 CPU, 8 logical and 8 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a


```
| Method                      | Input   | Case    | Char Count | Mean         | Error      | StdDev     | Gen0   | Gen1   | Allocated |
|---------------------------- |-------- |-------- |-----------:|-------------:|-----------:|-----------:|-------:|-------:|----------:|
| Calculate_FormulaCalculator | Len08_A | Len08_A |          8 |     21.89 ns |   0.458 ns |   0.428 ns |      - |      - |         - |
| Calculate_FormulaCalculator | Len08_B | Len08_B |          9 |     22.51 ns |   0.157 ns |   0.139 ns |      - |      - |         - |
| Calculate_FormulaCalculator | Len08_C | Len08_C |          9 |     21.71 ns |   0.097 ns |   0.081 ns |      - |      - |         - |
| Calculate_FormulaCalculator | Len16_A | Len16_A |         16 |     36.75 ns |   0.148 ns |   0.131 ns |      - |      - |         - |
| Calculate_FormulaCalculator | Len16_B | Len16_B |         16 |     35.43 ns |   0.324 ns |   0.287 ns |      - |      - |         - |
| Calculate_FormulaCalculator | Len16_C | Len16_C |         17 |     34.44 ns |   0.216 ns |   0.202 ns |      - |      - |         - |
| Calculate_FormulaCalculator | Len64_A | Len64_A |         64 |    192.44 ns |   2.133 ns |   1.995 ns |      - |      - |         - |
| Calculate_FormulaCalculator | Len64_B | Len64_B |         65 |    160.79 ns |   2.478 ns |   2.318 ns |      - |      - |         - |
| Calculate_FormulaCalculator | Len64_C | Len64_C |         67 |    176.52 ns |   3.545 ns |   3.641 ns |      - |      - |         - |
| Calculate_ClosedXml         | Len08_A | Len08_A |          8 |    835.21 ns |   3.478 ns |   3.253 ns | 0.2050 |      - |    1720 B |
| Calculate_ClosedXml         | Len08_B | Len08_B |          9 |    855.10 ns |   5.380 ns |   5.032 ns | 0.2050 |      - |    1720 B |
| Calculate_ClosedXml         | Len08_C | Len08_C |          9 |    856.86 ns |   4.320 ns |   3.607 ns | 0.2050 |      - |    1720 B |
| Calculate_ClosedXml         | Len16_A | Len16_A |         16 |  1,123.98 ns |   3.585 ns |   3.353 ns | 0.2403 |      - |    2016 B |
| Calculate_ClosedXml         | Len16_B | Len16_B |         16 |  1,130.33 ns |   3.216 ns |   2.851 ns | 0.2403 |      - |    2016 B |
| Calculate_ClosedXml         | Len16_C | Len16_C |         17 |  1,150.69 ns |   6.532 ns |   6.110 ns | 0.2403 |      - |    2016 B |
| Calculate_ClosedXml         | Len64_A | Len64_A |         64 |  3,889.22 ns |   9.824 ns |   9.190 ns | 0.5112 |      - |    4336 B |
| Calculate_ClosedXml         | Len64_B | Len64_B |         65 |  3,397.87 ns |  14.484 ns |  12.095 ns | 0.4883 |      - |    4096 B |
| Calculate_ClosedXml         | Len64_C | Len64_C |         67 |  3,623.39 ns |  17.475 ns |  16.346 ns | 0.4959 |      - |    4176 B |
| Calculate_DataTable         | Len08_A | Len08_A |          8 |    388.87 ns |   2.332 ns |   2.182 ns | 0.3133 | 0.0005 |    2624 B |
| Calculate_DataTable         | Len08_B | Len08_B |          9 |    414.30 ns |   7.301 ns |   6.829 ns | 0.3152 | 0.0005 |    2640 B |
| Calculate_DataTable         | Len08_C | Len08_C |          9 |    408.05 ns |   3.793 ns |   3.548 ns | 0.3152 |      - |    2640 B |
| Calculate_DataTable         | Len16_A | Len16_A |         16 |    571.54 ns |   2.944 ns |   2.754 ns | 0.3548 |      - |    2992 B |
| Calculate_DataTable         | Len16_B | Len16_B |         16 |    612.57 ns |   4.124 ns |   3.858 ns | 0.3586 |      - |    3000 B |
| Calculate_DataTable         | Len16_C | Len16_C |         17 |    631.32 ns |   2.139 ns |   2.001 ns | 0.3586 |      - |    3016 B |
| Calculate_DataTable         | Len64_A | Len64_A |         64 |  2,452.50 ns |   8.593 ns |   8.038 ns | 0.7935 |      - |    6704 B |
| Calculate_DataTable         | Len64_B | Len64_B |         65 |  2,340.80 ns |  19.408 ns |  18.154 ns | 0.7324 |      - |    6152 B |
| Calculate_DataTable         | Len64_C | Len64_C |         67 |  2,387.95 ns |  13.852 ns |  12.957 ns | 0.7477 |      - |    6344 B |
| Calculate_IronPython        | Len08_A | Len08_A |          8 |  9,918.66 ns |  93.264 ns |  87.239 ns | 5.0964 | 0.4578 |   42683 B |
| Calculate_IronPython        | Len08_B | Len08_B |          9 |  9,980.16 ns |  72.612 ns |  67.922 ns | 5.1117 | 0.3815 |   42836 B |
| Calculate_IronPython        | Len08_C | Len08_C |          9 | 10,466.19 ns |  80.814 ns |  75.594 ns | 5.0659 | 0.3662 |   42828 B |
| Calculate_IronPython        | Len16_A | Len16_A |         16 | 10,657.89 ns |  88.630 ns |  82.905 ns | 5.1270 | 0.3662 |   43108 B |
| Calculate_IronPython        | Len16_B | Len16_B |         16 | 10,015.94 ns | 135.260 ns | 126.522 ns | 5.0049 | 0.4272 |   42044 B |
| Calculate_IronPython        | Len16_C | Len16_C |         17 | 10,564.43 ns |  69.028 ns |  64.569 ns | 5.1270 | 0.3662 |   43284 B |
| Calculate_IronPython        | Len64_A | Len64_A |         64 | 18,642.51 ns | 260.965 ns | 244.107 ns | 6.8359 | 0.4883 |   57486 B |
| Calculate_IronPython        | Len64_B | Len64_B |         65 | 16,765.16 ns | 134.484 ns | 125.796 ns | 6.4697 | 0.4883 |   54845 B |
| Calculate_IronPython        | Len64_C | Len64_C |         67 | 16,992.80 ns | 106.027 ns |  99.178 ns | 6.5918 | 0.4883 |   55197 B |
| Calculate_NCalc             | Len08_A | Len08_A |          8 |    196.32 ns |   1.870 ns |   1.657 ns | 0.1223 |      - |    1024 B |
| Calculate_NCalc             | Len08_B | Len08_B |          9 |    189.36 ns |   0.767 ns |   0.718 ns | 0.1194 |      - |    1000 B |
| Calculate_NCalc             | Len08_C | Len08_C |          9 |    186.45 ns |   2.240 ns |   2.095 ns | 0.1194 |      - |    1000 B |
| Calculate_NCalc             | Len16_A | Len16_A |         16 |    297.12 ns |   1.159 ns |   1.084 ns | 0.1545 |      - |    1296 B |
| Calculate_NCalc             | Len16_B | Len16_B |         16 |    261.86 ns |   0.704 ns |   0.588 ns | 0.1516 |      - |    1272 B |
| Calculate_NCalc             | Len16_C | Len16_C |         17 |    247.65 ns |   0.596 ns |   0.557 ns | 0.1488 |      - |    1248 B |
| Calculate_NCalc             | Len64_A | Len64_A |         64 |  1,389.48 ns |   2.934 ns |   2.450 ns | 0.6371 | 0.0019 |    5344 B |
| Calculate_NCalc             | Len64_B | Len64_B |         65 |  1,133.92 ns |   1.565 ns |   1.464 ns | 0.5226 | 0.0019 |    4384 B |
| Calculate_NCalc             | Len64_C | Len64_C |         67 |  1,080.65 ns |   1.585 ns |   1.405 ns | 0.5589 | 0.0019 |    4680 B |
| Calculate_xFunc             | Len08_A | Len08_A |          8 |    338.06 ns |   0.584 ns |   0.456 ns | 0.0420 |      - |     352 B |
| Calculate_xFunc             | Len08_B | Len08_B |          9 |    342.14 ns |   0.791 ns |   0.740 ns | 0.0420 |      - |     352 B |
| Calculate_xFunc             | Len08_C | Len08_C |          9 |    338.30 ns |   0.715 ns |   0.669 ns | 0.0420 |      - |     352 B |
| Calculate_xFunc             | Len16_A | Len16_A |         16 |    566.78 ns |   3.248 ns |   3.038 ns | 0.0572 |      - |     480 B |
| Calculate_xFunc             | Len16_B | Len16_B |         16 |    556.56 ns |   2.125 ns |   1.884 ns | 0.0572 |      - |     480 B |
| Calculate_xFunc             | Len16_C | Len16_C |         17 |    555.61 ns |   1.131 ns |   1.058 ns | 0.0572 |      - |     480 B |
| Calculate_xFunc             | Len64_A | Len64_A |         64 |  2,943.05 ns |   8.477 ns |   7.929 ns | 0.2708 |      - |    2272 B |
| Calculate_xFunc             | Len64_B | Len64_B |         65 |  2,526.29 ns |   6.291 ns |   5.885 ns | 0.2251 |      - |    1888 B |
| Calculate_xFunc             | Len64_C | Len64_C |         67 |  2,630.50 ns |   5.780 ns |   5.407 ns | 0.2403 |      - |    2016 B |
| Calculate_ExprTk            | Len08_A | Len08_A |          8 | 34,275.61 ns | 203.251 ns | 190.121 ns |      - |      - |         - |
| Calculate_ExprTk            | Len08_B | Len08_B |          9 | 34,451.75 ns | 131.928 ns | 116.951 ns |      - |      - |         - |
| Calculate_ExprTk            | Len08_C | Len08_C |          9 | 34,407.37 ns | 107.142 ns | 100.221 ns |      - |      - |         - |
| Calculate_ExprTk            | Len16_A | Len16_A |         16 | 35,368.64 ns | 139.659 ns | 123.804 ns |      - |      - |         - |
| Calculate_ExprTk            | Len16_B | Len16_B |         16 | 35,488.66 ns | 129.281 ns | 107.955 ns |      - |      - |         - |
| Calculate_ExprTk            | Len16_C | Len16_C |         17 | 35,418.56 ns | 184.912 ns | 163.920 ns |      - |      - |         - |
| Calculate_ExprTk            | Len64_A | Len64_A |         64 | 46,183.22 ns | 204.457 ns | 191.249 ns |      - |      - |         - |
| Calculate_ExprTk            | Len64_B | Len64_B |         65 | 43,953.11 ns | 324.386 ns | 303.431 ns |      - |      - |         - |
| Calculate_ExprTk            | Len64_C | Len64_C |         67 | 44,669.55 ns | 228.474 ns | 213.715 ns |      - |      - |         - |
