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

                // Calculate() assumes IsValidFormula() has already accepted the syntax.
                // This flag is only needed to distinguish unary +/- after '(' from binary +/-.
                bool previousWasLeftParen = false;

                for (int i = 0; i < len; i++)
                {
                    char c = p[i];

                    if (c == ' ')
                        continue;

                    // Number token
                    if (Helpers.IsDigit(c))
                    {
                        double number;
                        int end = Helpers.ReadNumberOrMinusOne(p, len, i, &number);
                        if (end < 0)
                            return double.NaN;

                        values[vTop++] = number;
                        i = end;
                        previousWasLeftParen = false;
                        continue;
                    }

                    // Left parenthesis
                    if (c == '(')
                    {
                        ops[oTop++] = c;
                        previousWasLeftParen = true;
                        continue;
                    }

                    // Right parenthesis
                    if (c == ')')
                    {
                        while (oTop > 0 && ops[oTop - 1] != '(')
                        {
                            if (!ApplyTop(values, ref vTop, ops, ref oTop))
                                return double.NaN;
                        }

                        // Internal safety:
                        // valid syntax should always have a matching '(' here.
                        if (oTop == 0)
                            return double.NaN;

                        oTop--; // pop '('
                        previousWasLeftParen = false;
                        continue;
                    }

                    // Unary +/- is valid only immediately after '('.
                    // If it is attached to a number, read it as one signed numeric token.
                    if ((c == '+' || c == '-') && previousWasLeftParen)
                    {
                        char next = Helpers.PeekNextNonSpaceOrZero(p, len, i + 1);

                        if (Helpers.IsDigit(next))
                        {
                            double number;
                            int end = Helpers.ReadSignedNumberOrMinusOne(p, len, i, &number);
                            if (end < 0)
                                return double.NaN;

                            values[vTop++] = number;
                            i = end;
                            previousWasLeftParen = false;
                            continue;
                        }

                        // Unary +/- before a parenthesized expression:
                        //   (+(...)) -> (0 + (...))
                        //   (-(...)) -> (0 - (...))
                        //
                        // IsValidFormula() guarantees that the next meaningful char is '('.
                        values[vTop++] = 0.0;
                    }

                    // Binary operator, or lowered unary +/- before a parenthesized expression.
                    while (oTop > 0 && Helpers.ShouldReduce(ops[oTop - 1], c))
                    {
                        if (!ApplyTop(values, ref vTop, ops, ref oTop))
                            return double.NaN;
                    }

                    ops[oTop++] = c;
                    previousWasLeftParen = false;
                }

                // Evaluate remaining operators.
                while (oTop > 0)
                {
                    // Internal safety:
                    // valid syntax should not leave '(' on the operator stack.
                    if (ops[oTop - 1] == '(')
                        return double.NaN;

                    if (!ApplyTop(values, ref vTop, ops, ref oTop))
                        return double.NaN;
                }

                // Internal safety:
                // a valid expression should reduce to exactly one value.
                return vTop == 1 ? values[0] : double.NaN;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe bool ApplyTop(double* values, ref int vTop, char* ops, ref int oTop)
        {
            // Internal safety against stack underflow.
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
                if (a == 0.0)
                {
                    // 0^b is valid only when b is positive.
                    if (b <= 0.0)
                        return false;

                    result = 0.0;
                }
                else if (Helpers.IsInteger(b))
                {
                    double rounded = Math.Round(b);

                    // Use fast integer exponentiation only inside Int32 range.
                    // Otherwise, fall back to Math.Pow and allow Infinity.
                    if (rounded > int.MinValue && rounded <= int.MaxValue)
                    {
                        int e = (int)rounded;

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
