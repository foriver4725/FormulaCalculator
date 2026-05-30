```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.5 (25F71) [Darwin 25.5.0]
Apple M3, 1 CPU, 8 logical and 8 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a


```
| Method                      | Input   | Case    | Char Count | Mean         | Error      | StdDev     | Median       | Gen0   | Gen1   | Gen2   | Allocated |
|---------------------------- |-------- |-------- |-----------:|-------------:|-----------:|-----------:|-------------:|-------:|-------:|-------:|----------:|
| Calculate_FormulaCalculator | Len08_A | Len08_A |          8 |     17.62 ns |   0.197 ns |   0.164 ns |     17.62 ns |      - |      - |      - |         - |
| Calculate_FormulaCalculator | Len08_B | Len08_B |          9 |     19.29 ns |   0.344 ns |   0.269 ns |     19.26 ns |      - |      - |      - |         - |
| Calculate_FormulaCalculator | Len08_C | Len08_C |          9 |     18.02 ns |   0.190 ns |   0.158 ns |     18.03 ns |      - |      - |      - |         - |
| Calculate_FormulaCalculator | Len16_A | Len16_A |         16 |     32.44 ns |   0.341 ns |   0.319 ns |     32.50 ns |      - |      - |      - |         - |
| Calculate_FormulaCalculator | Len16_B | Len16_B |         16 |     32.10 ns |   0.232 ns |   0.206 ns |     32.07 ns |      - |      - |      - |         - |
| Calculate_FormulaCalculator | Len16_C | Len16_C |         17 |     28.95 ns |   0.196 ns |   0.184 ns |     28.99 ns |      - |      - |      - |         - |
| Calculate_FormulaCalculator | Len64_A | Len64_A |         64 |    175.10 ns |   3.243 ns |   5.594 ns |    172.80 ns |      - |      - |      - |         - |
| Calculate_FormulaCalculator | Len64_B | Len64_B |         65 |    150.10 ns |   2.902 ns |   4.518 ns |    149.80 ns |      - |      - |      - |         - |
| Calculate_FormulaCalculator | Len64_C | Len64_C |         67 |    151.32 ns |   2.041 ns |   1.809 ns |    151.20 ns |      - |      - |      - |         - |
| Calculate_ClosedXml         | Len08_A | Len08_A |          8 |    834.98 ns |   5.720 ns |   5.071 ns |    835.87 ns | 0.2050 |      - |      - |    1720 B |
| Calculate_ClosedXml         | Len08_B | Len08_B |          9 |    860.63 ns |   6.193 ns |   5.490 ns |    859.76 ns | 0.2050 |      - |      - |    1720 B |
| Calculate_ClosedXml         | Len08_C | Len08_C |          9 |    863.51 ns |   5.640 ns |   4.710 ns |    864.65 ns | 0.2050 |      - |      - |    1720 B |
| Calculate_ClosedXml         | Len16_A | Len16_A |         16 |  1,116.80 ns |   4.443 ns |   3.938 ns |  1,116.93 ns | 0.2403 |      - |      - |    2016 B |
| Calculate_ClosedXml         | Len16_B | Len16_B |         16 |  1,148.55 ns |  22.826 ns |  23.441 ns |  1,147.54 ns | 0.2403 |      - |      - |    2016 B |
| Calculate_ClosedXml         | Len16_C | Len16_C |         17 |  1,134.37 ns |   7.150 ns |   5.970 ns |  1,133.44 ns | 0.2403 |      - |      - |    2016 B |
| Calculate_ClosedXml         | Len64_A | Len64_A |         64 |  4,121.01 ns |  21.952 ns |  20.534 ns |  4,122.80 ns | 0.5112 |      - |      - |    4336 B |
| Calculate_ClosedXml         | Len64_B | Len64_B |         65 |  3,495.64 ns |  17.729 ns |  14.804 ns |  3,495.31 ns | 0.4883 |      - |      - |    4096 B |
| Calculate_ClosedXml         | Len64_C | Len64_C |         67 |  3,650.43 ns |  19.874 ns |  16.596 ns |  3,646.53 ns | 0.4959 |      - |      - |    4176 B |
| Calculate_DataTable         | Len08_A | Len08_A |          8 |    396.30 ns |   2.948 ns |   2.758 ns |    395.71 ns | 0.3128 | 0.0010 |      - |    2624 B |
| Calculate_DataTable         | Len08_B | Len08_B |          9 |    408.33 ns |   7.857 ns |   7.350 ns |    413.13 ns | 0.3152 | 0.0010 |      - |    2640 B |
| Calculate_DataTable         | Len08_C | Len08_C |          9 |    405.52 ns |   6.647 ns |   5.892 ns |    407.00 ns | 0.3152 | 0.0005 |      - |    2640 B |
| Calculate_DataTable         | Len16_A | Len16_A |         16 |    579.38 ns |   4.139 ns |   3.871 ns |    580.33 ns | 0.3548 |      - |      - |    2992 B |
| Calculate_DataTable         | Len16_B | Len16_B |         16 |    639.61 ns |  12.176 ns |  13.028 ns |    636.84 ns | 0.3586 |      - |      - |    3000 B |
| Calculate_DataTable         | Len16_C | Len16_C |         17 |    634.17 ns |   3.639 ns |   3.404 ns |    634.30 ns | 0.3605 | 0.0029 | 0.0010 |    3016 B |
| Calculate_DataTable         | Len64_A | Len64_A |         64 |  2,505.90 ns |  10.084 ns |   9.433 ns |  2,506.21 ns | 0.7935 |      - |      - |    6704 B |
| Calculate_DataTable         | Len64_B | Len64_B |         65 |  2,302.53 ns |  24.041 ns |  21.311 ns |  2,295.68 ns | 0.7324 |      - |      - |    6152 B |
| Calculate_DataTable         | Len64_C | Len64_C |         67 |  2,415.20 ns |  47.837 ns |  75.875 ns |  2,390.75 ns | 0.7477 |      - |      - |    6344 B |
| Calculate_IronPython        | Len08_A | Len08_A |          8 | 10,341.19 ns | 199.222 ns | 514.256 ns | 10,160.29 ns | 5.1117 | 0.3815 |      - |   42859 B |
| Calculate_IronPython        | Len08_B | Len08_B |          9 | 10,252.75 ns | 202.175 ns | 399.073 ns | 10,119.56 ns | 5.1117 |      - |      - |   42836 B |
| Calculate_IronPython        | Len08_C | Len08_C |          9 | 10,534.07 ns | 206.232 ns | 571.468 ns | 10,363.84 ns | 5.1117 | 0.3967 |      - |   42828 B |
| Calculate_IronPython        | Len16_A | Len16_A |         16 | 11,448.83 ns | 228.319 ns | 375.135 ns | 11,505.00 ns | 5.1270 | 0.3662 |      - |   43108 B |
| Calculate_IronPython        | Len16_B | Len16_B |         16 | 10,489.40 ns | 208.629 ns | 426.174 ns | 10,320.22 ns | 5.0049 | 0.3662 |      - |   42044 B |
| Calculate_IronPython        | Len16_C | Len16_C |         17 | 10,542.27 ns |  92.446 ns |  81.951 ns | 10,560.20 ns | 5.1270 | 0.3662 |      - |   43284 B |
| Calculate_IronPython        | Len64_A | Len64_A |         64 | 18,833.95 ns | 301.469 ns | 451.225 ns | 18,737.87 ns | 6.8359 | 0.4883 |      - |   57486 B |
| Calculate_IronPython        | Len64_B | Len64_B |         65 | 16,587.76 ns | 220.517 ns | 195.482 ns | 16,587.53 ns | 6.5308 | 0.4883 |      - |   54845 B |
| Calculate_IronPython        | Len64_C | Len64_C |         67 | 17,000.88 ns | 138.847 ns | 115.943 ns | 17,009.05 ns | 6.5918 | 0.4883 |      - |   55197 B |
| Calculate_NCalc             | Len08_A | Len08_A |          8 |    200.94 ns |   3.913 ns |   5.485 ns |    198.64 ns | 0.1223 |      - |      - |    1024 B |
| Calculate_NCalc             | Len08_B | Len08_B |          9 |    193.52 ns |   0.685 ns |   0.572 ns |    193.41 ns | 0.1194 |      - |      - |    1000 B |
| Calculate_NCalc             | Len08_C | Len08_C |          9 |    194.62 ns |   3.778 ns |   4.199 ns |    193.28 ns | 0.1194 |      - |      - |    1000 B |
| Calculate_NCalc             | Len16_A | Len16_A |         16 |    299.58 ns |   5.858 ns |   7.409 ns |    298.66 ns | 0.1545 |      - |      - |    1296 B |
| Calculate_NCalc             | Len16_B | Len16_B |         16 |    270.12 ns |   5.357 ns |   7.509 ns |    268.33 ns | 0.1516 |      - |      - |    1272 B |
| Calculate_NCalc             | Len16_C | Len16_C |         17 |    262.62 ns |   5.033 ns |   4.203 ns |    263.09 ns | 0.1488 |      - |      - |    1248 B |
| Calculate_NCalc             | Len64_A | Len64_A |         64 |  1,311.52 ns |  25.096 ns |  24.648 ns |  1,298.38 ns | 0.6371 | 0.0019 |      - |    5344 B |
| Calculate_NCalc             | Len64_B | Len64_B |         65 |  1,050.06 ns |  18.754 ns |  16.625 ns |  1,042.97 ns | 0.5226 | 0.0019 |      - |    4384 B |
| Calculate_NCalc             | Len64_C | Len64_C |         67 |  1,134.95 ns |  19.046 ns |  17.816 ns |  1,132.92 ns | 0.5589 | 0.0019 |      - |    4680 B |
| Calculate_xFunc             | Len08_A | Len08_A |          8 |    350.66 ns |   6.628 ns |   6.200 ns |    347.36 ns | 0.0420 |      - |      - |     352 B |
| Calculate_xFunc             | Len08_B | Len08_B |          9 |    350.91 ns |   6.735 ns |   7.486 ns |    349.13 ns | 0.0420 |      - |      - |     352 B |
| Calculate_xFunc             | Len08_C | Len08_C |          9 |    352.38 ns |   5.982 ns |   5.303 ns |    352.56 ns | 0.0420 |      - |      - |     352 B |
| Calculate_xFunc             | Len16_A | Len16_A |         16 |    590.97 ns |  11.341 ns |  11.139 ns |    593.99 ns | 0.0572 |      - |      - |     480 B |
| Calculate_xFunc             | Len16_B | Len16_B |         16 |    568.56 ns |   5.382 ns |   4.494 ns |    568.31 ns | 0.0572 |      - |      - |     480 B |
| Calculate_xFunc             | Len16_C | Len16_C |         17 |    573.69 ns |  10.313 ns |   8.051 ns |    577.00 ns | 0.0572 |      - |      - |     480 B |
| Calculate_xFunc             | Len64_A | Len64_A |         64 |  3,029.21 ns |  44.387 ns |  37.065 ns |  3,040.08 ns | 0.2708 |      - |      - |    2272 B |
| Calculate_xFunc             | Len64_B | Len64_B |         65 |  2,542.58 ns |  50.810 ns |  49.902 ns |  2,526.11 ns | 0.2251 |      - |      - |    1888 B |
| Calculate_xFunc             | Len64_C | Len64_C |         67 |  2,701.08 ns |  49.944 ns |  44.274 ns |  2,694.07 ns | 0.2403 |      - |      - |    2016 B |
| Calculate_ExprTk            | Len08_A | Len08_A |          8 | 34,532.84 ns | 545.643 ns | 510.395 ns | 34,723.01 ns |      - |      - |      - |         - |
| Calculate_ExprTk            | Len08_B | Len08_B |          9 | 34,398.14 ns | 640.262 ns | 711.649 ns | 34,315.12 ns |      - |      - |      - |         - |
| Calculate_ExprTk            | Len08_C | Len08_C |          9 | 34,116.07 ns | 552.838 ns | 517.125 ns | 33,923.76 ns |      - |      - |      - |         - |
| Calculate_ExprTk            | Len16_A | Len16_A |         16 | 34,710.02 ns | 213.337 ns | 189.117 ns | 34,782.14 ns |      - |      - |      - |         - |
| Calculate_ExprTk            | Len16_B | Len16_B |         16 | 34,841.45 ns | 528.165 ns | 518.729 ns | 34,610.28 ns |      - |      - |      - |         - |
| Calculate_ExprTk            | Len16_C | Len16_C |         17 | 34,847.98 ns | 334.533 ns | 296.555 ns | 34,813.16 ns |      - |      - |      - |         - |
| Calculate_ExprTk            | Len64_A | Len64_A |         64 | 45,242.95 ns | 205.607 ns | 182.266 ns | 45,311.04 ns |      - |      - |      - |         - |
| Calculate_ExprTk            | Len64_B | Len64_B |         65 | 43,314.55 ns | 309.746 ns | 274.582 ns | 43,300.88 ns |      - |      - |      - |         - |
| Calculate_ExprTk            | Len64_C | Len64_C |         67 | 44,266.54 ns | 345.993 ns | 306.714 ns | 44,361.03 ns |      - |      - |      - |         - |
