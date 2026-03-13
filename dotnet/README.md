## Build Commands

Location: `dotnet/`

Run the following commands from the dotnet directory.

```bash
# Apply version text to the files used in build
# Should be executed before pushing project to remote
make apply-version-text VERSION_TEXT_TO_APPLY=(YOUR_VERSION_TEXT)

# 1. Build the library (Release / netstandard2.1)
# 2. Run all unit tests
# 3. Deploy the built DLL and test scripts to the Unity project
#    (It copies those files to the Unity project in this repository)
make all

# Run performance benchmarks
# NOTE: It takes certain time
make benchmark-run

# Should be executed after running benchmarks
# Generate a markdown report for the benchmarks
# It generates a graph, then copies those results to be show in the root README.md
make benchmark-deploy-readme

# Push to NuGet
make push-nuget NUGET_PUSH_PACKAGE_VERSION=(YOUR_PACKAGE_VERSION) NUGET_PUSH_API_KEY=(YOUR_API_KEY)
```
