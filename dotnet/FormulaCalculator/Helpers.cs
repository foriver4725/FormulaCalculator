using System;
using System.Runtime.CompilerServices;

namespace foriver4725.FormulaCalculator
{
    internal static class Helpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsDigit(char c)
            => (uint)(c - '0') <= 9u;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsOperator(char c)
            => c == '+' || c == '-' || c == '*' || c == '/' || c == '%' || c == '^';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Precedence(char op)
        {
            if (op == '+' || op == '-') return 1;
            if (op == '*' || op == '/' || op == '%') return 2;
            if (op == '^') return 3;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsRightAssociative(char op)
            => op == '^';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool ShouldReduce(char stackOp, char incomingOp)
        {
            if (stackOp == '(')
                return false;

            int sp = Precedence(stackOp);
            int ip = Precedence(incomingOp);

            if (sp > ip) return true;
            if (sp < ip) return false;

            return !IsRightAssociative(incomingOp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsInteger(double x)
            => Math.Abs(x - Math.Round(x)) < 1.0e-12;

        // Returns the next non-space character, or '\0' if none exists.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe char PeekNextNonSpaceOrZero(char* p, int len, int start)
        {
            for (int i = start; i < len; i++)
            {
                char c = p[i];
                if (c != ' ')
                    return c;
            }

            return '\0';
        }

        // Validator-only reader:
        // validates the number token shape without constructing a double.
        //
        // Supported:
        //   123
        //   123.456
        //
        // Rejected:
        //   .5
        //   1.
        //   1.2.3
        //
        // Returns:
        //   end index of the number token on success
        //   -1 on failure
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe int SkipNumberTokenOrMinusOne(char* p, int len, int start)
        {
            int idx = start;

            if (idx >= len || !IsDigit(p[idx]))
                return -1;

            // Integer part
            do
            {
                idx++;
            } while (idx < len && IsDigit(p[idx]));

            // Integer-only form
            if (idx >= len || p[idx] != '.')
                return idx - 1;

            // Decimal point requires at least one digit after it
            idx++;
            if (idx >= len || !IsDigit(p[idx]))
                return -1;

            // Fractional part
            do
            {
                idx++;
            } while (idx < len && IsDigit(p[idx]));

            return idx - 1;
        }

        // Calculator-only reader:
        // parses the number token directly into a double without allocations.
        //
        // Supported:
        //   123
        //   123.456
        //
        // Rejected:
        //   .5
        //   1.
        //   1.2.3
        //
        // Returns:
        //   end index of the number token on success
        //   -1 on failure
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe int ReadNumberOrMinusOne(char* p, int len, int start, double* outValue)
        {
            int idx = start;

            if (idx >= len || !IsDigit(p[idx]))
                return -1;

            double integerPart = 0.0;

            // Integer part
            do
            {
                integerPart = integerPart * 10.0 + (p[idx] - '0');
                idx++;
            } while (idx < len && IsDigit(p[idx]));

            // Integer-only form
            if (idx >= len || p[idx] != '.')
            {
                *outValue = integerPart;
                return idx - 1;
            }

            // Decimal point requires at least one digit after it
            idx++;
            if (idx >= len || !IsDigit(p[idx]))
                return -1;

            double fractionalPart = 0.0;
            double scale = 1.0;

            // Fractional part
            do
            {
                fractionalPart = fractionalPart * 10.0 + (p[idx] - '0');
                scale *= 10.0;
                idx++;
            } while (idx < len && IsDigit(p[idx]));

            *outValue = integerPart + (fractionalPart / scale);
            return idx - 1;
        }
    }
}