```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.3.1 (25D2128) [Darwin 25.3.0]
Apple M3, 1 CPU, 8 logical and 8 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a


```
| Method                        | Formula              | Char Count | Mean      | Error    | StdDev   | Allocated |
|------------------------------ |--------------------- |-----------:|----------:|---------:|---------:|----------:|
| Calculate                     | 2*4-12/3             |          8 |  27.47 ns | 0.307 ns | 0.272 ns |         - |
| Calculate                     | 12^(7-3)*3/(4-5)     |         16 |  51.99 ns | 0.370 ns | 0.346 ns |         - |
| Calculate                     | ((125(...)21)^3 [64] |         64 | 222.97 ns | 4.103 ns | 3.838 ns |         - |
| IsValidFormula                | 2*4-12/3             |          8 |  11.46 ns | 0.098 ns | 0.091 ns |         - |
| IsValidFormula                | 12^(7-3)*3/(4-5)     |         16 |  28.10 ns | 0.551 ns | 0.541 ns |         - |
| IsValidFormula                | ((125(...)21)^3 [64] |         64 | 111.32 ns | 1.296 ns | 1.212 ns |         - |
| Calculate_With_IsValidFormula | 2*4-12/3             |          8 |  38.45 ns | 0.180 ns | 0.168 ns |         - |
| Calculate_With_IsValidFormula | 12^(7-3)*3/(4-5)     |         16 |  76.45 ns | 0.560 ns | 0.524 ns |         - |
| Calculate_With_IsValidFormula | ((125(...)21)^3 [64] |         64 | 330.84 ns | 4.688 ns | 4.385 ns |         - |
