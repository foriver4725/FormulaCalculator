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
make

# Run performance benchmarks
make benchmark-run

# Generate a markdown report for the benchmarks
# It generates a graph, then copies those results to be show in the root README.md
make benchmark-deploy-readme

# Run all benchmark commands at once
# `make benchmark-run` and `make benchmark-deploy-readme` run automatically
make benchmark

# Push to NuGet
make push-nuget NUGET_PUSH_PACKAGE_VERSION=(YOUR_PACKAGE_VERSION) NUGET_PUSH_API_KEY=(YOUR_API_KEY)
```
