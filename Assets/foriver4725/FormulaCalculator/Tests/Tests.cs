using NUnit.Framework;

namespace foriver4725.FormulaCalculator.Tests
{
    public static class Tests
    {
        public static class Tests_IsValid_IsWholeOK
        {
            [Test] public static void Case1() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("")));
            [Test] public static void Case2() => Assert.That(double.IsNaN(FormulaCalculator.Calculate(" ")));
            [Test] public static void Case3() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("1+2a")));
            [Test] public static void Case4() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("1+2_3")));
            [Test] public static void Case5() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("1+2.3")));
            [Test] public static void Case6() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("1+2,3")));
            [Test] public static void Case7() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("1+2?3")));
            [Test] public static void Case8() => Assert.That(!double.IsNaN(FormulaCalculator.Calculate("1+2*3")));
        }

        public static class Tests_IsValid_IsNumberOK
        {
            [Test] public static void Case1() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("2(3+4)")));
            [Test] public static void Case2() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("(1+2)(3+4)")));
            [Test] public static void Case3() => Assert.That(!double.IsNaN(FormulaCalculator.Calculate("1 23")));
            [Test] public static void Case4() => Assert.That(!double.IsNaN(FormulaCalculator.Calculate("+12-21")));
            [Test] public static void Case5() => Assert.That(!double.IsNaN(FormulaCalculator.Calculate("-123 4")));
            [Test] public static void Case6() => Assert.That(!double.IsNaN(FormulaCalculator.Calculate("12345678")));
            [Test] public static void Case7() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("123456789")));
        }

        public static class Tests_IsValid_IsOperatorOK
        {
            [Test] public static void Case1() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("+")));
            [Test] public static void Case2() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("-")));
            [Test] public static void Case3() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("*")));
            [Test] public static void Case4() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("/")));
            [Test] public static void Case5() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("1+")));
            [Test] public static void Case6() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("1-")));
            [Test] public static void Case7() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("1*")));
            [Test] public static void Case8() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("1/")));
            [Test] public static void Case9() => Assert.That(!double.IsNaN(FormulaCalculator.Calculate("+1")));
            [Test] public static void Case10() => Assert.That(!double.IsNaN(FormulaCalculator.Calculate("-1")));
            [Test] public static void Case11() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("*1")));
            [Test] public static void Case12() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("/1")));
            [Test] public static void Case13() => Assert.That(!double.IsNaN(FormulaCalculator.Calculate("1+2")));
            [Test] public static void Case14() => Assert.That(!double.IsNaN(FormulaCalculator.Calculate("1-2")));
            [Test] public static void Case15() => Assert.That(!double.IsNaN(FormulaCalculator.Calculate("1*2")));
            [Test] public static void Case16() => Assert.That(!double.IsNaN(FormulaCalculator.Calculate("1/2")));
            [Test] public static void Case17() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("1++2")));
            [Test] public static void Case18() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("1+*2")));
        }

        public static class Tests_IsValid_IsParagraphOK
        {
            [Test] public static void Case1() => Assert.That(double.IsNaN(FormulaCalculator.Calculate(")(")));
            [Test] public static void Case2() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("(()")));
            [Test] public static void Case3() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("())")));
            [Test] public static void Case4() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("((()))())")));
            [Test] public static void Case5() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("((()))(()")));
            [Test] public static void Case6() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("()")));
            [Test] public static void Case7() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("( )")));
            [Test] public static void Case8() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("(+ )")));
            [Test] public static void Case9() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("(- )")));
            [Test] public static void Case10() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("(* )")));
            [Test] public static void Case11() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("(/ )")));
            [Test] public static void Case12() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("(+)")));
            [Test] public static void Case13() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("(-)")));
            [Test] public static void Case14() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("(*)")));
            [Test] public static void Case15() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("(/)")));
            [Test] public static void Case16() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("(+3-4*)")));
            [Test] public static void Case17() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("(-3+4/)")));
            [Test] public static void Case18() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("(*3-4+)")));
            [Test] public static void Case19() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("(/3+4-)")));
            [Test] public static void Case20() => Assert.That(!double.IsNaN(FormulaCalculator.Calculate("(3)")));
            [Test] public static void Case21() => Assert.That(!double.IsNaN(FormulaCalculator.Calculate("((1+2))")));
            [Test] public static void Case22() => Assert.That(!double.IsNaN(FormulaCalculator.Calculate("((2+3)*4)")));
            [Test] public static void Case23() => Assert.That(!double.IsNaN(FormulaCalculator.Calculate("((2+3)*(4-1))")));
        }

        public static class Tests_Calculate
        {
            [Test] public static void Case1() => Assert.That(FormulaCalculator.Calculate("1+2*3").IsAlmostEqualTo(7.0));
            [Test] public static void Case2() => Assert.That(FormulaCalculator.Calculate("(1+2)*3").IsAlmostEqualTo(9.0));
            [Test] public static void Case3() => Assert.That(FormulaCalculator.Calculate("1+2* 3-4/5").IsAlmostEqualTo(6.2));
            [Test] public static void Case4() => Assert.That(FormulaCalculator.Calculate("( +(  1+2) *3-4) /5").IsAlmostEqualTo(1.0));
            [Test] public static void Case5() => Assert.That(FormulaCalculator.Calculate("1+2*3-4/5+(6-7*8+9)/10").IsAlmostEqualTo(2.1));
            [Test] public static void Case6() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("1/0")));
            [Test] public static void Case7() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("1/(2-2)")));
            [Test] public static void Case8() => Assert.That(double.IsNaN(FormulaCalculator.Calculate("1+2*3-4/5+(6-7*8+9)/0")));
            [Test] public static void Case9() => Assert.That(FormulaCalculator.Calculate("9999*9999*9999").IsAlmostEqualTo(short.MaxValue));
            [Test] public static void Case10() => Assert.That(FormulaCalculator.Calculate("-9999*9999*9999").IsAlmostEqualTo(short.MinValue));
            [Test] public static void Case11() => Assert.That(FormulaCalculator.Calculate("((((((((((1+2))))))))))").IsAlmostEqualTo(3.0));
            [Test] public static void Case12() => Assert.That(FormulaCalculator.Calculate("((((((((((1+2)*3-4/5+(6-7*8+9)/10)))))))))").IsAlmostEqualTo(4.1));
        }

        internal static bool IsAlmostEqualTo(this double a, double b, double epsilon = 1e-4)
            => System.Math.Abs(a - b) < epsilon;
    }
}
