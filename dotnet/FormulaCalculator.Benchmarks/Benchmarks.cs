using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using foriver4725.FormulaCalculator;

namespace foriver4725.FormulaCalculator.Benchmarks;

[MemoryDiagnoser]
public class CalcBench
{
    // Initial value
    private const string FormulaText = "1+2^(7-3)*3/(4-5)";

    // LoopAmount can be switched with Params
    [Params(1_000, 100_000, 10_000_000)]
    public int LoopAmount;

    [Benchmark]
    public double Calculate_Loop()
    {
        double last = 0;

        for (int i = 0; i < LoopAmount; i++)
            last = FormulaText.AsSpan().Calculate();

        return last;
    }
}

public static class Program
{
    public static void Main(string[] args)
        => BenchmarkRunner.Run<CalcBench>();
}