using System;
using System.Runtime.CompilerServices;

namespace foriver4725.FormulaCalculator
{
    public static class FormulaCalculatorWrapper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Calculate(this ReadOnlySpan<char> formula)
            => FormulaCalculator.Calculate(formula);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidFormula(this ReadOnlySpan<char> formula)
            => FormulaValidator.IsValidFormula(formula);
    }
}