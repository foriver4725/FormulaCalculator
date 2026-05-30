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

                        i = end;
                        prevType = Constants.PrevNumber;
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
                        continue;
                    }

                    // -------------------------------------------------
                    // Right parenthesis
                    // -------------------------------------------------
                    if (c == ')')
                    {
                        if (parenDepth <= 0)
                            return false;

                        // Disallow empty parentheses and operator-only content.
                        if (prevType == Constants.PrevStart ||
                            prevType == Constants.PrevOp ||
                            prevType == Constants.PrevParenL)
                            return false;

                        parenDepth--;
                        prevType = Constants.PrevParenR;
                        continue;
                    }

                    // -------------------------------------------------
                    // Operator
                    // -------------------------------------------------
                    if (!Helpers.IsOperator(c))
                        return false;

                    if (c == '+' || c == '-')
                    {
                        // Unary +/- is allowed only immediately after '('.
                        //
                        // Valid:
                        //   (+7)
                        //   (-12.3)
                        //   (+(1+2))
                        //
                        // Invalid:
                        //   -23+1
                        //   1+-2
                        //   1^+2
                        if (prevType == Constants.PrevParenL)
                        {
                            char next = Helpers.PeekNextNonSpaceOrZero(p, len, i + 1);
                            if (next == '\0')
                                return false;

                            if (!Helpers.IsDigit(next) && next != '(')
                                return false;

                            prevType = Constants.PrevOp;
                            continue;
                        }
                    }

                    // Binary operators must follow a number or ')'.
                    if (prevType != Constants.PrevNumber && prevType != Constants.PrevParenR)
                        return false;

                    prevType = Constants.PrevOp;
                }

                return parenDepth == 0 &&
                       (prevType == Constants.PrevNumber || prevType == Constants.PrevParenR);
            }
        }
    }
}
