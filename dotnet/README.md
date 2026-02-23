## Build Commands

Location: `dotnet/`

Run the following commands from the dotnet directory.

```bash
# Build the library (Release / netstandard2.1)
make build

# Run all unit tests
# `make build` runs automatically
make test

# Deploy the built DLL and test scripts to the Unity project
# It copies those files to the Unity project in this repository
# `make build` runs automatically
make deploy-unity

# Build, test, and deploy for Unity (all at once)
make all

# Run performance benchmarks
make benchmark
```

## NuGet Push Commands

```bash
dotnet pack -c Release

dotnet nuget push FormulaCalculator/bin/Release/foriver4725.FormulaCalculator.(YOUR_PACKAGE_VERSION).nupkg \
  --api-key (YOUR_API_KEY) \
  --source https://api.nuget.org/v3/index.json
```
