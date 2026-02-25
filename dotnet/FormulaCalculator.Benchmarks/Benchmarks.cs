using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Order;

namespace foriver4725.FormulaCalculator.Benchmarks;

[MemoryDiagnoser]
[Config(typeof(BenchmarkConfig))]
public class Benchmarks
{
    [Params(
        "2*4-12/3",
        "12^(7-3)*3/(4-5)",
        "((125-35)*2^3+(80/4-125)*6-7)*(3-2^2)+(5*(90-3/15)^2-4)-(3*21)^3"
    )]
    public string? Formula;

    [Benchmark]
    public double Calculate()
    {
        return Formula!.AsSpan().Calculate();
    }

    [Benchmark]
    public bool IsValidFormula()
    {
        return Formula!.AsSpan().IsValidFormula();
    }

    [Benchmark]
    public double Calculate_With_IsValidFormula()
    {
        var formulaSpan = Formula!.AsSpan();
        return formulaSpan.IsValidFormula() ? formulaSpan.Calculate() : double.NaN;
    }
}

public sealed class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        AddColumn(new FormulaLengthColumn());

        Orderer = new FormulaLengthOrderer();
    }
}

public sealed class FormulaLengthColumn : IColumn
{
    public string Id => nameof(FormulaLengthColumn);
    public string ColumnName => "Char Count";
    public string Legend => "Number of characters in the formula";
    public bool AlwaysShow => true;

    public ColumnCategory Category => ColumnCategory.Params;

    public int PriorityInCategory => 1000;

    public bool IsNumeric => true;
    public UnitType UnitType => UnitType.Dimensionless;

    public bool IsAvailable(Summary summary) => true;
    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
        => GetValue(summary, benchmarkCase, SummaryStyle.Default);

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
    {
        var formula = benchmarkCase.Parameters["Formula"]?.ToString();
        return (formula?.Length ?? 0).ToString(CultureInfo.InvariantCulture);
    }
}

public sealed class FormulaLengthOrderer : IOrderer
{
    public bool SeparateLogicalGroups => false;

    public IEnumerable<BenchmarkCase> GetExecutionOrder(
        ImmutableArray<BenchmarkCase> benchmarks,
        IEnumerable<BenchmarkLogicalGroupRule>? order = null)
        => Order(benchmarks);

    public IEnumerable<BenchmarkCase> GetSummaryOrder(
        ImmutableArray<BenchmarkCase> benchmarks,
        Summary summary)
        => Order(benchmarks);

    public string? GetHighlightGroupKey(BenchmarkCase benchmarkCase) => null;

    public string GetLogicalGroupKey(
        ImmutableArray<BenchmarkCase> benchmarks,
        BenchmarkCase benchmarkCase)
        => "";

    public IEnumerable<IGrouping<string, BenchmarkCase>> GetLogicalGroupOrder(
        IEnumerable<IGrouping<string, BenchmarkCase>> groups,
        IEnumerable<BenchmarkLogicalGroupRule>? order = null)
        => groups;

    private static IEnumerable<BenchmarkCase> Order(IEnumerable<BenchmarkCase> benchmarks)
        => benchmarks
            .OrderBy(GetMethodRank)
            .ThenBy(GetFormulaLength);

    private static int GetMethodRank(BenchmarkCase b)
    {
        string name = b.Descriptor.WorkloadMethod.Name;

        return name switch
        {
            "Calculate"                     => 0,
            "IsValidFormula"                => 1,
            "Calculate_With_IsValidFormula" => 2,
            _                               => 999,
        };
    }

    private static int GetFormulaLength(BenchmarkCase b)
    {
        var formula = b.Parameters["Formula"]?.ToString();
        return formula?.Length ?? 0;
    }
}

public static class Program
{
    public static void Main(string[] args)
        => BenchmarkRunner.Run<Benchmarks>();
}