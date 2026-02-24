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

UNITY_DIR := ../Unity/Assets/foriver4725/FormulaCalculator
UNITY_DLL_DIR := $(UNITY_DIR)/Plugins
UNITY_TEST_SCRIPT_DIR := $(UNITY_DIR)/Tests/Editor
BENCHMARK_MD_PATH := ./BenchmarkResult.md
# ===================

.DEFAULT_GOAL := all

.PHONY: build test deploy-unity all benchmark

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

benchmark:
	@$(LOG_INFO) "Benchmarking started..."
	dotnet run -c $(CONFIG) -f $(OTHER_TFM) --project ./FormulaCalculator.Benchmarks/FormulaCalculator.Benchmarks.csproj
	mkdir -p $(dir $(BENCHMARK_MD_PATH))
	cp -f $(BENCHMARK_RESULT_MD_PATH) $(BENCHMARK_MD_PATH)
	@$(LOG_SUCCESS) "Benchmarking completed"
