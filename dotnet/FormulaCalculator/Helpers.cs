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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe bool TryPeekNextNonSpace(char* p, int len, int start, out char next)
        {
            for (int i = start; i < len; i++)
            {
                char c = p[i];
                if (c == ' ')
                    continue;

                next = c;
                return true;
            }

            next = '\0';
            return false;
        }
    }
}