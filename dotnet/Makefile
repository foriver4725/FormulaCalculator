# ===== Config =====
CONFIG := Release
TFM := netstandard2.1

DLL_PATH := ./FormulaCalculator/bin/$(CONFIG)/$(TFM)/FormulaCalculator.dll
TEST_SCRIPT_PATH := ./FormulaCalculator.Tests/Tests.cs

UNITY_DIR := ../Unity/Assets/foriver4725/FormulaCalculator
UNITY_DLL_DIR := $(UNITY_DIR)/Plugins
UNITY_TEST_SCRIPT_DIR := $(UNITY_DIR)/Tests/Editor

.PHONY: build test deploy-unity all benchmark

build:
	dotnet build -c $(CONFIG) -f $(TFM)

test: build
	dotnet test ./FormulaCalculator.Tests/FormulaCalculator.Tests.csproj -c $(CONFIG) -v detailed

deploy-unity: build
	test -f $(DLL_PATH) $(TEST_SCRIPT_PATH)
	mkdir -p $(UNITY_DLL_DIR) $(UNITY_TEST_SCRIPT_DIR)
	cp -f $(DLL_PATH) $(UNITY_DLL_DIR)/
	cp -f $(TEST_SCRIPT_PATH) $(UNITY_TEST_SCRIPT_DIR)/

all: build test deploy-unity

benchmark:
	dotnet run -c $(CONFIG) --project ./FormulaCalculator.Benchmarks/FormulaCalculator.Benchmarks.csproj