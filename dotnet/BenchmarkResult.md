```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.3 (25D125) [Darwin 25.3.0]
Apple M3, 1 CPU, 8 logical and 8 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a


```
| Method                        | Formula              | Char Count | Mean      | Error    | StdDev   | Allocated |
|------------------------------ |--------------------- |-----------:|----------:|---------:|---------:|----------:|
| Calculate                     | 2*4-12/3             |          8 |  27.08 ns | 0.269 ns | 0.225 ns |         - |
| Calculate                     | 12^(7-3)*3/(4-5)     |         16 |  52.49 ns | 0.867 ns | 0.769 ns |         - |
| Calculate                     | ((125(...)21)^3 [64] |         64 | 245.11 ns | 4.525 ns | 4.233 ns |         - |
| IsValidFormula                | 2*4-12/3             |          8 |  34.42 ns | 0.143 ns | 0.127 ns |         - |
| IsValidFormula                | 12^(7-3)*3/(4-5)     |         16 |  77.02 ns | 0.799 ns | 0.708 ns |         - |
| IsValidFormula                | ((125(...)21)^3 [64] |         64 | 312.70 ns | 5.645 ns | 6.040 ns |         - |
| Calculate_With_IsValidFormula | 2*4-12/3             |          8 |  60.47 ns | 0.485 ns | 0.405 ns |         - |
| Calculate_With_IsValidFormula | 12^(7-3)*3/(4-5)     |         16 | 129.29 ns | 0.910 ns | 0.806 ns |         - |
| Calculate_With_IsValidFormula | ((125(...)21)^3 [64] |         64 | 554.09 ns | 5.103 ns | 4.773 ns |         - |
