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
                int digitCount = 0;
                bool sawMeaningful = false;

                for (int i = 0; i < len; i++)
                {
                    char c = p[i];

                    if (c == ' ')
                        continue;

                    // digit
                    if ((uint)(c - '0') <= 9u)
                    {
                        if (prevType == Constants.PrevParenR)
                            return false; // ")1" adjacency NG

                        digitCount++;
                        if (digitCount > 8)
                            return false;

                        prevType = Constants.PrevNumber;
                        sawMeaningful = true;
                        continue;
                    }

                    // non-digit => current number is closed
                    digitCount = 0;

                    // '('
                    if (c == '(')
                    {
                        if (prevType == Constants.PrevNumber || prevType == Constants.PrevParenR)
                            return false; // "2(" or ")(" NG

                        parenDepth++;
                        prevType = Constants.PrevParenL;
                        sawMeaningful = true;
                        continue;
                    }

                    // ')'
                    if (c == ')')
                    {
                        if (parenDepth <= 0)
                            return false;

                        if (prevType == Constants.PrevStart || prevType == Constants.PrevOp ||
                            prevType == Constants.PrevParenL)
                            return false; // empty or operator-only NG

                        parenDepth--;
                        prevType = Constants.PrevParenR;
                        sawMeaningful = true;
                        continue;
                    }

                    // operator
                    if (Helpers.IsOperator(c))
                    {
                        if (c == '+' || c == '-')
                        {
                            // unary + / - is allowed only at start or right after '('
                            if (prevType == Constants.PrevStart || prevType == Constants.PrevParenL)
                            {
                                if (!Helpers.TryPeekNextNonSpace(p, len, i + 1, out char next))
                                    return false;

                                if (!Helpers.IsDigit(next) && next != '(')
                                    return false;

                                prevType = Constants.PrevOp;
                                sawMeaningful = true;
                                continue;
                            }
                        }

                        // binary operator must follow number or ')'
                        if (prevType != Constants.PrevNumber && prevType != Constants.PrevParenR)
                            return false;

                        prevType = Constants.PrevOp;
                        sawMeaningful = true;
                        continue;
                    }

                    // invalid charset
                    return false;
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