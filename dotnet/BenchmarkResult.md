```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.3.1 (25D2128) [Darwin 25.3.0]
Apple M3, 1 CPU, 8 logical and 8 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a


```
| Method                        | Formula              | Char Count | Mean      | Error    | StdDev    | Median    | Allocated |
|------------------------------ |--------------------- |-----------:|----------:|---------:|----------:|----------:|----------:|
| Calculate                     | 2*4-12/3             |          8 |  27.37 ns | 0.405 ns |  0.379 ns |  27.34 ns |         - |
| Calculate                     | 12^(7-3)*3/(4-5)     |         16 |  53.66 ns | 1.082 ns |  1.837 ns |  53.04 ns |         - |
| Calculate                     | ((125(...)21)^3 [64] |         64 | 234.56 ns | 4.729 ns | 11.865 ns | 230.91 ns |         - |
| IsValidFormula                | 2*4-12/3             |          8 |  11.86 ns | 0.264 ns |  0.361 ns |  11.72 ns |         - |
| IsValidFormula                | 12^(7-3)*3/(4-5)     |         16 |  30.51 ns | 0.620 ns |  1.398 ns |  29.79 ns |         - |
| IsValidFormula                | ((125(...)21)^3 [64] |         64 | 119.46 ns | 1.429 ns |  1.194 ns | 119.23 ns |         - |
| Calculate_With_IsValidFormula | 2*4-12/3             |          8 |  39.64 ns | 0.683 ns |  0.605 ns |  39.55 ns |         - |
| Calculate_With_IsValidFormula | 12^(7-3)*3/(4-5)     |         16 |  79.48 ns | 1.455 ns |  2.838 ns |  78.91 ns |         - |
| Calculate_With_IsValidFormula | ((125(...)21)^3 [64] |         64 | 339.23 ns | 6.803 ns | 11.915 ns | 338.08 ns |         - |
