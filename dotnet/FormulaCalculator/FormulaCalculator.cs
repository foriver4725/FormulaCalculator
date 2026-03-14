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
                // Worst case:
                // every meaningful character becomes either a value or an operator.
                double* values = stackalloc double[len];
                char* ops = stackalloc char[len];

                int vTop = 0;
                int oTop = 0;

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
                            return double.NaN;

                        double number;
                        int end = Helpers.ReadNumberOrMinusOne(p, len, i, &number);
                        if (end < 0)
                            return double.NaN;

                        // Preserve the existing restriction for unary-minus bare numbers.
                        if (currentNumberHasUnaryMinus)
                        {
                            char nextAfterNumber = Helpers.PeekNextNonSpaceOrZero(p, len, end + 1);
                            if (nextAfterNumber == '%' || nextAfterNumber == '^')
                                return double.NaN;
                        }

                        values[vTop++] = number;

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
                            return double.NaN;

                        ops[oTop++] = c;
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
                            return double.NaN;

                        // Disallow empty parentheses or operator-only content.
                        if (prevType == Constants.PrevStart ||
                            prevType == Constants.PrevOp ||
                            prevType == Constants.PrevParenL)
                            return double.NaN;

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
                        currentNumberHasUnaryMinus = false;
                        continue;
                    }

                    // -------------------------------------------------
                    // Operator
                    // -------------------------------------------------
                    if (!Helpers.IsOperator(c))
                        return double.NaN;

                    if (c == '+' || c == '-')
                    {
                        // Unary +/- is allowed only at the start,
                        // or right after '('.
                        if (prevType == Constants.PrevStart || prevType == Constants.PrevParenL)
                        {
                            char next = Helpers.PeekNextNonSpaceOrZero(p, len, i + 1);
                            if (next == '\0')
                                return double.NaN;

                            // Existing spec:
                            // unary sign can be followed only by a digit or '('.
                            // ".5" is intentionally not supported.
                            if (!Helpers.IsDigit(next) && next != '(')
                                return double.NaN;

                            currentNumberHasUnaryMinus = (c == '-' && Helpers.IsDigit(next));

                            // Lower unary +/- into "0 +/- x".
                            values[vTop++] = 0.0;
                            ops[oTop++] = c;
                            prevType = Constants.PrevOp;
                            sawMeaningful = true;
                            continue;
                        }
                    }

                    // Binary operators must follow a number or ')'.
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
                    currentNumberHasUnaryMinus = false;
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
        // Core reduction
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
                // '%' is restricted to positive integer-valued operands.
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
                        double rounded = Math.Round(b);

                        // Use fast integer exponentiation only inside Int32 range.
                        // Otherwise, fall back to Math.Pow and allow Infinity.
                        if (rounded > int.MinValue && rounded <= int.MaxValue)
                        {
                            int e = (int)rounded;

                            if (e < 0)
                            {
                                // Note:
                                // e cannot be int.MinValue here because the range check above excludes it
                                // from causing overflow in negation after rounding cast logic.
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
                            result = Math.Pow(a, b);
                        }
                    }
                    else
                    {
                        // A negative base with a non-integer exponent is not a real number.
                        if (a < 0.0)
                            return false;

                        result = Math.Pow(a, b);
                    }
                }

                // Infinity is allowed by design; NaN is not.
                if (double.IsNaN(result))
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