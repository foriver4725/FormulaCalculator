using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace foriver4725.FormulaCalculator.Tests
{
    public static class Tests
    {
        // =========================================================
        // IsValidFormula : Whole / Charset
        // =========================================================
        public static class IsValid_Whole
        {
            [TestCaseSource(nameof(Cases))]
            public static void Run(string f, bool expectedValid)
            {
                if (expectedValid) f.V();
                else f.Nv();
            }

            private static IEnumerable<TestCaseData> Cases()
            {
                // --- empty / whitespace
                yield return Case("", false, "Whole_Empty_NG");
                yield return Case(" ", false, "Whole_OnlySpaces_NG");

                // --- invalid chars (future: decimal support will flip this)
                yield return Case("1+2a", false, "Whole_InvalidChar_Letter_NG");
                yield return Case("1+2_3", false, "Whole_InvalidChar_Underscore_NG");
                yield return Case("1+2?3", false, "Whole_InvalidChar_Question_NG");

                // decimal not supported yet (as per your current tests)
                yield return Case("1+2.3", false, "Whole_Decimal_Dot_NG_NotSupportedYet");
                yield return Case("1+2,3", false, "Whole_Decimal_Comma_NG");

                // --- basic valid
                yield return Case("1+2*3", true, "Whole_Basic_OK");
                yield return Case(" 1 + 2 * 3 ", true, "Whole_Whitespace_OK");
            }
        }

        // =========================================================
        // IsValidFormula : Number
        // =========================================================
        public static class IsValid_Number
        {
            [TestCaseSource(nameof(Cases))]
            public static void Run(string f, bool expectedValid)
            {
                if (expectedValid) f.V();
                else f.Nv();
            }

            private static IEnumerable<TestCaseData> Cases()
            {
                // --- adjacency with parentheses
                yield return Case("2(3+4)", false, "Number_Adjacent_NumberThenParenL_NG");
                yield return Case("(1+2)(3+4)", false, "Number_Adjacent_ParenRThenParenL_NG");

                // --- adjacency with parentheses (fixed by operator between them)
                yield return Case("2*(3+4)", true, "Number_Adjacent_FixedByMul_OK");
                yield return Case("(1+2)*(3+4)", true, "Number_Adjacent_FixedByMul_OK2");

                // --- whitespace inside number (current spec)
                yield return Case("1 23", true, "Number_WhitespaceInsideDigits_OK_CurrentSpec");
                yield return Case("12 3", true, "Number_WhitespaceInsideDigits_OK_CurrentSpec2");
                yield return Case("1 2 3", true, "Number_WhitespaceInsideDigits_OK_CurrentSpec3");

                // --- leading sign sequences (current spec)
                yield return Case("+12-21", true, "Number_SignedSequence_OK");
                yield return Case("-123 4", true, "Number_Signed_WithWhitespaceInsideDigits_OK");

                // --- max digits boundary
                yield return Case("12345678", true, "Number_MaxDigits_OK");
                yield return Case("123456789", false, "Number_MaxDigits_TooLong_NG");

                // --- max digits at the end of the expression
                yield return Case("12345678+1", true, "Number_MaxDigits_InExpression_OK");
                yield return Case("123456789+1", false, "Number_MaxDigits_InExpression_NG");
            }
        }

        // =========================================================
        // IsValidFormula : Operator (+-*/ and ^)
        // =========================================================
        public static class IsValid_Operator
        {
            [TestCaseSource(nameof(Cases))]
            public static void Run(string f, bool expectedValid)
            {
                if (expectedValid) f.V();
                else f.Nv();
            }

            private static IEnumerable<TestCaseData> Cases()
            {
                // --- only operator
                yield return Case("+", false, "Op_OnlyPlus_NG");
                yield return Case("-", false, "Op_OnlyMinus_NG");
                yield return Case("*", false, "Op_OnlyMul_NG");
                yield return Case("/", false, "Op_OnlyDiv_NG");
                yield return Case("^", false, "Pow_Only_NG");

                // --- trailing operators
                yield return Case("1+", false, "Op_TrailingPlus_NG");
                yield return Case("1-", false, "Op_TrailingMinus_NG");
                yield return Case("1*", false, "Op_TrailingMul_NG");
                yield return Case("1/", false, "Op_TrailingDiv_NG");
                yield return Case("1^", false, "Pow_Trailing_NG");

                // --- leading operators: unary +/- allowed, others not
                yield return Case("+1", true, "Op_UnaryPlus_AtStart_OK");
                yield return Case("-1", true, "Op_UnaryMinus_AtStart_OK");
                yield return Case("*1", false, "Op_LeadingMul_NG");
                yield return Case("/1", false, "Op_LeadingDiv_NG");
                yield return Case("^1", false, "Pow_Leading_NG");

                // --- binary basic
                yield return Case("1+2", true, "Op_BinaryPlus_OK");
                yield return Case("1-2", true, "Op_BinaryMinus_OK");
                yield return Case("1*2", true, "Op_BinaryMul_OK");
                yield return Case("1/2", true, "Op_BinaryDiv_OK");

                // --- invalid operator sequences
                yield return Case("1++2", false, "Op_DoublePlus_NG");
                yield return Case("1+*2", false, "Op_PlusThenMul_NG");
                yield return Case("1*/2", false, "Op_MulThenDiv_NG");
                yield return Case("1/*2", false, "Op_DivAfterMulStyle_NG");

                // --- pow basic (paired)
                yield return Case("1^2", true, "Pow_Basic_OK");
                yield return Case("1^)", false, "Pow_ThenParenR_NG");
                yield return Case("(^1)", false, "Pow_InParen_Invalid_NG");

                // --- pow with parentheses (paired)
                yield return Case("1^(2)", true, "Pow_ExpParen_OK");
                yield return Case("(1)^2", true, "Pow_BaseParen_OK");

                // --- pow with sign on exponent: current spec requires parentheses for signed exponent
                yield return Case("1^-2", false, "Pow_NegExp_NoParen_NG_CurrentSpec");
                yield return Case("1^+2", false, "Pow_PosExp_NoParen_NG_CurrentSpec");
                yield return Case("1^(-2)", true, "Pow_NegExp_Paren_OK");
                yield return Case("1^(+2)", true, "Pow_PosExp_Paren_OK");

                // --- unary with pow (paired)
                yield return Case("+1^2", true, "Pow_UnaryPlusBase_OK");
                yield return Case("-1^2", true, "Pow_UnaryMinusBase_OK");

                // --- pow edge: exponent empty-ish
                yield return Case("1^+", false, "Pow_ThenPlusOnly_NG");
                yield return Case("1^-", false, "Pow_ThenMinusOnly_NG");
            }
        }

        // =========================================================
        // IsValidFormula : Parentheses
        // =========================================================
        public static class IsValid_Parentheses
        {
            [TestCaseSource(nameof(Cases))]
            public static void Run(string f, bool expectedValid)
            {
                if (expectedValid) f.V();
                else f.Nv();
            }

            private static IEnumerable<TestCaseData> Cases()
            {
                // --- matching / order
                yield return Case(")(", false, "Paren_WrongOrder_NG");
                yield return Case("(()", false, "Paren_UnmatchedL_NG");
                yield return Case("())", false, "Paren_UnmatchedR_NG");
                yield return Case("((()))())", false, "Paren_UnmatchedR_Deep_NG");
                yield return Case("((()))(()", false, "Paren_UnmatchedL_Deep_NG");

                // --- empty parentheses (current spec NG) + paired OK
                yield return Case("()", false, "Paren_Empty_NG");
                yield return Case("( )", false, "Paren_Empty_WithSpace_NG");
                yield return Case("(3)", true, "Paren_SingleNumber_OK");

                // --- operator-only inside parentheses (NG) + paired OK
                yield return Case("(+)", false, "Paren_OnlyPlus_NG");
                yield return Case("(-)", false, "Paren_OnlyMinus_NG");
                yield return Case("(*)", false, "Paren_OnlyMul_NG");
                yield return Case("(/)", false, "Paren_OnlyDiv_NG");

                // whitespace variants
                yield return Case("(+ )", false, "Paren_OnlyPlus_WithSpace_NG");
                yield return Case("(- )", false, "Paren_OnlyMinus_WithSpace_NG");
                yield return Case("(* )", false, "Paren_OnlyMul_WithSpace_NG");
                yield return Case("(/ )", false, "Paren_OnlyDiv_WithSpace_NG");

                // paired OK: signed number in parentheses
                yield return Case("(+3)", true, "Paren_SignedNumber_Plus_OK");
                yield return Case("(-3)", true, "Paren_SignedNumber_Minus_OK");

                // --- trailing operator inside parentheses (NG) + paired OK
                yield return Case("(+3-4*)", false, "Paren_TrailingMul_NG");
                yield return Case("(-3+4/)", false, "Paren_TrailingDiv_NG");
                yield return Case("(*3-4+)", false, "Paren_LeadingMul_TrailingPlus_NG");
                yield return Case("(/3+4-)", false, "Paren_LeadingDiv_TrailingMinus_NG");

                yield return Case("((1+2))", true, "Paren_NestedSimple_OK");
                yield return Case("((2+3)*4)", true, "Paren_NestedMul_OK");
                yield return Case("((2+3)*(4-1))", true, "Paren_NestedComplex_OK");

                // adjacency in parentheses context: NG and OK pair
                yield return Case("(1)(2)", false, "Paren_Adjacent_ParenRParenL_NG");
                yield return Case("(1)*(2)", true, "Paren_Adjacent_FixedByMul_OK");
            }
        }

        // =========================================================
        // Calculate
        // =========================================================
        public static class Calculate
        {
            [TestCaseSource(nameof(Cases))]
            public static void Run(string f, double expected)
                => f.Eq(expected);

            private static IEnumerable<TestCaseData> Cases()
            {
                // --- precedence / parentheses / whitespace
                yield return Case("1+2*3", 7.0, "Calc_Prec_MulBeforeAdd");
                yield return Case("(1+2)*3", 9.0, "Calc_Paren_OverridesPrec");
                yield return Case("1+2* 3-4/5", 6.2, "Calc_Whitespace_Mixed");
                yield return Case("( +(  1+2) *3-4) /5", 1.0, "Calc_Whitespace_Heavy");
                yield return Case("1+2*3-4/5+(6-7*8+9)/10", 2.1, "Calc_LongExpression");

                // --- div by zero -> NaN
                yield return Case("1/0", double.NaN, "Calc_DivZero");
                yield return Case("1/(2-2)", double.NaN, "Calc_DivZero_InParen");
                yield return Case("1+2*3-4/5+(6-7*8+9)/0", double.NaN, "Calc_DivZero_Late");

                // --- large
                yield return Case("9999*9999*9999", 9999.0 * 9999.0 * 9999.0, "Calc_Large_Positive");
                yield return Case("-9999*9999*9999", -9999.0 * 9999.0 * 9999.0, "Calc_Large_Negative");

                // --- deep parentheses
                yield return Case("((((((((((1+2))))))))))", 3.0, "Calc_DeepParen_Short");
                yield return Case("((((((((((1+2)*3-4/5+(6-7*8+9)/10)))))))))", 4.1, "Calc_DeepParen_Long");

                // --- pow associativity / precedence
                yield return Case("2^3", 8.0, "Calc_Pow_Basic");
                yield return Case("2^3^2", 512.0, "Calc_Pow_RightAssociative");
                yield return Case("(2^3)^2", 64.0, "Calc_Pow_ParenAssociative");
                yield return Case("2^(3^2)", 512.0, "Calc_Pow_ExplicitRight");

                yield return Case("2*3^2", 18.0, "Calc_Pow_BeforeMul_Left");
                yield return Case("2^3*2", 16.0, "Calc_Pow_BeforeMul_Right");

                // --- unary minus with pow (paired)
                yield return Case("-2^2", -4.0, "Calc_UnaryMinus_BindsAs0MinusPow");
                yield return Case("(-2)^2", 4.0, "Calc_UnaryMinus_ParenBase");

                // --- signed exponent (paired)
                yield return Case("3^(-2)", 1.0 / 9.0, "Calc_Pow_NegExp_Paren_OK");

                // --- 0^0 group (adjacent)
                yield return Case("0^0", double.NaN, "Calc_Pow_0_0");
                yield return Case("(0)^0", double.NaN, "Calc_Pow_0_0_BaseParen");
                yield return Case("0^(0)", double.NaN, "Calc_Pow_0_0_ExpParen");
                yield return Case("(3-3)^0", double.NaN, "Calc_Pow_0_0_ByExpr");
                yield return Case("(4-2*2)^(3-3)", double.NaN, "Calc_Pow_0_0_ByExpr2");

                // --- zero base group (adjacent)
                yield return Case("0^1", 0.0, "Calc_Pow_Zero_Pos1");
                yield return Case("0^2", 0.0, "Calc_Pow_Zero_Pos2");
                yield return Case("0^(-1)", double.NaN, "Calc_Pow_Zero_Neg");
                yield return Case("(0)^(1-2)", double.NaN, "Calc_Pow_Zero_Neg_ByExpr");

                // --- NaN propagation group (adjacent)
                yield return Case("1/(0^0)", double.NaN, "Calc_Div_By_NaN");
                yield return Case("1/(0^1)", double.NaN, "Calc_Div_By_Zero");

                // --- fractional exponent (adjacent)
                yield return Case("4^(1/2)", 2.0, "Calc_Pow_FractionExp_PositiveBase");
                yield return Case("2^(3/10)", Math.Pow(2.0, 0.3), "Calc_Pow_FractionExp");
                yield return Case("(73/23)^(11/3)", Math.Pow(73.0 / 23.0, 11.0 / 3.0), "Calc_Pow_FractionExp2");

                // --- fractional exponent (with whitespace inside the exponent expression)
                yield return Case("4^( 1 / 2 )", 2.0, "Calc_Pow_FractionExp_WithWhitespace");
            }
        }

        // =========================================================
        // Helpers
        // =========================================================
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void V(this string formula)
            => Assert.That(formula.AsSpan().IsValidFormula(), Is.True, $"Should be VALID: {formula}");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Nv(this string formula)
            => Assert.That(formula.AsSpan().IsValidFormula(), Is.False, $"Should be INVALID: {formula}");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Eq(this string formula, double expected)
        {
            var actual = formula.AsSpan().Calculate();

            if (double.IsNaN(expected))
            {
                Assert.That(actual, Is.NaN, $"Expected NaN but was {actual}: {formula}");
                return;
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0e-8), formula);
        }

        private static TestCaseData Case(string f, bool expectedValid, string name)
            => new TestCaseData(f, expectedValid).SetName(name);

        private static TestCaseData Case(string f, double expected, string name)
            => new TestCaseData(f, expected).SetName(name);
    }
}