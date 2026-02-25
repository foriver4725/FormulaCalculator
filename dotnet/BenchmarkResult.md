```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.3 (25D125) [Darwin 25.3.0]
Apple M3, 1 CPU, 8 logical and 8 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a


```
| Method                        | Formula              | Char Count | Mean      | Error    | StdDev   | Allocated |
|------------------------------ |--------------------- |-----------:|----------:|---------:|---------:|----------:|
| Calculate                     | 2*4-12/3             |          8 |  26.71 ns | 0.159 ns | 0.141 ns |         - |
| Calculate                     | 12^(7-3)*3/(4-5)     |         16 |  51.32 ns | 0.227 ns | 0.213 ns |         - |
| Calculate                     | ((125(...)21)^3 [64] |         64 | 237.41 ns | 3.340 ns | 3.124 ns |         - |
| IsValidFormula                | 2*4-12/3             |          8 |  34.40 ns | 0.116 ns | 0.090 ns |         - |
| IsValidFormula                | 12^(7-3)*3/(4-5)     |         16 |  76.57 ns | 0.574 ns | 0.537 ns |         - |
| IsValidFormula                | ((125(...)21)^3 [64] |         64 | 305.76 ns | 1.882 ns | 1.761 ns |         - |
| Calculate_With_IsValidFormula | 2*4-12/3             |          8 |  60.29 ns | 0.272 ns | 0.241 ns |         - |
| Calculate_With_IsValidFormula | 12^(7-3)*3/(4-5)     |         16 | 128.91 ns | 0.907 ns | 0.804 ns |         - |
| Calculate_With_IsValidFormula | ((125(...)21)^3 [64] |         64 | 549.80 ns | 3.339 ns | 2.960 ns |         - |
