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
    public string? CaseName { get; set; }
    public string? Formula { get; set; }

    [ParamsSource(nameof(GetCases))]
    public BenchmarkInput Input
    {
        set
        {
            CaseName = value.CaseName;
            Formula = value.Formula;
        }
    }

    public static IEnumerable<BenchmarkInput> GetCases()
    {
        // =========================================================
        // 8 chars
        // =========================================================
        yield return new BenchmarkInput("Len08_Decimal_AddMul", "12.5*3+7");
        yield return new BenchmarkInput("Len08_Mod_Pow", "9%4+2^3");
        yield return new BenchmarkInput("Len08_Whitespace", " 1+2* 3 ");

        // =========================================================
        // 16 chars
        // =========================================================
        yield return new BenchmarkInput("Len16_Mod_Decimal", "(12%5+3.2)*4-1");
        yield return new BenchmarkInput("Len16_Pow_Decimal", "2.5^(3-1)+4*2.0");
        yield return new BenchmarkInput("Len16_Whitespace", " ( 8+2 )*3-4/2 ");

        // =========================================================
        // 64 chars
        // =========================================================
        yield return new BenchmarkInput("Len64_Mixed_01",
            "((37%6/3)*2^3+(80/4-9%7)*6-7)*(3-2^2)+(5*(90-3/15)^2%4)-(3*21)^3");
        yield return new BenchmarkInput("Len64_Mixed_02", "((12.5+3)*2^3-(18%5))*((7-2)^2+4)/(3+1)+(9%4)*(6.0-2.5)+8");
        yield return new BenchmarkInput("Len64_Mixed_03", " ( (15%4+2^3)*3 - (8/2) ) + ( 7.5*(4-1) ) - ( 9%5 ) + 12 ");

        // Note:
        // If you want strictly fixed lengths for every long case,
        // adjust the strings after confirming Formula.Length.
        // The main purpose here is to benchmark multiple realistic patterns
        // per size bucket rather than a single formula per size.
    }

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

public readonly record struct BenchmarkInput(string CaseName, string Formula)
{
    public override string ToString() => CaseName;
}

public sealed class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        AddColumn(new CaseNameColumn());
        AddColumn(new FormulaLengthColumn());

        Orderer = new FormulaLengthOrderer();
    }
}

public sealed class CaseNameColumn : IColumn
{
    public string Id => nameof(CaseNameColumn);
    public string ColumnName => "Case";
    public string Legend => "Benchmark case name";
    public bool AlwaysShow => true;

    public ColumnCategory Category => ColumnCategory.Params;

    public int PriorityInCategory => 0;

    public bool IsNumeric => false;
    public UnitType UnitType => UnitType.Dimensionless;

    public bool IsAvailable(Summary summary) => true;
    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
        => GetValue(summary, benchmarkCase, SummaryStyle.Default);

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
    {
        var input = benchmarkCase.Parameters["Input"];
        return input.ToString() ?? "";
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
        if (benchmarkCase.Parameters["Input"] is BenchmarkInput input)
            return input.Formula.Length.ToString(CultureInfo.InvariantCulture);

        var raw = benchmarkCase.Parameters["Input"].ToString();
        return (raw?.Length ?? 0).ToString(CultureInfo.InvariantCulture);
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
            .ThenBy(GetFormulaLength)
            .ThenBy(GetCaseName, StringComparer.Ordinal);

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
        if (b.Parameters["Input"] is BenchmarkInput input)
            return input.Formula.Length;

        var raw = b.Parameters["Input"].ToString();
        return raw?.Length ?? 0;
    }

    private static string GetCaseName(BenchmarkCase b)
    {
        if (b.Parameters["Input"] is BenchmarkInput input)
            return input.CaseName;

        return b.Parameters["Input"].ToString() ?? "";
    }
}

public static class Program
{
    public static void Main(string[] args)
        => BenchmarkRunner.Run<Benchmarks>();
}