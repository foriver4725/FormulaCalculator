# ===== Log =====
GREEN  := \033[32m
YELLOW := \033[33m
RED    := \033[31m
RESET  := \033[0m

LOG_INFO    = printf "$(YELLOW)▶ %s$(RESET)\n"
LOG_SUCCESS = printf "$(GREEN)✔ %s$(RESET)\n"
# ===============

# ===== Config =====
CONFIG := Release
DLL_TFM := netstandard2.1
OTHER_TFM := net8.0

DLL_PATH := ./FormulaCalculator/bin/$(CONFIG)/$(DLL_TFM)/FormulaCalculator.dll
TEST_SCRIPT_PATH := ./FormulaCalculator.Tests/Tests.cs
BENCHMARK_RESULT_MD_PATH := ./BenchmarkDotNet.Artifacts/results/foriver4725.FormulaCalculator.Benchmarks.Benchmarks-report-github.md
BENCHMARK_RESULT_CSV_PATH := ./BenchmarkDotNet.Artifacts/results/foriver4725.FormulaCalculator.Benchmarks.Benchmarks-report.csv
BENCHMARK_RESULT_HTML_PATH := ./BenchmarkDotNet.Artifacts/results/foriver4725.FormulaCalculator.Benchmarks.Benchmarks-report.html
BENCHMARK_LIBRARY_COMPARISON_RESULT_MD_PATH := ./BenchmarkDotNet.Artifacts/results/foriver4725.FormulaCalculator.Benchmarks.LibraryComparison.Benchmarks-report-github.md
BENCHMARK_LIBRARY_COMPARISON_RESULT_CSV_PATH := ./BenchmarkDotNet.Artifacts/results/foriver4725.FormulaCalculator.Benchmarks.LibraryComparison.Benchmarks-report.csv
BENCHMARK_LIBRARY_COMPARISON_RESULT_HTML_PATH := ./BenchmarkDotNet.Artifacts/results/foriver4725.FormulaCalculator.Benchmarks.LibraryComparison.Benchmarks-report.html

UNITY_DIR := ../Unity/Assets/foriver4725/FormulaCalculator
UNITY_DLL_DIR := $(UNITY_DIR)/Plugins
UNITY_TEST_SCRIPT_DIR := $(UNITY_DIR)/Tests/Editor
BENCHMARK_MD_PATH := ./BenchmarkResult.md
BENCHMARK_LIBRARY_COMPARISON_MD_PATH := ./BenchmarkLibraryComparisonResult.md

UNITY_PACKAGE_JSON_PATH := $(UNITY_DIR)/package.json
FORMULA_CALCULATOR_CSPROJ_PATH := ./FormulaCalculator/FormulaCalculator.csproj

PYTHON_TOOLS_DIR := ../PythonTools
BENCHMARK_RESULT_GRAPH_MAKER_DIR := $(PYTHON_TOOLS_DIR)/BenchmarkResultGraphMaker
BENCHMARK_RESULT_GRAPH_MAKER_PY_PATH := $(BENCHMARK_RESULT_GRAPH_MAKER_DIR)/benchmark_result_graph_maker.py
BENCHMARK_RESULT_GRAPH_MAKER_OUTPUT_DIR := $(BENCHMARK_RESULT_GRAPH_MAKER_DIR)/output
BENCHMARK_RESULT_GRAPH_MAKER_OUTPUT_NAME := benchmark_result_graph
BENCHMARK_LIBRARY_COMPARISON_RESULT_GRAPH_MAKER_OUTPUT_NAME := benchmark_library_comparison_result_graph
BENCHMARK_RESULT_GRAPH_PATH := ./BenchmarkResultGraph.png
BENCHMARK_LIBRARY_COMPARISON_RESULT_GRAPH_PATH := ./BenchmarkLibraryComparisonResultGraph.png
VERSION_TEXT_APPLIER_DIR := $(PYTHON_TOOLS_DIR)/VersionTextApplier
VERSION_TEXT_APPLIER_UNITY_PY_PATH := $(VERSION_TEXT_APPLIER_DIR)/version_text_applier_unity.py
VERSION_TEXT_APPLIER_DOTNET_PY_PATH := $(VERSION_TEXT_APPLIER_DIR)/version_text_applier_dotnet.py

VERSION_TEXT_TO_APPLY ?=NONE
NUGET_PUSH_PACKAGE_VERSION ?= NONE
NUGET_PUSH_API_KEY ?= NONE
# ===================

.PHONY: \
	apply-version-text \
	build \
	test \
	deploy-unity \
	all \
	benchmark-run \
	benchmark-make-graph-from-result \
	benchmark-deploy-readme \
	benchmark \
	benchmark-library-comparison-run \
	benchmark-library-comparison-make-graph-from-result \
	benchmark-library-comparison-deploy-readme \
	benchmark-library-comparison \
	push-nuget

apply-version-text:
	@$(LOG_INFO) "Applying version text..."
	python3 $(VERSION_TEXT_APPLIER_UNITY_PY_PATH) $(VERSION_TEXT_TO_APPLY) \
		--file $(UNITY_PACKAGE_JSON_PATH)
	python3 $(VERSION_TEXT_APPLIER_DOTNET_PY_PATH) $(VERSION_TEXT_TO_APPLY) \
		--file $(FORMULA_CALCULATOR_CSPROJ_PATH)
	@$(LOG_SUCCESS) "Version text was applied"

build:
	@$(LOG_INFO) "Build started..."
	dotnet build ./FormulaCalculator/FormulaCalculator.csproj -c $(CONFIG) -f $(DLL_TFM)
	@$(LOG_SUCCESS) "Build completed"

test: build
	@$(LOG_INFO) "Testing started..."
	dotnet test ./FormulaCalculator.Tests/FormulaCalculator.Tests.csproj -c $(CONFIG) -f $(OTHER_TFM) -v detailed
	@$(LOG_SUCCESS) "Testing completed"

deploy-unity: build
	@$(LOG_INFO) "Deploying to Unity..."
	test -f $(DLL_PATH)
	test -f $(TEST_SCRIPT_PATH)
	mkdir -p $(UNITY_DLL_DIR)
	mkdir -p $(UNITY_TEST_SCRIPT_DIR)
	cp -f $(DLL_PATH) $(UNITY_DLL_DIR)
	cp -f $(TEST_SCRIPT_PATH) $(UNITY_TEST_SCRIPT_DIR)
	@$(LOG_SUCCESS) "Deployment to Unity completed"

all: build test deploy-unity
	@$(LOG_SUCCESS) "All tasks completed"

benchmark-run:
	@$(LOG_INFO) "Benchmarking started..."
	dotnet run -c $(CONFIG) -f $(OTHER_TFM) \
		--project ./FormulaCalculator.Benchmarks/FormulaCalculator.Benchmarks.csproj
	@$(LOG_SUCCESS) "Benchmarking completed"

benchmark-make-graph-from-result:
	@$(LOG_INFO) "Making benchmark graph from result..."
	python3 $(BENCHMARK_RESULT_GRAPH_MAKER_PY_PATH) $(BENCHMARK_RESULT_CSV_PATH) \
		--output $(BENCHMARK_RESULT_GRAPH_MAKER_OUTPUT_DIR) \
		--name $(BENCHMARK_RESULT_GRAPH_MAKER_OUTPUT_NAME)
	@$(LOG_SUCCESS) "Benchmark graph creation completed"

benchmark-deploy-readme: benchmark-make-graph-from-result
	@$(LOG_INFO) "Deploying benchmark result to README.md..."
	mkdir -p $(dir $(BENCHMARK_MD_PATH))
	cp -f $(BENCHMARK_RESULT_MD_PATH) $(BENCHMARK_MD_PATH)
	@$(LOG_SUCCESS) "Copied result markdown"
	mkdir -p $(dir $(BENCHMARK_RESULT_GRAPH_PATH))
	cp -f $(BENCHMARK_RESULT_GRAPH_MAKER_OUTPUT_DIR)/$(BENCHMARK_RESULT_GRAPH_MAKER_OUTPUT_NAME).png $(BENCHMARK_RESULT_GRAPH_PATH)
	@$(LOG_SUCCESS) "Copied result graph"
	@$(LOG_SUCCESS) "Deployment of benchmark result to README.md completed"

benchmark: benchmark-run benchmark-make-graph-from-result benchmark-deploy-readme
	@$(LOG_SUCCESS) "All benchmark tasks completed"

benchmark-library-comparison-run:
	@$(LOG_INFO) "Benchmarking library comparison started..."
	dotnet run -c $(CONFIG) -f $(OTHER_TFM) \
		--project ./FormulaCalculator.Benchmarks.LibraryComparison/FormulaCalculator.Benchmarks.LibraryComparison.csproj
	@$(LOG_SUCCESS) "Benchmarking library comparison completed"

benchmark-library-comparison-make-graph-from-result:
	@$(LOG_INFO) "Making benchmark library comparison graph from result..."
	python3 $(BENCHMARK_RESULT_GRAPH_MAKER_PY_PATH) $(BENCHMARK_LIBRARY_COMPARISON_RESULT_CSV_PATH) \
		--output $(BENCHMARK_RESULT_GRAPH_MAKER_OUTPUT_DIR) \
		--name $(BENCHMARK_LIBRARY_COMPARISON_RESULT_GRAPH_MAKER_OUTPUT_NAME)
	@$(LOG_SUCCESS) "Benchmark library comparison graph creation completed"

benchmark-library-comparison-deploy-readme: benchmark-library-comparison-make-graph-from-result
	@$(LOG_INFO) "Deploying benchmark library comparison result to README.md..."
	mkdir -p $(dir $(BENCHMARK_LIBRARY_COMPARISON_MD_PATH))
	cp -f $(BENCHMARK_LIBRARY_COMPARISON_RESULT_MD_PATH) $(BENCHMARK_LIBRARY_COMPARISON_MD_PATH)
	@$(LOG_SUCCESS) "Copied library comparison result markdown"
	mkdir -p $(dir $(BENCHMARK_LIBRARY_COMPARISON_RESULT_GRAPH_PATH))
	cp -f $(BENCHMARK_RESULT_GRAPH_MAKER_OUTPUT_DIR)/$(BENCHMARK_LIBRARY_COMPARISON_RESULT_GRAPH_MAKER_OUTPUT_NAME).png $(BENCHMARK_LIBRARY_COMPARISON_RESULT_GRAPH_PATH)
	@$(LOG_SUCCESS) "Copied library comparison result graph"
	@$(LOG_SUCCESS) "Deployment of benchmark library comparison result to README.md completed"

benchmark-library-comparison: benchmark-library-comparison-run benchmark-library-comparison-make-graph-from-result benchmark-library-comparison-deploy-readme
	@$(LOG_SUCCESS) "All benchmark library comparison tasks completed"

push-nuget:
	dotnet pack -c Release
	dotnet nuget push FormulaCalculator/bin/Release/foriver4725.FormulaCalculator.$(NUGET_PUSH_PACKAGE_VERSION).nupkg \
		--api-key $(NUGET_PUSH_API_KEY) \
		--source https://api.nuget.org/v3/index.json
