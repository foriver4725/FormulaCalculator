using System;
using System.Runtime.CompilerServices;

namespace foriver4725.FormulaCalculator
{
    public static class FormulaValidator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool IsValidFormula(this ReadOnlySpan<char> formula)
        {
            int len = formula.Length;
            if (len == 0) return false;

            fixed (char* p = formula)
            {
                byte prevType = Constants.PrevStart;
                int parenDepth = 0;
                bool sawMeaningful = false;

                // Existing spec:
                // a unary minus directly attached to a bare number is invalid
                // when immediately followed by % or ^, e.g. "-5%2", "-2^2".
                bool currentNumberHasUnaryMinus = false;

                for (int i = 0; i < len; i++)
                {
                    char c = p[i];

                    if (c == ' ')
                        continue;

                    // -------------------------------------------------
                    // Number token
                    // -------------------------------------------------
                    if (Helpers.IsDigit(c))
                    {
                        // Disallow adjacency such as "1 23" or ")1".
                        if (prevType == Constants.PrevNumber || prevType == Constants.PrevParenR)
                            return false;

                        int end = Helpers.SkipNumberTokenOrMinusOne(p, len, i);
                        if (end < 0)
                            return false;

                        // Preserve the existing restriction for unary-minus bare numbers.
                        if (currentNumberHasUnaryMinus)
                        {
                            char nextAfterNumber = Helpers.PeekNextNonSpaceOrZero(p, len, end + 1);
                            if (nextAfterNumber == '%' || nextAfterNumber == '^')
                                return false;
                        }

                        i = end;
                        prevType = Constants.PrevNumber;
                        sawMeaningful = true;
                        currentNumberHasUnaryMinus = false;
                        continue;
                    }

                    // -------------------------------------------------
                    // Left parenthesis
                    // -------------------------------------------------
                    if (c == '(')
                    {
                        // Disallow adjacency such as "2(" or ")(".
                        if (prevType == Constants.PrevNumber || prevType == Constants.PrevParenR)
                            return false;

                        parenDepth++;
                        prevType = Constants.PrevParenL;
                        sawMeaningful = true;
                        currentNumberHasUnaryMinus = false;
                        continue;
                    }

                    // -------------------------------------------------
                    // Right parenthesis
                    // -------------------------------------------------
                    if (c == ')')
                    {
                        if (parenDepth <= 0)
                            return false;

                        // Disallow empty parentheses or operator-only content.
                        if (prevType == Constants.PrevStart ||
                            prevType == Constants.PrevOp ||
                            prevType == Constants.PrevParenL)
                            return false;

                        parenDepth--;
                        prevType = Constants.PrevParenR;
                        sawMeaningful = true;
                        currentNumberHasUnaryMinus = false;
                        continue;
                    }

                    // -------------------------------------------------
                    // Operator
                    // -------------------------------------------------
                    if (!Helpers.IsOperator(c))
                        return false;

                    if (c == '+' || c == '-')
                    {
                        // Unary +/- is allowed only at the start,
                        // or right after '('.
                        if (prevType == Constants.PrevStart || prevType == Constants.PrevParenL)
                        {
                            char next = Helpers.PeekNextNonSpaceOrZero(p, len, i + 1);
                            if (next == '\0')
                                return false;

                            // Existing spec:
                            // unary sign can be followed only by a digit or '('.
                            if (!Helpers.IsDigit(next) && next != '(')
                                return false;

                            currentNumberHasUnaryMinus = (c == '-' && Helpers.IsDigit(next));

                            prevType = Constants.PrevOp;
                            sawMeaningful = true;
                            continue;
                        }
                    }

                    // Binary operators must follow a number or ')'.
                    if (prevType != Constants.PrevNumber && prevType != Constants.PrevParenR)
                        return false;

                    prevType = Constants.PrevOp;
                    sawMeaningful = true;
                    currentNumberHasUnaryMinus = false;
                }

                if (!sawMeaningful)
                    return false;

                if (parenDepth != 0)
                    return false;

                return prevType == Constants.PrevNumber || prevType == Constants.PrevParenR;
            }
        }
    }
}