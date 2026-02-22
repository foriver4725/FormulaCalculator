// @formatter:off
using System;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace foriver4725.FormulaCalculator.Tests
{
    public static class Tests
    {
        public static class Tests_IsValid_IsWholeOK
        {
            [Test] public static void Case1() => "".EqN();
            [Test] public static void Case2() => " ".EqN();
            [Test] public static void Case3() => "1+2a".EqN();
            [Test] public static void Case4() => "1+2_3".EqN();
            [Test] public static void Case5() => "1+2.3".EqN();
            [Test] public static void Case6() => "1+2,3".EqN();
            [Test] public static void Case7() => "1+2?3".EqN();
            [Test] public static void Case8() => "1+2*3".NeqN();
        }

        public static class Tests_IsValid_IsNumberOK
        {
            [Test] public static void Case1() => "2(3+4)".EqN();
            [Test] public static void Case2() => "(1+2)(3+4)".EqN();
            [Test] public static void Case3() => "1 23".NeqN();
            [Test] public static void Case4() => "+12-21".NeqN();
            [Test] public static void Case5() => "-123 4".NeqN();
            [Test] public static void Case6() => "12345678".NeqN();
            [Test] public static void Case7() => "123456789".EqN();
        }

        public static class Tests_IsValid_IsOperatorOK
        {
            [Test] public static void Case1() => "+".EqN();
            [Test] public static void Case2() => "-".EqN();
            [Test] public static void Case3() => "*".EqN();
            [Test] public static void Case4() => "/".EqN();
            [Test] public static void Case5() => "1+".EqN();
            [Test] public static void Case6() => "1-".EqN();
            [Test] public static void Case7() => "1*".EqN();
            [Test] public static void Case8() => "1/".EqN();
            [Test] public static void Case9() => "+1".NeqN();
            [Test] public static void Case10() => "-1".NeqN();
            [Test] public static void Case11() => "*1".EqN();
            [Test] public static void Case12() => "/1".EqN();
            [Test] public static void Case13() => "1+2".NeqN();
            [Test] public static void Case14() => "1-2".NeqN();
            [Test] public static void Case15() => "1*2".NeqN();
            [Test] public static void Case16() => "1/2".NeqN();
            [Test] public static void Case17() => "1++2".EqN();
            [Test] public static void Case18() => "1+*2".EqN();
        }

        public static class Tests_IsValid_IsParagraphOK
        {
            [Test] public static void Case1() => ")(".EqN();
            [Test] public static void Case2() => "(()".EqN();
            [Test] public static void Case3() => "())".EqN();
            [Test] public static void Case4() => "((()))())".EqN();
            [Test] public static void Case5() => "((()))(()".EqN();
            [Test] public static void Case6() => "()".EqN();
            [Test] public static void Case7() => "( )".EqN();
            [Test] public static void Case8() => "(+ )".EqN();
            [Test] public static void Case9() => "(- )".EqN();
            [Test] public static void Case10() => "(* )".EqN();
            [Test] public static void Case11() => "(/ )".EqN();
            [Test] public static void Case12() => "(+)".EqN();
            [Test] public static void Case13() => "(-)".EqN();
            [Test] public static void Case14() => "(*)".EqN();
            [Test] public static void Case15() => "(/)".EqN();
            [Test] public static void Case16() => "(+3-4*)".EqN();
            [Test] public static void Case17() => "(-3+4/)".EqN();
            [Test] public static void Case18() => "(*3-4+)".EqN();
            [Test] public static void Case19() => "(/3+4-)".EqN();
            [Test] public static void Case20() => "(3)".NeqN();
            [Test] public static void Case21() => "((1+2))".NeqN();
            [Test] public static void Case22() => "((2+3)*4)".NeqN();
            [Test] public static void Case23() => "((2+3)*(4-1))".NeqN();
        }

        public static class Tests_Calculate
        {
            [Test] public static void Case1() => "1+2*3".Eq(7.0);
            [Test] public static void Case2() => "(1+2)*3".Eq(9.0);
            [Test] public static void Case3() => "1+2* 3-4/5".Eq(6.2);
            [Test] public static void Case4() => "( +(  1+2) *3-4) /5".Eq(1.0);
            [Test] public static void Case5() => "1+2*3-4/5+(6-7*8+9)/10".Eq(2.1);
            [Test] public static void Case6() => "1/0".EqN();
            [Test] public static void Case7() => "1/(2-2)".EqN();
            [Test] public static void Case8() => "1+2*3-4/5+(6-7*8+9)/0".EqN();
            [Test] public static void Case9() => "9999*9999*9999".Eq(short.MaxValue);
            [Test] public static void Case10() => "-9999*9999*9999".Eq(short.MinValue);
            [Test] public static void Case11() => "((((((((((1+2))))))))))".Eq(3.0);
            [Test] public static void Case12() => "((((((((((1+2)*3-4/5+(6-7*8+9)/10)))))))))".Eq(4.1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Eq(this string formula, double expected) => Assert.That(formula.ValueEquals(expected));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Neq(this string formula, double expected) => Assert.That(formula.ValueNotEquals(expected));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EqN(this string formula) => formula.Eq(double.NaN);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void NeqN(this string formula) => formula.Neq(double.NaN);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ValueEquals(this string formula, double expected)
        {
            double result = formula.AsSpan().Calculate();

            if (double.IsNaN(expected))
                return double.IsNaN(result);
            else
                return AlmostEquals(result, expected);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ValueNotEquals(this string formula, double expected)
            => !formula.ValueEquals(expected);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool AlmostEquals(this double a, double b, double epsilon = 1e-4)
            => Math.Abs(a - b) < epsilon;
    }
}
// @formatter:on