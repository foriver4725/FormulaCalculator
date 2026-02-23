## Build Commands

### Build the project
```bash
dotnet clean -c Release
dotnet build -c Release
```

### Run the tests
```bash
dotnet test -c Release -v normal
```

### Run the benchmarks
```bash
dotnet run -c Release --project FormulaCalculator.Benchmarks
```
Output : `dotnet/FormulaCalculator.Benchmarks/BenchmarkDotNet.Artifacts/`
