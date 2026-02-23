using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace foriver4725.FormulaCalculator.Benchmarks;

[MemoryDiagnoser]
public class Benchmarks
{
    [Params(
        "2*4-12/3",
        "1+2^(7-3)*3/(4-5)",
        "((125-35)*2^3+(80/4-125)*6-7)*(3-2^2)+(5*(90-3/15)^2-4)"
    )]
    public string? Formula;

    [Params(
        (int)1e4,
        (int)1e5,
        (int)1e6
    )]
    public int LoopAmount;

    [Benchmark]
    public double Run()
    {
        ReadOnlySpan<char> formulaAsSpan = Formula.AsSpan();
        double result = 0.0;

        for (int i = 0; i < LoopAmount; i++)
            result = formulaAsSpan.Calculate();

        return result;
    }
}

public static class Program
{
    public static void Main(string[] args)
        => BenchmarkRunner.Run<Benchmarks>();
}