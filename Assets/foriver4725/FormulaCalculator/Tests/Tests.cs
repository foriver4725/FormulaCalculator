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
            [Test] public static void Case1() => "".Nv();
            [Test] public static void Case2() => " ".Nv();
            [Test] public static void Case3() => "1+2a".Nv();
            [Test] public static void Case4() => "1+2_3".Nv();
            [Test] public static void Case5() => "1+2.3".Nv();
            [Test] public static void Case6() => "1+2,3".Nv();
            [Test] public static void Case7() => "1+2?3".Nv();
            [Test] public static void Case8() => "1+2*3".V();
        }

        public static class Tests_IsValid_IsNumberOK
        {
            [Test] public static void Case1() => "2(3+4)".Nv();
            [Test] public static void Case2() => "(1+2)(3+4)".Nv();
            [Test] public static void Case3() => "1 23".V();
            [Test] public static void Case4() => "+12-21".V();
            [Test] public static void Case5() => "-123 4".V();
            [Test] public static void Case6() => "12345678".V();
            [Test] public static void Case7() => "123456789".Nv();
        }

        public static class Tests_IsValid_IsOperatorOK
        {
            [Test] public static void Case1() => "+".Nv();
            [Test] public static void Case2() => "-".Nv();
            [Test] public static void Case3() => "*".Nv();
            [Test] public static void Case4() => "/".Nv();
            [Test] public static void Case5() => "1+".Nv();
            [Test] public static void Case6() => "1-".Nv();
            [Test] public static void Case7() => "1*".Nv();
            [Test] public static void Case8() => "1/".Nv();
            [Test] public static void Case9() => "+1".V();
            [Test] public static void Case10() => "-1".V();
            [Test] public static void Case11() => "*1".Nv();
            [Test] public static void Case12() => "/1".Nv();
            [Test] public static void Case13() => "1+2".V();
            [Test] public static void Case14() => "1-2".V();
            [Test] public static void Case15() => "1*2".V();
            [Test] public static void Case16() => "1/2".V();
            [Test] public static void Case17() => "1++2".Nv();
            [Test] public static void Case18() => "1+*2".Nv();
        }

        public static class Tests_IsValid_IsParagraphOK
        {
            [Test] public static void Case1() => ")(".Nv();
            [Test] public static void Case2() => "(()".Nv();
            [Test] public static void Case3() => "())".Nv();
            [Test] public static void Case4() => "((()))())".Nv();
            [Test] public static void Case5() => "((()))(()".Nv();
            [Test] public static void Case6() => "()".Nv();
            [Test] public static void Case7() => "( )".Nv();
            [Test] public static void Case8() => "(+ )".Nv();
            [Test] public static void Case9() => "(- )".Nv();
            [Test] public static void Case10() => "(* )".Nv();
            [Test] public static void Case11() => "(/ )".Nv();
            [Test] public static void Case12() => "(+)".Nv();
            [Test] public static void Case13() => "(-)".Nv();
            [Test] public static void Case14() => "(*)".Nv();
            [Test] public static void Case15() => "(/)".Nv();
            [Test] public static void Case16() => "(+3-4*)".Nv();
            [Test] public static void Case17() => "(-3+4/)".Nv();
            [Test] public static void Case18() => "(*3-4+)".Nv();
            [Test] public static void Case19() => "(/3+4-)".Nv();
            [Test] public static void Case20() => "(3)".V();
            [Test] public static void Case21() => "((1+2))".V();
            [Test] public static void Case22() => "((2+3)*4)".V();
            [Test] public static void Case23() => "((2+3)*(4-1))".V();
        }

        public static class Tests_Calculate
        {
            [Test] public static void Case1() => "1+2*3".Eq(7.0);
            [Test] public static void Case2() => "(1+2)*3".Eq(9.0);
            [Test] public static void Case3() => "1+2* 3-4/5".Eq(6.2);
            [Test] public static void Case4() => "( +(  1+2) *3-4) /5".Eq(1.0);
            [Test] public static void Case5() => "1+2*3-4/5+(6-7*8+9)/10".Eq(2.1);
            [Test] public static void Case6() => "1/0".Eq(double.NaN);
            [Test] public static void Case7() => "1/(2-2)".Eq(double.NaN);
            [Test] public static void Case8() => "1+2*3-4/5+(6-7*8+9)/0".Eq(double.NaN);
            [Test] public static void Case9() => "9999*9999*9999".Eq(9999.0 * 9999.0 * 9999.0);
            [Test] public static void Case10() => "-9999*9999*9999".Eq(-9999.0 * 9999.0 * 9999.0);
            [Test] public static void Case11() => "((((((((((1+2))))))))))".Eq(3.0);
            [Test] public static void Case12() => "((((((((((1+2)*3-4/5+(6-7*8+9)/10)))))))))".Eq(4.1);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void V(this string formula)
            => Assert.IsTrue(formula.AsSpan().IsValidFormula());
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Nv(this string formula)
            => Assert.IsFalse(formula.AsSpan().IsValidFormula());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Eq(this string formula, double expected)
            => Assert.AreEqual(formula.AsSpan().Calculate(), expected, 1.0e-8);
    }
}
// @formatter:on