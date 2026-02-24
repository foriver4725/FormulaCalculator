```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.3 (25D125) [Darwin 25.3.0]
Apple M3, 1 CPU, 8 logical and 8 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 8.0.10 (8.0.10, 8.0.1024.46610), Arm64 RyuJIT armv8.0-a

Method=Run  

```
| Formula              | Char Count | Mean      | Error    | StdDev   | Allocated |
|--------------------- |-----------:|----------:|---------:|---------:|----------:|
| 2*4-12/3             |          8 |  26.62 ns | 0.099 ns | 0.093 ns |         - |
| 12^(7-3)*3/(4-5)     |         16 |  51.67 ns | 0.348 ns | 0.291 ns |         - |
| ((125(...)21)^3 [64] |         64 | 238.86 ns | 3.138 ns | 2.935 ns |         - |
