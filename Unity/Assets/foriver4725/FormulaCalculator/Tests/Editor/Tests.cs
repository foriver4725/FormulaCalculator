using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace foriver4725.FormulaCalculator.Tests
{
    public static class Tests
    {
        // =========================================================
        // IsValidFormula : Valid syntax cases
        // =========================================================
        public static class IsValidFormula_ValidSyntaxCases
        {
            [TestCaseSource(nameof(Cases))]
            public static void Run(string f)
                => f.V();

            private static IEnumerable<TestCaseData> Cases()
            {
                // --- basic / whitespace
                yield return Valid("1+2*3", "Valid_Basic");
                yield return Valid(" 1 + 2 * 3 ", "Valid_Whitespace");
                yield return Valid(" 1 + 2.5 * 3 ", "Valid_Whitespace_WithDecimal");

                // --- number literals
                yield return Valid("0.5", "Valid_Number_Decimal_Basic");
                yield return Valid("12.34", "Valid_Number_Decimal_MultiDigit");
                yield return Valid("00012.340", "Valid_Number_Decimal_LeadingZeros");
                yield return Valid("00.00", "Valid_Number_Decimal_AllZero");
                yield return Valid("000123", "Valid_Number_LeadingZeros_Integer");
                yield return Valid("000123+1", "Valid_Number_LeadingZeros_InExpression");

                // --- operators
                yield return Valid("1+2", "Valid_Op_BinaryPlus");
                yield return Valid("1-2", "Valid_Op_BinaryMinus");
                yield return Valid("1*2", "Valid_Op_BinaryMul");
                yield return Valid("1/2", "Valid_Op_BinaryDiv");
                yield return Valid("1%2", "Valid_Mod_Basic");
                yield return Valid("1.5+2.5", "Valid_Op_BinaryPlus_WithDecimal");
                yield return Valid("1.5*2", "Valid_Op_BinaryMul_WithDecimal");

                // --- unary sign syntax
                // Unary +/- is allowed only immediately after '('.
                yield return Valid("(+7)^2.3", "Valid_UnaryPlus_AfterParenL_BeforePow");
                yield return Valid("(+7) ^ 2.3", "Valid_UnaryPlus_AfterParenL_BeforePow_WithSpaces");
                yield return Valid("14^(-12.3)", "Valid_UnaryMinus_AfterParenL_AsPowExp");
                yield return Valid("14 ^ (-12.3)", "Valid_UnaryMinus_AfterParenL_AsPowExp_WithSpaces");

                // --- parentheses
                yield return Valid("(3)", "Valid_Paren_SingleNumber");
                yield return Valid("(3.5)", "Valid_Paren_SingleDecimal");
                yield return Valid("(+3)", "Valid_Paren_SignedNumber_Plus");
                yield return Valid("(-3)", "Valid_Paren_SignedNumber_Minus");
                yield return Valid("(+3.5)", "Valid_Paren_SignedDecimal_Plus");
                yield return Valid("(-3.5)", "Valid_Paren_SignedDecimal_Minus");
                yield return Valid("((1+2))", "Valid_Paren_NestedSimple");
                yield return Valid("((2+3)*4)", "Valid_Paren_NestedMul");
                yield return Valid("((2+3)*(4-1))", "Valid_Paren_NestedComplex");
                yield return Valid("((2.5+3.5)*(4-1))", "Valid_Paren_NestedComplex_WithDecimal");
                yield return Valid("2*(3+4)", "Valid_Paren_NumberThenParenFixedByMul");
                yield return Valid("(1+2)*(3+4)", "Valid_Paren_ParenRThenParenLFixedByMul");
                yield return Valid("(1)*(2)", "Valid_Paren_Adjacent_FixedByMul");
                yield return Valid("(1.5)*(2)", "Valid_Paren_Adjacent_Decimal_FixedByMul");

                // --- mod syntax
                yield return Valid("1%(2)", "Valid_Mod_RightParenExpr");
                yield return Valid("(5)%2", "Valid_Mod_LeftParenExpr");
                yield return Valid("(1+4)%2", "Valid_Mod_LeftExpr");
                yield return Valid("5%(1+1)", "Valid_Mod_RightExpr");
                yield return Valid("(5%2)", "Valid_Paren_Mod_Basic");
                yield return Valid("(+5)%2", "Valid_Mod_LeftUnaryPlusNumber_Paren");
                yield return Valid("(-5)%2", "Valid_Mod_LeftUnaryMinusNumber_Paren");
                yield return Valid("(+5.0)%2", "Valid_Mod_LeftUnaryPlusDecimalInteger_Paren");
                yield return Valid("(-5.0)%2", "Valid_Mod_LeftUnaryMinusDecimalInteger_Paren");

                // --- pow syntax
                yield return Valid("1^2", "Valid_Pow_Basic");
                yield return Valid("1^(2)", "Valid_Pow_ExpParen");
                yield return Valid("(1)^2", "Valid_Pow_BaseParen");
                yield return Valid("4^(0.5)", "Valid_Pow_DecimalExp_Paren");
                yield return Valid("(1.5)^2", "Valid_Pow_DecimalBase_Paren");
                yield return Valid("1^(-2)", "Valid_Pow_NegExp_Paren");
                yield return Valid("1^(+2)", "Valid_Pow_PosExp_Paren");
                yield return Valid("1^(-2.5)", "Valid_Pow_NegDecimalExp_Paren");
                yield return Valid("1^(+2.5)", "Valid_Pow_PosDecimalExp_Paren");
                yield return Valid("(+2)^2", "Valid_Pow_LeftUnaryPlusNumber_Paren");
                yield return Valid("(-2)^2", "Valid_Pow_LeftUnaryMinusNumber_Paren");
                yield return Valid("(+2.5)^2", "Valid_Pow_LeftUnaryPlusDecimal_Paren");
                yield return Valid("(-2.5)^2", "Valid_Pow_LeftUnaryMinusDecimal_Paren");
            }
        }

        // =========================================================
        // IsValidFormula : Invalid syntax cases
        // =========================================================
        public static class IsValidFormula_InvalidSyntaxCases
        {
            [TestCaseSource(nameof(Cases))]
            public static void Run(string f)
                => f.Nv();

            private static IEnumerable<TestCaseData> Cases()
            {
                // --- empty / whitespace
                yield return Invalid("", "Invalid_Whole_Empty");
                yield return Invalid(" ", "Invalid_Whole_OnlySpaces");

                // --- invalid chars
                yield return Invalid("1+2a", "Invalid_Char_Letter");
                yield return Invalid("1+2_3", "Invalid_Char_Underscore");
                yield return Invalid("1+2?3", "Invalid_Char_Question");
                yield return Invalid("1+2,3", "Invalid_Decimal_Comma");

                // --- decimal format errors
                yield return Invalid("1+2..3", "Invalid_Decimal_DoubleDot");
                yield return Invalid("1+.", "Invalid_Decimal_DotOnlyAfterOp");
                yield return Invalid("1+.5", "Invalid_Decimal_NoIntegerPartAfterUnaryPlus");
                yield return Invalid("1+2.", "Invalid_Decimal_NoFractionPartAfterOp");
                yield return Invalid(".5", "Invalid_Decimal_NoIntegerPart");
                yield return Invalid("1.", "Invalid_Decimal_NoFractionPart");
                yield return Invalid(".", "Invalid_Decimal_OnlyDot");
                yield return Invalid("1.2.3", "Invalid_Decimal_MultipleDots");
                yield return Invalid("1 .2", "Invalid_Decimal_SpaceBeforeDot");
                yield return Invalid("1. 2", "Invalid_Decimal_SpaceAfterDot");
                yield return Invalid("1 2.3", "Invalid_Decimal_SpaceInsideIntegerPart");
                yield return Invalid("12.3 4", "Invalid_Decimal_SpaceInsideFractionPart");

                // --- adjacency / whitespace inside number
                yield return Invalid("2(3+4)", "Invalid_Adjacent_NumberThenParenL");
                yield return Invalid("(1+2)(3+4)", "Invalid_Adjacent_ParenRThenParenL");
                yield return Invalid("(1)(2)", "Invalid_Adjacent_ParenRParenL");
                yield return Invalid("(1.5)(2)", "Invalid_Adjacent_DecimalParenRParenL");
                yield return Invalid("1 23", "Invalid_Number_WhitespaceInsideDigits");
                yield return Invalid("12 3", "Invalid_Number_WhitespaceInsideDigits2");
                yield return Invalid("1 2 3", "Invalid_Number_WhitespaceInsideDigits3");
                yield return Invalid("(-123 4)", "Invalid_Number_Signed_WithWhitespaceInsideDigits");

                // --- only / trailing / leading operators
                yield return Invalid("+", "Invalid_Op_OnlyPlus");
                yield return Invalid("-", "Invalid_Op_OnlyMinus");
                yield return Invalid("*", "Invalid_Op_OnlyMul");
                yield return Invalid("/", "Invalid_Op_OnlyDiv");
                yield return Invalid("%", "Invalid_Mod_Only");
                yield return Invalid("^", "Invalid_Pow_Only");
                yield return Invalid("1+", "Invalid_Op_TrailingPlus");
                yield return Invalid("1-", "Invalid_Op_TrailingMinus");
                yield return Invalid("1*", "Invalid_Op_TrailingMul");
                yield return Invalid("1/", "Invalid_Op_TrailingDiv");
                yield return Invalid("1%", "Invalid_Mod_Trailing");
                yield return Invalid("1^", "Invalid_Pow_Trailing");
                yield return Invalid("+1", "Invalid_Op_UnaryPlus_AtStart");
                yield return Invalid("-1", "Invalid_Op_UnaryMinus_AtStart");
                yield return Invalid("+1.5", "Invalid_Op_UnaryPlusDecimal_AtStart");
                yield return Invalid("-1.5", "Invalid_Op_UnaryMinusDecimal_AtStart");
                yield return Invalid("*1", "Invalid_Op_LeadingMul");
                yield return Invalid("/1", "Invalid_Op_LeadingDiv");
                yield return Invalid("%1", "Invalid_Mod_Leading");
                yield return Invalid("^1", "Invalid_Pow_Leading");

                // --- invalid operator sequences
                yield return Invalid("1++2", "Invalid_Op_DoublePlus");
                yield return Invalid("1+*2", "Invalid_Op_PlusThenMul");
                yield return Invalid("1*/2", "Invalid_Op_MulThenDiv");
                yield return Invalid("1/*2", "Invalid_Op_DivAfterMulStyle");
                yield return Invalid("1%%2", "Invalid_Mod_Double");
                yield return Invalid("1+%2", "Invalid_Mod_AfterPlus");
                yield return Invalid("1%*2", "Invalid_Mod_ThenMul");
                yield return Invalid("1/^2", "Invalid_Op_DivThenPowStyle");
                yield return Invalid("1%/2", "Invalid_Mod_ThenDiv");
                yield return Invalid("1%^2", "Invalid_Mod_ThenPow");

                // --- unary +/- is allowed only right after '('
                yield return Invalid("-23+1.2", "Invalid_UnaryMinus_AtStart");
                yield return Invalid("-23 + 1.2", "Invalid_UnaryMinus_AtStart_WithSpaces");
                yield return Invalid("23+-1.2", "Invalid_UnaryMinus_AfterBinaryPlus");
                yield return Invalid("23 + -1.2", "Invalid_UnaryMinus_AfterBinaryPlus_WithSpaces");
                yield return Invalid("+1.23*+9.24", "Invalid_UnaryPlus_AtStart_AndAfterBinaryMul");
                yield return Invalid("+1.23 * +9.24", "Invalid_UnaryPlus_AtStart_AndAfterBinaryMul_WithSpaces");
                yield return Invalid("1.23*+9.24", "Invalid_UnaryPlus_AfterBinaryMul");
                yield return Invalid("1.23 * +9.24", "Invalid_UnaryPlus_AfterBinaryMul_WithSpaces");

                // --- parentheses
                yield return Invalid(")(", "Invalid_Paren_WrongOrder");
                yield return Invalid("(()", "Invalid_Paren_UnmatchedL");
                yield return Invalid("())", "Invalid_Paren_UnmatchedR");
                yield return Invalid("((()))())", "Invalid_Paren_UnmatchedR_Deep");
                yield return Invalid("((()))(()", "Invalid_Paren_UnmatchedL_Deep");
                yield return Invalid("()", "Invalid_Paren_Empty");
                yield return Invalid("( )", "Invalid_Paren_Empty_WithSpace");
                yield return Invalid("(+)", "Invalid_Paren_OnlyPlus");
                yield return Invalid("(-)", "Invalid_Paren_OnlyMinus");
                yield return Invalid("(*)", "Invalid_Paren_OnlyMul");
                yield return Invalid("(/)", "Invalid_Paren_OnlyDiv");
                yield return Invalid("(%)", "Invalid_Paren_OnlyMod");
                yield return Invalid("(^)", "Invalid_Paren_OnlyPow");
                yield return Invalid("(+ )", "Invalid_Paren_OnlyPlus_WithSpace");
                yield return Invalid("(- )", "Invalid_Paren_OnlyMinus_WithSpace");
                yield return Invalid("(* )", "Invalid_Paren_OnlyMul_WithSpace");
                yield return Invalid("(/ )", "Invalid_Paren_OnlyDiv_WithSpace");
                yield return Invalid("(% )", "Invalid_Paren_OnlyMod_WithSpace");
                yield return Invalid("(^ )", "Invalid_Paren_OnlyPow_WithSpace");
                yield return Invalid("(+3-4*)", "Invalid_Paren_TrailingMul");
                yield return Invalid("(-3+4/)", "Invalid_Paren_TrailingDiv");
                yield return Invalid("(*3-4+)", "Invalid_Paren_LeadingMul_TrailingPlus");
                yield return Invalid("(/3+4-)", "Invalid_Paren_LeadingDiv_TrailingMinus");
                yield return Invalid("(%2)", "Invalid_Paren_LeadingMod");
                yield return Invalid("(2%)", "Invalid_Paren_TrailingMod");

                // --- pow syntax
                yield return Invalid("1^)", "Invalid_Pow_ThenParenR");
                yield return Invalid("(^1)", "Invalid_Pow_InParen");
                yield return Invalid("1^-2", "Invalid_Pow_NegExp_NoParen");
                yield return Invalid("1^+2", "Invalid_Pow_PosExp_NoParen");
                yield return Invalid("1^-2.5", "Invalid_Pow_NegDecimalExp_NoParen");
                yield return Invalid("1^+2.5", "Invalid_Pow_PosDecimalExp_NoParen");
                yield return Invalid("1^+", "Invalid_Pow_ThenPlusOnly");
                yield return Invalid("1^-", "Invalid_Pow_ThenMinusOnly");
                yield return Invalid("+7^2.3", "Invalid_Pow_UnaryPlusAtStart_BeforePow");
                yield return Invalid("+7 ^ 2.3", "Invalid_Pow_UnaryPlusAtStart_BeforePow_WithSpaces");
                yield return Invalid("-14^(-12.3)", "Invalid_Pow_UnaryMinusAtStart_BeforePowParenExp");
                yield return Invalid("-14 ^ (-12.3)", "Invalid_Pow_UnaryMinusAtStart_BeforePowParenExp_WithSpaces");
                yield return Invalid("+2^2", "Invalid_Pow_LeftUnaryPlusAtStart");
                yield return Invalid("-2^2", "Invalid_Pow_LeftUnaryMinusAtStart");
                yield return Invalid("+2.5^2", "Invalid_Pow_LeftUnaryPlusDecimalAtStart");
                yield return Invalid("-2.5^2", "Invalid_Pow_LeftUnaryMinusDecimalAtStart");
            }
        }

        // =========================================================
        // Calculate : Valid result cases
        // =========================================================
        public static class Calculate_ValidResultCases
        {
            [TestCaseSource(nameof(Cases))]
            public static void Run(string f, double expected)
                => f.Eq(expected);

            private static IEnumerable<TestCaseData> Cases()
            {
                // --- precedence / parentheses / whitespace
                yield return Result("1+2*3", 7.0, "Result_Prec_MulBeforeAdd");
                yield return Result("(1+2)*3", 9.0, "Result_Paren_OverridesPrec");
                yield return Result("1+2* 3-4/5", 6.2, "Result_Whitespace_Mixed");
                yield return Result("( +(  1+2) *3-4) /5", 1.0, "Result_Whitespace_Heavy");
                yield return Result("1+2*3-4/5+(6-7*8+9)/10", 2.1, "Result_LongExpression");

                // --- decimal arithmetic
                yield return Result("1.5+2.25", 3.75, "Result_Decimal_Add");
                yield return Result("2.5*4", 10.0, "Result_Decimal_Mul");
                yield return Result("(1.5+2.5)*2", 8.0, "Result_Decimal_Paren");
                yield return Result("(-0.5)+1.25", 0.75, "Result_Decimal_SignedByParen");
                yield return Result("00012.340+0.660", 13.0, "Result_Decimal_LeadingZeros");
                yield return Result("1 + 2.5 * 3", 8.5, "Result_Decimal_WithWhitespace");

                // --- large
                yield return Result("9999*9999*9999", 9999.0 * 9999.0 * 9999.0, "Result_Large_Positive");
                yield return Result("(-9999)*9999*9999", -9999.0 * 9999.0 * 9999.0, "Result_Large_NegativeByParen");

                // --- deep parentheses
                yield return Result("((((((((((1+2))))))))))", 3.0, "Result_DeepParen_Short");
                yield return Result("((((((((((1+2)*3-4/5+(6-7*8+9)/10)))))))))", 4.1, "Result_DeepParen_Long");

                // --- pow associativity / precedence
                yield return Result("2^3", 8.0, "Result_Pow_Basic");
                yield return Result("2^3^2", 512.0, "Result_Pow_RightAssociative");
                yield return Result("(2^3)^2", 64.0, "Result_Pow_ParenAssociative");
                yield return Result("2^(3^2)", 512.0, "Result_Pow_ExplicitRight");
                yield return Result("2*3^2", 18.0, "Result_Pow_BeforeMul_Left");
                yield return Result("2^3*2", 16.0, "Result_Pow_BeforeMul_Right");
                yield return Result("(+2)^2", 4.0, "Result_Pow_LeftUnaryPlus_Paren");
                yield return Result("(-2)^2", 4.0, "Result_Pow_LeftUnaryMinus_Paren");
                yield return Result("(+2.5)^2", 6.25, "Result_Pow_LeftUnaryPlusDecimal_Paren");
                yield return Result("(-2.5)^2", 6.25, "Result_Pow_LeftUnaryMinusDecimal_IntegerExp_Paren");

                // --- unary with mod/pow
                yield return Result("(+5)%2", 1.0, "Result_Mod_LeftUnaryPlus_Paren");
                yield return Result("(0-(5%2))", -1.0, "Result_Mod_NegatedByExplicitSubtraction");
                yield return Result("(0-(2^2))", -4.0, "Result_Pow_NegatedByExplicitSubtraction");
                yield return Result("(-2)^2", 4.0, "Result_Pow_NegativeBase_Paren");
                yield return Result("(-2.5)^2", 6.25, "Result_Pow_NegativeDecimalBase_Paren");
                yield return Result("3^(-2)", 1.0 / 9.0, "Result_Pow_NegExp_Paren");
                yield return Result("3^(-2.5)", Math.Pow(3.0, -2.5), "Result_Pow_NegDecimalExp_Paren");

                // --- pow rule: zero base requires positive exponent
                yield return Result("0^1", 0.0, "Result_Pow_Zero_Pos1");
                yield return Result("0^2", 0.0, "Result_Pow_Zero_Pos2");
                yield return Result("0^0.5", 0.0, "Result_Pow_Zero_PosFraction");

                // --- pow rule: non-zero base with integer exponent accepts any base
                yield return Result("2^3", 8.0, "Result_Pow_NonZeroBase_PosIntegerExp");
                yield return Result("52.3^(-78)", Math.Pow(52.3, -78.0), "Result_Pow_PositiveDecimalBase_NegIntegerExp");
                yield return Result("(-81)^0", 1.0, "Result_Pow_NegativeBase_ZeroExp");
                yield return Result("(-2.3)^(-942)", Math.Pow(-2.3, -942.0), "Result_Pow_NegativeDecimalBase_NegIntegerExp");

                // --- pow rule: non-zero base with real exponent requires positive base
                yield return Result("0.1^2.3", Math.Pow(0.1, 2.3), "Result_Pow_PositiveSmallBase_PosRealExp");
                yield return Result("3.45^(-12.34)", Math.Pow(3.45, -12.34), "Result_Pow_PositiveDecimalBase_NegRealExp");

                // --- fractional exponent
                yield return Result("4^(1/2)", 2.0, "Result_Pow_FractionExp_PositiveBase");
                yield return Result("2^(3/10)", Math.Pow(2.0, 0.3), "Result_Pow_FractionExp");
                yield return Result("(73/23)^(11/3)", Math.Pow(73.0 / 23.0, 11.0 / 3.0), "Result_Pow_FractionExp2");
                yield return Result("4^0.5", 2.0, "Result_Pow_DecimalExp_PositiveBase");
                yield return Result("2^1.5", Math.Pow(2.0, 1.5), "Result_Pow_DecimalExp");
                yield return Result("(73/23)^3.5", Math.Pow(73.0 / 23.0, 3.5), "Result_Pow_DecimalExp2");
                yield return Result("4^( 1 / 2 )", 2.0, "Result_Pow_FractionExp_WithWhitespace");

                // --- mod basic / precedence
                yield return Result("5%2", 1.0, "Result_Mod_Basic");
                yield return Result("10%6%4", 0.0, "Result_Mod_LeftAssociative");
                yield return Result("1+5%2*3", 4.0, "Result_Mod_Precedence_WithMulAdd");
                yield return Result("2^3%3", 2.0, "Result_Mod_AfterPow");
                yield return Result("2%3^2", 2.0, "Result_Mod_BeforePowResult");
                yield return Result("(1+4)%2", 1.0, "Result_Mod_LeftExpr");
                yield return Result("8%(1+2)", 2.0, "Result_Mod_RightExpr");
                yield return Result("(2^3)%3", 2.0, "Result_Mod_LeftPowExpr");

                // --- mod - values that evaluate to integers are allowed
                yield return Result("(8/4)%2", 0.0, "Result_Mod_LeftExpr_Integer");
                yield return Result("8%(6/2)", 2.0, "Result_Mod_RightExpr_Integer");
                yield return Result("6%3", 0.0, "Result_Mod_ExactDivision");
                yield return Result("1%2+3", 4.0, "Result_Mod_BeforeAdd");
                yield return Result("6%(1+2)", 0.0, "Result_Mod_RightExpr_ExactDivision");
                yield return Result("(1+5)%(1+2)", 0.0, "Result_Mod_BothExpr_ExactDivision");
                yield return Result("5%2.0", 1.0, "Result_Mod_RightDecimalTextButInteger");
                yield return Result("5.0%2", 1.0, "Result_Mod_LeftDecimalTextButInteger");
            }
        }

        // =========================================================
        // Calculate : Valid syntax, but runtime NaN cases
        // =========================================================
        public static class Calculate_ValidButRuntimeNaNCases
        {
            [TestCaseSource(nameof(Cases))]
            public static void Run(string f)
                => f.Eq(double.NaN);

            private static IEnumerable<TestCaseData> Cases()
            {
                // --- div by zero
                yield return RuntimeNaN("1/0", "RuntimeNaN_DivZero");
                yield return RuntimeNaN("1/(2-2)", "RuntimeNaN_DivZero_InParen");
                yield return RuntimeNaN("1+2*3-4/5+(6-7*8+9)/0", "RuntimeNaN_DivZero_Late");
                yield return RuntimeNaN("1.0/0", "RuntimeNaN_DivZero_DecimalNumerator");
                yield return RuntimeNaN("1/(0^1)", "RuntimeNaN_Div_By_Zero");

                // --- 0^0 group
                yield return RuntimeNaN("0^0", "RuntimeNaN_Pow_0_0");
                yield return RuntimeNaN("(0)^0", "RuntimeNaN_Pow_0_0_BaseParen");
                yield return RuntimeNaN("0^(0)", "RuntimeNaN_Pow_0_0_ExpParen");
                yield return RuntimeNaN("(3-3)^0", "RuntimeNaN_Pow_0_0_ByExpr");
                yield return RuntimeNaN("(4-2*2)^(3-3)", "RuntimeNaN_Pow_0_0_ByExpr2");
                yield return RuntimeNaN("1/(0^0)", "RuntimeNaN_Div_By_NaN");

                // --- zero base with negative exponent
                yield return RuntimeNaN("0^(-1)", "RuntimeNaN_Pow_Zero_Neg");
                yield return RuntimeNaN("(0)^(1-2)", "RuntimeNaN_Pow_Zero_Neg_ByExpr");

                // --- pow rule: negative base with real exponent is not a real number
                yield return RuntimeNaN("(-1)^0.5", "RuntimeNaN_Pow_NegativeBase_FractionExp");
                yield return RuntimeNaN("(-8)^(1/3)", "RuntimeNaN_Pow_NegativeBase_FractionExpressionExp_CurrentSpec");
                yield return RuntimeNaN("(-0.3)^71.2", "RuntimeNaN_Pow_NegativeDecimalBase_PosRealExp");
                yield return RuntimeNaN("(-27.431)^(-931.4)", "RuntimeNaN_Pow_NegativeDecimalBase_NegRealExp");

                // --- mod - operands should be positive integer values
                yield return RuntimeNaN("0%2", "RuntimeNaN_Mod_LeftZero");
                yield return RuntimeNaN("5%0", "RuntimeNaN_Mod_RightZero");
                yield return RuntimeNaN("1%(2-2)", "RuntimeNaN_Mod_RightZero_ByExpr");
                yield return RuntimeNaN("5%0.0", "RuntimeNaN_Mod_RightZeroDecimal");
                yield return RuntimeNaN("(-5)%2", "RuntimeNaN_Mod_LeftNegative_Paren");
                yield return RuntimeNaN("(-5.0)%2", "RuntimeNaN_Mod_LeftNegativeDecimalInteger_Paren");
                yield return RuntimeNaN("5%(-2)", "RuntimeNaN_Mod_RightNegative");
                yield return RuntimeNaN("5%(-2.0)", "RuntimeNaN_Mod_RightNegativeDecimal");
                yield return RuntimeNaN("(5/2)%2", "RuntimeNaN_Mod_LeftFraction");
                yield return RuntimeNaN("5%(3/2)", "RuntimeNaN_Mod_RightFraction");
                yield return RuntimeNaN("5.5%2", "RuntimeNaN_Mod_LeftDecimalFraction");
                yield return RuntimeNaN("5%2.5", "RuntimeNaN_Mod_RightDecimalFraction");
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
            Assert.That(
                formula.AsSpan().IsValidFormula(),
                Is.True,
                $"Calculate test formula must be valid syntax: {formula}"
            );

            var actual = formula.AsSpan().Calculate();

            if (double.IsNaN(expected))
            {
                Assert.That(actual, Is.NaN, $"Expected NaN but was {actual}: {formula}");
                return;
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0e-8), formula);
        }

        private static TestCaseData Valid(string f, string name)
            => new TestCaseData(f).SetName(name);

        private static TestCaseData Invalid(string f, string name)
            => new TestCaseData(f).SetName(name);

        private static TestCaseData Result(string f, double expected, string name)
            => new TestCaseData(f, expected).SetName(name);

        private static TestCaseData RuntimeNaN(string f, string name)
            => new TestCaseData(f).SetName(name);
    }
}
