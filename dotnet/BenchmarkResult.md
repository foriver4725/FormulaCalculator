```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.3.1 (25D2128) [Darwin 25.3.0]
Apple M3, 1 CPU, 8 logical and 8 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a


```
| Method                        | Formula              | Char Count | Mean      | Error    | StdDev   | Allocated |
|------------------------------ |--------------------- |-----------:|----------:|---------:|---------:|----------:|
| Calculate                     | 2*4-12/3             |          8 |  28.98 ns | 0.577 ns | 0.641 ns |         - |
| Calculate                     | 12^(7%3)*3/(4-5)     |         16 |  59.81 ns | 1.237 ns | 1.324 ns |         - |
| Calculate                     | ((37%(...)21)^3 [64] |         64 | 225.71 ns | 4.543 ns | 6.515 ns |         - |
| IsValidFormula                | 2*4-12/3             |          8 |  11.61 ns | 0.146 ns | 0.114 ns |         - |
| IsValidFormula                | 12^(7%3)*3/(4-5)     |         16 |  33.54 ns | 0.629 ns | 0.558 ns |         - |
| IsValidFormula                | ((37%(...)21)^3 [64] |         64 | 136.93 ns | 1.398 ns | 1.308 ns |         - |
| Calculate_With_IsValidFormula | 2*4-12/3             |          8 |  37.57 ns | 0.417 ns | 0.390 ns |         - |
| Calculate_With_IsValidFormula | 12^(7%3)*3/(4-5)     |         16 |  86.33 ns | 1.759 ns | 2.407 ns |         - |
| Calculate_With_IsValidFormula | ((37%(...)21)^3 [64] |         64 | 326.46 ns | 3.975 ns | 3.319 ns |         - |
