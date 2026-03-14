using System;
using System.Collections.Generic;
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

                // --- invalid chars
                yield return Case("1+2a", false, "Whole_InvalidChar_Letter_NG");
                yield return Case("1+2_3", false, "Whole_InvalidChar_Underscore_NG");
                yield return Case("1+2?3", false, "Whole_InvalidChar_Question_NG");

                // --- decimal charset / dot rules
                yield return Case("1+2.3", true, "Whole_Decimal_Dot_OK");
                yield return Case("1+2,3", false, "Whole_Decimal_Comma_NG");
                yield return Case("1+2..3", false, "Whole_Decimal_DoubleDot_NG");
                yield return Case("1+.", false, "Whole_Decimal_DotOnly_NG");
                yield return Case("1+.5", false, "Whole_Decimal_NoIntegerPart_NG");
                yield return Case("1+2.", false, "Whole_Decimal_NoFractionPart_NG");

                // --- basic valid
                yield return Case("1+2*3", true, "Whole_Basic_OK");
                yield return Case(" 1 + 2 * 3 ", true, "Whole_Whitespace_OK");
                yield return Case(" 1 + 2.5 * 3 ", true, "Whole_Whitespace_WithDecimal_OK");
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

                // --- whitespace inside number is not allowed
                yield return Case("1 23", false, "Number_WhitespaceInsideDigits_NG");
                yield return Case("12 3", false, "Number_WhitespaceInsideDigits_NG2");
                yield return Case("1 2 3", false, "Number_WhitespaceInsideDigits_NG3");
                yield return Case("-123 4", false, "Number_Signed_WithWhitespaceInsideDigits_NG");

                // --- decimal literals
                yield return Case("0.5", true, "Number_Decimal_Basic_OK");
                yield return Case("12.34", true, "Number_Decimal_MultiDigit_OK");
                yield return Case("+0.5", true, "Number_Decimal_UnaryPlus_OK");
                yield return Case("-0.5", true, "Number_Decimal_UnaryMinus_OK");
                yield return Case("00012.340", true, "Number_Decimal_LeadingZeros_OK");
                yield return Case("00.00", true, "Number_Decimal_AllZero_OK");

                // --- decimal format errors
                yield return Case(".5", false, "Number_Decimal_NoIntegerPart_NG");
                yield return Case("1.", false, "Number_Decimal_NoFractionPart_NG");
                yield return Case(".", false, "Number_Decimal_OnlyDot_NG");
                yield return Case("1.2.3", false, "Number_Decimal_MultipleDots_NG");
                yield return Case("1 .2", false, "Number_Decimal_SpaceBeforeDot_NG");
                yield return Case("1. 2", false, "Number_Decimal_SpaceAfterDot_NG");
                yield return Case("1 2.3", false, "Number_Decimal_SpaceInsideIntegerPart_NG");
                yield return Case("12.3 4", false, "Number_Decimal_SpaceInsideFractionPart_NG");

                // --- leading sign sequences under the existing spec
                yield return Case("+12-21", true, "Number_SignedSequence_OK");
                yield return Case("+12.5-21.25", true, "Number_SignedSequence_WithDecimal_OK");

                // --- leading zeros are allowed
                yield return Case("000123", true, "Number_LeadingZeros_Integer_OK");
                yield return Case("000123+1", true, "Number_LeadingZeros_InExpression_OK");
            }
        }

        // =========================================================
        // IsValidFormula : Operator (+-*/%^)
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
                yield return Case("%", false, "Mod_Only_NG");
                yield return Case("^", false, "Pow_Only_NG");

                // --- trailing operators
                yield return Case("1+", false, "Op_TrailingPlus_NG");
                yield return Case("1-", false, "Op_TrailingMinus_NG");
                yield return Case("1*", false, "Op_TrailingMul_NG");
                yield return Case("1/", false, "Op_TrailingDiv_NG");
                yield return Case("1%", false, "Mod_Trailing_NG");
                yield return Case("1^", false, "Pow_Trailing_NG");

                // --- leading operators: unary +/- allowed, others not
                yield return Case("+1", true, "Op_UnaryPlus_AtStart_OK");
                yield return Case("-1", true, "Op_UnaryMinus_AtStart_OK");
                yield return Case("+1.5", true, "Op_UnaryPlusDecimal_AtStart_OK");
                yield return Case("-1.5", true, "Op_UnaryMinusDecimal_AtStart_OK");
                yield return Case("*1", false, "Op_LeadingMul_NG");
                yield return Case("/1", false, "Op_LeadingDiv_NG");
                yield return Case("%1", false, "Mod_Leading_NG");
                yield return Case("^1", false, "Pow_Leading_NG");

                // --- binary basic
                yield return Case("1+2", true, "Op_BinaryPlus_OK");
                yield return Case("1-2", true, "Op_BinaryMinus_OK");
                yield return Case("1*2", true, "Op_BinaryMul_OK");
                yield return Case("1/2", true, "Op_BinaryDiv_OK");
                yield return Case("1%2", true, "Mod_Basic_OK");
                yield return Case("1.5+2.5", true, "Op_BinaryPlus_WithDecimal_OK");
                yield return Case("1.5*2", true, "Op_BinaryMul_WithDecimal_OK");

                // --- invalid operator sequences
                yield return Case("1++2", false, "Op_DoublePlus_NG");
                yield return Case("1+*2", false, "Op_PlusThenMul_NG");
                yield return Case("1*/2", false, "Op_MulThenDiv_NG");
                yield return Case("1/*2", false, "Op_DivAfterMulStyle_NG");
                yield return Case("1%%2", false, "Mod_Double_NG");
                yield return Case("1+%2", false, "Mod_AfterPlus_NG");
                yield return Case("1%*2", false, "Mod_ThenMul_NG");
                yield return Case("1/^2", false, "Op_DivThenPowStyle_NG");
                yield return Case("1%/2", false, "Mod_ThenDiv_NG");
                yield return Case("1%^2", false, "Mod_ThenPow_NG");

                // --- mod with parentheses
                yield return Case("1%(2)", true, "Mod_RightParenExpr_OK");
                yield return Case("(5)%2", true, "Mod_LeftParenExpr_OK");
                yield return Case("(1+4)%2", true, "Mod_LeftExpr_OK");
                yield return Case("5%(1+1)", true, "Mod_RightExpr_OK");

                // --- pow basic
                yield return Case("1^2", true, "Pow_Basic_OK");
                yield return Case("1^)", false, "Pow_ThenParenR_NG");
                yield return Case("(^1)", false, "Pow_InParen_Invalid_NG");

                // --- pow with parentheses
                yield return Case("1^(2)", true, "Pow_ExpParen_OK");
                yield return Case("(1)^2", true, "Pow_BaseParen_OK");
                yield return Case("4^(0.5)", true, "Pow_DecimalExp_Paren_OK");
                yield return Case("(1.5)^2", true, "Pow_DecimalBase_Paren_OK");

                // --- pow with sign on exponent: current spec requires parentheses for signed exponent
                yield return Case("1^-2", false, "Pow_NegExp_NoParen_NG_CurrentSpec");
                yield return Case("1^+2", false, "Pow_PosExp_NoParen_NG_CurrentSpec");
                yield return Case("1^(-2)", true, "Pow_NegExp_Paren_OK");
                yield return Case("1^(+2)", true, "Pow_PosExp_Paren_OK");
                yield return Case("1^(-2.5)", true, "Pow_NegDecimalExp_Paren_OK");
                yield return Case("1^(+2.5)", true, "Pow_PosDecimalExp_Paren_OK");
                yield return Case("1^-2.5", false, "Pow_NegDecimalExp_NoParen_NG_CurrentSpec");
                yield return Case("1^+2.5", false, "Pow_PosDecimalExp_NoParen_NG_CurrentSpec");

                // --- unary with mod
                yield return Case("+5%2", true, "Mod_LeftUnaryPlusNumber_OK");
                yield return Case("-5%2", false, "Mod_LeftUnaryMinusNumber_NG");
                yield return Case("+5.0%2", true, "Mod_LeftUnaryPlusDecimalInteger_OK");
                yield return Case("-5.0%2", false, "Mod_LeftUnaryMinusDecimalInteger_NG");

                // --- unary with pow
                yield return Case("+2^2", true, "Pow_LeftUnaryPlusNumber_OK");
                yield return Case("-2^2", false, "Pow_LeftUnaryMinusNumber_NG");
                yield return Case("+2.5^2", true, "Pow_LeftUnaryPlusDecimal_OK");
                yield return Case("-2.5^2", false, "Pow_LeftUnaryMinusDecimal_NG");

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

                // --- empty parentheses
                yield return Case("()", false, "Paren_Empty_NG");
                yield return Case("( )", false, "Paren_Empty_WithSpace_NG");
                yield return Case("(3)", true, "Paren_SingleNumber_OK");
                yield return Case("(3.5)", true, "Paren_SingleDecimal_OK");

                // --- operator-only inside parentheses
                yield return Case("(+)", false, "Paren_OnlyPlus_NG");
                yield return Case("(-)", false, "Paren_OnlyMinus_NG");
                yield return Case("(*)", false, "Paren_OnlyMul_NG");
                yield return Case("(/)", false, "Paren_OnlyDiv_NG");
                yield return Case("(%)", false, "Paren_OnlyMod_NG");
                yield return Case("(^)", false, "Paren_OnlyPow_NG");

                // --- whitespace variants
                yield return Case("(+ )", false, "Paren_OnlyPlus_WithSpace_NG");
                yield return Case("(- )", false, "Paren_OnlyMinus_WithSpace_NG");
                yield return Case("(* )", false, "Paren_OnlyMul_WithSpace_NG");
                yield return Case("(/ )", false, "Paren_OnlyDiv_WithSpace_NG");
                yield return Case("(% )", false, "Paren_OnlyMod_WithSpace_NG");
                yield return Case("(^ )", false, "Paren_OnlyPow_WithSpace_NG");

                // --- signed numbers in parentheses
                yield return Case("(+3)", true, "Paren_SignedNumber_Plus_OK");
                yield return Case("(-3)", true, "Paren_SignedNumber_Minus_OK");
                yield return Case("(+3.5)", true, "Paren_SignedDecimal_Plus_OK");
                yield return Case("(-3.5)", true, "Paren_SignedDecimal_Minus_OK");

                // --- trailing operator inside parentheses
                yield return Case("(+3-4*)", false, "Paren_TrailingMul_NG");
                yield return Case("(-3+4/)", false, "Paren_TrailingDiv_NG");
                yield return Case("(*3-4+)", false, "Paren_LeadingMul_TrailingPlus_NG");
                yield return Case("(/3+4-)", false, "Paren_LeadingDiv_TrailingMinus_NG");

                yield return Case("((1+2))", true, "Paren_NestedSimple_OK");
                yield return Case("((2+3)*4)", true, "Paren_NestedMul_OK");
                yield return Case("((2+3)*(4-1))", true, "Paren_NestedComplex_OK");
                yield return Case("((2.5+3.5)*(4-1))", true, "Paren_NestedComplex_WithDecimal_OK");

                // --- adjacency in parentheses context
                yield return Case("(1)(2)", false, "Paren_Adjacent_ParenRParenL_NG");
                yield return Case("(1)*(2)", true, "Paren_Adjacent_FixedByMul_OK");
                yield return Case("(1.5)(2)", false, "Paren_Adjacent_DecimalParenRParenL_NG");
                yield return Case("(1.5)*(2)", true, "Paren_Adjacent_Decimal_FixedByMul_OK");

                // --- mod with parentheses
                yield return Case("(5%2)", true, "Paren_Mod_Basic_OK");
                yield return Case("(%2)", false, "Paren_LeadingMod_NG");
                yield return Case("(2%)", false, "Paren_TrailingMod_NG");
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

                // --- decimal arithmetic
                yield return Case("1.5+2.25", 3.75, "Calc_Decimal_Add");
                yield return Case("2.5*4", 10.0, "Calc_Decimal_Mul");
                yield return Case("(1.5+2.5)*2", 8.0, "Calc_Decimal_Paren");
                yield return Case("-0.5+1.25", 0.75, "Calc_Decimal_Signed");
                yield return Case("00012.340+0.660", 13.0, "Calc_Decimal_LeadingZeros");
                yield return Case("1 + 2.5 * 3", 8.5, "Calc_Decimal_WithWhitespace");

                // --- div by zero -> NaN
                yield return Case("1/0", double.NaN, "Calc_DivZero");
                yield return Case("1/(2-2)", double.NaN, "Calc_DivZero_InParen");
                yield return Case("1+2*3-4/5+(6-7*8+9)/0", double.NaN, "Calc_DivZero_Late");
                yield return Case("1.0/0", double.NaN, "Calc_DivZero_DecimalNumerator");

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

                // --- unary with mod (plus OK, minus NG)
                yield return Case("+5%2", 1.0, "Calc_Mod_LeftUnaryPlus_OK");
                yield return Case("-5%2", double.NaN, "Calc_Mod_LeftNegative_NG");

                // --- unary with pow (plus OK, minus NG)
                yield return Case("+2^2", 4.0, "Calc_Pow_LeftUnaryPlus_OK");
                yield return Case("-2^2", double.NaN, "Calc_Pow_LeftNegative_NG");
                yield return Case("+2.5^2", 6.25, "Calc_Pow_LeftUnaryPlusDecimal_OK");
                yield return Case("-2.5^2", double.NaN, "Calc_Pow_LeftUnaryMinusDecimal_NG");

                // --- unary with mod/pow (with parentheses to make it valid)
                yield return Case("-(5%2)", -1.0, "Calc_Mod_NegatedByParen_OK");
                yield return Case("-(2^2)", -4.0, "Calc_Pow_NegatedByParen_OK");
                yield return Case("(-2)^2", 4.0, "Calc_Pow_NegativeBase_Paren_OK");
                yield return Case("(-2.5)^2", 6.25, "Calc_Pow_NegativeDecimalBase_Paren_OK");
                yield return Case("3^(-2)", 1.0 / 9.0, "Calc_Pow_NegExp_Paren_OK");
                yield return Case("3^(-2.5)", Math.Pow(3.0, -2.5), "Calc_Pow_NegDecimalExp_Paren_OK");

                // --- 0^0 group
                yield return Case("0^0", double.NaN, "Calc_Pow_0_0");
                yield return Case("(0)^0", double.NaN, "Calc_Pow_0_0_BaseParen");
                yield return Case("0^(0)", double.NaN, "Calc_Pow_0_0_ExpParen");
                yield return Case("(3-3)^0", double.NaN, "Calc_Pow_0_0_ByExpr");
                yield return Case("(4-2*2)^(3-3)", double.NaN, "Calc_Pow_0_0_ByExpr2");

                // --- zero base group
                yield return Case("0^1", 0.0, "Calc_Pow_Zero_Pos1");
                yield return Case("0^2", 0.0, "Calc_Pow_Zero_Pos2");
                yield return Case("0^(-1)", double.NaN, "Calc_Pow_Zero_Neg");
                yield return Case("(0)^(1-2)", double.NaN, "Calc_Pow_Zero_Neg_ByExpr");

                // --- NaN propagation group
                yield return Case("1/(0^0)", double.NaN, "Calc_Div_By_NaN");
                yield return Case("1/(0^1)", double.NaN, "Calc_Div_By_Zero");

                // --- fractional exponent
                yield return Case("4^(1/2)", 2.0, "Calc_Pow_FractionExp_PositiveBase");
                yield return Case("2^(3/10)", Math.Pow(2.0, 0.3), "Calc_Pow_FractionExp");
                yield return Case("(73/23)^(11/3)", Math.Pow(73.0 / 23.0, 11.0 / 3.0), "Calc_Pow_FractionExp2");
                yield return Case("4^0.5", 2.0, "Calc_Pow_DecimalExp_PositiveBase");
                yield return Case("2^1.5", Math.Pow(2.0, 1.5), "Calc_Pow_DecimalExp");
                yield return Case("(73/23)^3.5", Math.Pow(73.0 / 23.0, 3.5), "Calc_Pow_DecimalExp2");

                // --- fractional exponent with whitespace inside the exponent expression
                yield return Case("4^( 1 / 2 )", 2.0, "Calc_Pow_FractionExp_WithWhitespace");

                // --- mod basic
                yield return Case("5%2", 1.0, "Calc_Mod_Basic");
                yield return Case("10%6%4", 0.0, "Calc_Mod_LeftAssociative");

                // --- mod with other operators
                yield return Case("1+5%2*3", 4.0, "Calc_Mod_Precedence_WithMulAdd");
                yield return Case("2^3%3", 2.0, "Calc_Mod_AfterPow");
                yield return Case("2%3^2", 2.0, "Calc_Mod_BeforePowResult");

                // --- mod with parentheses
                yield return Case("(1+4)%2", 1.0, "Calc_Mod_LeftExpr");
                yield return Case("8%(1+2)", 2.0, "Calc_Mod_RightExpr");
                yield return Case("(2^3)%3", 2.0, "Calc_Mod_LeftPowExpr");

                // --- mod - numbers should be positive integers (0: NG)
                yield return Case("0%2", double.NaN, "Calc_Mod_LeftZero_NG");
                yield return Case("5%0", double.NaN, "Calc_Mod_RightZero_NG");
                yield return Case("1%(2-2)", double.NaN, "Calc_Mod_RightZero_ByExpr_NG");
                yield return Case("5%0.0", double.NaN, "Calc_Mod_RightZeroDecimal_NG");

                // --- mod - numbers should be positive integers (minus: NG)
                yield return Case("-5%2", double.NaN, "Calc_Mod_LeftNegative_NG");
                yield return Case("5%(-2)", double.NaN, "Calc_Mod_RightNegative_NG");
                yield return Case("(-5)%2", double.NaN, "Calc_Mod_LeftNegative_Paren_NG");
                yield return Case("5%(-2.0)", double.NaN, "Calc_Mod_RightNegativeDecimal_NG");

                // --- mod - numbers should be positive integers (non-integer: NG)
                yield return Case("(5/2)%2", double.NaN, "Calc_Mod_LeftFraction_NG");
                yield return Case("5%(3/2)", double.NaN, "Calc_Mod_RightFraction_NG");
                yield return Case("5.5%2", double.NaN, "Calc_Mod_LeftDecimalFraction_NG");
                yield return Case("5%2.5", double.NaN, "Calc_Mod_RightDecimalFraction_NG");

                // --- mod - values that evaluate to integers are allowed
                yield return Case("(8/4)%2", 0.0, "Calc_Mod_LeftExpr_Integer_OK");
                yield return Case("8%(6/2)", 2.0, "Calc_Mod_RightExpr_Integer_OK");
                yield return Case("6%3", 0.0, "Calc_Mod_ExactDivision");
                yield return Case("1%2+3", 4.0, "Calc_Mod_BeforeAdd");
                yield return Case("6%(1+2)", 0.0, "Calc_Mod_RightExpr_ExactDivision");
                yield return Case("(1+5)%(1+2)", 0.0, "Calc_Mod_BothExpr_ExactDivision");
                yield return Case("5%2.0", 1.0, "Calc_Mod_RightDecimalTextButInteger_OK");
                yield return Case("5.0%2", 1.0, "Calc_Mod_LeftDecimalTextButInteger_OK");
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