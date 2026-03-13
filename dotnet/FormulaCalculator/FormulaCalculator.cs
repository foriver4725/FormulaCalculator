using System;
using System.Runtime.CompilerServices;

namespace foriver4725.FormulaCalculator
{
    public static class FormulaCalculator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe double Calculate(this ReadOnlySpan<char> formula)
        {
            int len = formula.Length;
            if (len == 0) return double.NaN;

            fixed (char* p = formula)
            {
                double* values = stackalloc double[len];
                char* ops = stackalloc char[len];

                int vTop = 0;
                int oTop = 0;

                byte prevType = Constants.PrevStart;
                int parenDepth = 0;

                int connectedNumber = -1;
                int digitCount = 0;
                bool sawMeaningful = false;

                bool currentNumberHasUnaryMinus = false;

                for (int i = 0; i < len; i++)
                {
                    char c = p[i];

                    if (c == ' ')
                        continue;

                    // digit
                    if ((uint)(c - '0') <= 9u)
                    {
                        if (prevType == Constants.PrevParenR)
                            return double.NaN; // ")1" NG

                        int digit = c - '0';

                        digitCount++;
                        if (digitCount > 8)
                            return double.NaN;

                        connectedNumber = (connectedNumber < 0)
                            ? digit
                            : (connectedNumber * 10 + digit);

                        prevType = Constants.PrevNumber;
                        sawMeaningful = true;
                        continue;
                    }

                    // close current number
                    if (connectedNumber >= 0)
                    {
                        if (currentNumberHasUnaryMinus && (c == '%' || c == '^'))
                            return double.NaN;

                        values[vTop++] = connectedNumber;
                        connectedNumber = -1;
                        digitCount = 0;

                        currentNumberHasUnaryMinus = false;
                    }

                    // '('
                    if (c == '(')
                    {
                        if (prevType == Constants.PrevNumber || prevType == Constants.PrevParenR)
                            return double.NaN; // "2(" or ")(" NG

                        ops[oTop++] = c;
                        parenDepth++;
                        prevType = Constants.PrevParenL;
                        sawMeaningful = true;
                        continue;
                    }

                    // ')'
                    if (c == ')')
                    {
                        if (parenDepth <= 0)
                            return double.NaN;

                        if (prevType == Constants.PrevStart || prevType == Constants.PrevOp ||
                            prevType == Constants.PrevParenL)
                            return double.NaN; // empty or operator-only NG

                        while (oTop > 0 && ops[oTop - 1] != '(')
                        {
                            if (!ApplyTop(values, ref vTop, ops, ref oTop))
                                return double.NaN;
                        }

                        if (oTop == 0)
                            return double.NaN;

                        oTop--; // pop '('
                        parenDepth--;
                        prevType = Constants.PrevParenR;
                        sawMeaningful = true;
                        continue;
                    }

                    // operator
                    if (!Helpers.IsOperator(c))
                        return double.NaN;

                    if (c == '+' || c == '-')
                    {
                        // unary + / - is allowed only at start or right after '('
                        if (prevType == Constants.PrevStart || prevType == Constants.PrevParenL)
                        {
                            if (!Helpers.TryPeekNextNonSpace(p, len, i + 1, out char next))
                                return double.NaN;

                            if (!Helpers.IsDigit(next) && next != '(')
                                return double.NaN;

                            // unary minus directly attached to a bare number
                            if (c == '-' && Helpers.IsDigit(next))
                                currentNumberHasUnaryMinus = true;
                            else
                                currentNumberHasUnaryMinus = false;

                            // simulate unary as 0 +/- x
                            values[vTop++] = 0.0;
                            ops[oTop++] = c;
                            prevType = Constants.PrevOp;
                            sawMeaningful = true;
                            continue;
                        }
                    }

                    // binary operator must follow number or ')'
                    if (prevType != Constants.PrevNumber && prevType != Constants.PrevParenR)
                        return double.NaN;

                    while (oTop > 0 && Helpers.ShouldReduce(ops[oTop - 1], c))
                    {
                        if (!ApplyTop(values, ref vTop, ops, ref oTop))
                            return double.NaN;
                    }

                    ops[oTop++] = c;
                    prevType = Constants.PrevOp;
                    sawMeaningful = true;
                }

                // close trailing number
                if (connectedNumber >= 0)
                {
                    values[vTop++] = connectedNumber;
                }

                if (!sawMeaningful)
                    return double.NaN;

                if (parenDepth != 0)
                    return double.NaN;

                if (prevType != Constants.PrevNumber && prevType != Constants.PrevParenR)
                    return double.NaN;

                while (oTop > 0)
                {
                    if (ops[oTop - 1] == '(')
                        return double.NaN;

                    if (!ApplyTop(values, ref vTop, ops, ref oTop))
                        return double.NaN;
                }

                return vTop == 1 ? values[0] : double.NaN;
            }
        }

        // =========================================================
        // Core
        // =========================================================

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe bool ApplyTop(double* values, ref int vTop, char* ops, ref int oTop)
        {
            if (vTop < 2 || oTop <= 0)
                return false;

            char op = ops[oTop - 1];
            double b = values[vTop - 1];
            double a = values[vTop - 2];
            double result;

            if (op == '+')
            {
                result = a + b;
            }
            else if (op == '-')
            {
                result = a - b;
            }
            else if (op == '*')
            {
                result = a * b;
            }
            else if (op == '/')
            {
                if (b == 0.0)
                    return false;

                result = a / b;
            }
            else if (op == '%')
            {
                if (a <= 0.0 || b <= 0.0)
                    return false;

                if (!Helpers.IsInteger(a) || !Helpers.IsInteger(b))
                    return false;

                result = a % b;
            }
            else if (op == '^')
            {
                // 0^b
                if (a == 0.0)
                {
                    if (b <= 0.0)
                        return false;

                    result = 0.0;
                }
                else
                {
                    if (Helpers.IsInteger(b))
                    {
                        long e64 = (long)Math.Round(b);

                        if (e64 < int.MinValue || e64 > int.MaxValue)
                            return false;

                        int e = (int)e64;

                        if (e < 0)
                        {
                            int posE = -e;
                            double p = PowInt(a, posE);
                            if (p == 0.0)
                                return false;

                            result = 1.0 / p;
                        }
                        else
                        {
                            result = PowInt(a, e);
                        }
                    }
                    else
                    {
                        // negative base with non-integer exponent => not real
                        if (a < 0.0)
                            return false;

                        result = Math.Pow(a, b);
                    }
                }

                if (double.IsNaN(result) || double.IsInfinity(result))
                    return false;
            }
            else
            {
                return false;
            }

            values[vTop - 2] = result;
            vTop--;
            oTop--;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double PowInt(double a, int e)
        {
            double result = 1.0;
            double baseVal = a;

            while (e > 0)
            {
                if ((e & 1) != 0)
                    result *= baseVal;

                baseVal *= baseVal;
                e >>= 1;
            }

            return result;
        }
    }
}