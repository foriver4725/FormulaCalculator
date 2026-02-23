## Build Commands

### Location
```
/dotnet/
```

### Build the project
```bash
dotnet clean FormulaCalculator/FormulaCalculator.csproj -c Release
dotnet build FormulaCalculator/FormulaCalculator.csproj -c Release
```

Output : `FormulaCalculator/bin/Release/net8.0/FormulaCalculator.dll`

### Run the tests
```bash
dotnet test FormulaCalculator.Tests/FormulaCalculator.Tests.csproj -c Release -v normal
```

### Run the benchmarks
```bash
dotnet run -c Release --project FormulaCalculator.Benchmarks/FormulaCalculator.Benchmarks.csproj
```

Output : `FormulaCalculator.Benchmarks/BenchmarkDotNet.Artifacts/`
