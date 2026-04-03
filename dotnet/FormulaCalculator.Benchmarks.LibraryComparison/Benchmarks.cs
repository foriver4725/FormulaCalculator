using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Order;
using ClosedXML.Excel;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using NCalc;
using System.Data;
using xFunc.Maths;

namespace foriver4725.FormulaCalculator.Benchmarks.LibraryComparison;

// for ExprTk
internal static class Native_ExprTk
{
    [DllImport("FormulaCalculator_ExprTk", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe double Calculate_ExprTk(byte* expression, int length);

    public static unsafe double Calculate(string expression)
    {
        ReadOnlySpan<char> formulaSpan = expression.AsSpan();
        int length = formulaSpan.Length;

        Span<byte> buffer = stackalloc byte[length];
        for (int i = 0; i < length; i++)
        {
            buffer[i] = (byte)formulaSpan[i];
        }

        fixed (byte* p = buffer)
        {
            return Calculate_ExprTk(p, length);
        }
    }
}

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
        // Not use '%', '^' and white space.

        // =========================
        // Len08 (≈8)
        // =========================
        yield return new BenchmarkInput("Len08_A", "12.5*3+7");
        yield return new BenchmarkInput("Len08_B", "3.2+8.1*2");
        yield return new BenchmarkInput("Len08_C", "9.4*2-6.3");

        // =========================
        // Len16 (≈16)
        // =========================
        yield return new BenchmarkInput("Len16_A", "(12.578+3.2)*4-1");
        yield return new BenchmarkInput("Len16_B", "(3.25*8.1+7.6)/2");
        yield return new BenchmarkInput("Len16_C", "5.3*(9.2+4.8)-6.1");

        // =========================
        // Len64 (≈64)
        // =========================
        yield return new BenchmarkInput(
            "Len64_A",
            "((37.68/3)*2.3+(80/4-9.7)*6-7)*(3-2.2)+(5*(90-3/15)/0.24)-3*21-3"
        );

        yield return new BenchmarkInput(
            "Len64_B",
            "((12.45*3.6+7.8)/2.1+(44.2/5-3.7)*8)*(6.3-2.4)+(9*(81-4/12)/0.33)"
        );

        yield return new BenchmarkInput(
            "Len64_C",
            "((28.6/2.2)*3.7+(90/6-8.4)*5.3)*(4.1-1.9)+(6*(72-5/18)/0.27)-2.5*13"
        );
    }

    // for ClosedXml
    private readonly IXLCell _cell;

    // for DataTable
    private readonly DataTable _table = new();

    // for IronPython
    private readonly ScriptEngine _engine = Python.CreateEngine();

    // for xFunc
    private readonly Processor _processor = new();

    public Benchmarks()
    {
        // for ClosedXml
        var book = new XLWorkbook();
        book.AddWorksheet();
        var sheet = book.Worksheets.Worksheet(1);
        _cell = sheet.Cell("A1");
    }

    [Benchmark]
    public double Calculate_FormulaCalculator()
    {
        // Other libraries seem not to handle errors.
        return Formula!.AsSpan().Calculate();
    }

    [Benchmark]
    public double Calculate_ClosedXml()
    {
        _cell.FormulaA1 = Formula!;
        return _cell.Value.GetNumber();
    }

    [Benchmark]
    public double Calculate_DataTable()
    {
        // Return value may be double, decimal, or other types.
        return (Convert.ToDouble(_table.Compute(Formula!, null), CultureInfo.InvariantCulture));
    }

    [Benchmark]
    public double Calculate_IronPython()
    {
        return (double)_engine.Execute($"eval('{Formula!}')");
    }

    [Benchmark]
    public double Calculate_NCalc()
    {
        return (double)new Expression(Formula!).Evaluate()!;
    }

    [Benchmark]
    public double Calculate_xFunc()
    {
        return _processor.Solve(Formula!).Number.Number;
    }

    [Benchmark]
    public double Calculate_ExprTk()
    {
        return Native_ExprTk.Calculate(Formula!);
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
            "Calculate_FormulaCalculator" => 0,
            "Calculate_ClosedXml"         => 1,
            "Calculate_DataTable"         => 2,
            "Calculate_IronPython"        => 3,
            "Calculate_NCalc"             => 4,
            "Calculate_xFunc"             => 5,
            _                             => 999,
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