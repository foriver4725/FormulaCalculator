using System;
using System.Runtime.CompilerServices;

namespace foriver4725.FormulaCalculator
{
    internal static class FormulaElement
    {
        internal const char N0 = '0';
        internal const char N1 = '1';
        internal const char N2 = '2';
        internal const char N3 = '3';
        internal const char N4 = '4';
        internal const char N5 = '5';
        internal const char N6 = '6';
        internal const char N7 = '7';
        internal const char N8 = '8';
        internal const char N9 = '9';
        internal const char OA = '+';
        internal const char OS = '-';
        internal const char OM = '*';
        internal const char OD = '/';
        internal const char OP = '^';
        internal const char PL = '(';
        internal const char PR = ')';
        internal const char NONE = ' ';

        internal enum Type : byte
        {
            Number = 0,
            Operator = 1,
            Paragraph = 2,
            None = 3,
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsValidChar(this char c) => c is
            (>= N0 and <= N9)
            or OA or OS or OM or OD or OP
            or PL or PR
            or NONE;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Type ToType(this char c) => c switch
        {
            N0 or N1 or N2 or N3 or N4 or N5 or N6 or N7 or N8 or N9 => Type.Number,
            OA or OS or OM or OD or OP => Type.Operator,
            PL or PR => Type.Paragraph,
            NONE => Type.None,
            _ => throw new ArgumentOutOfRangeException(nameof(c), $"Invalid character: {c}"),
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int ToInt(this char c) => c switch
        {
            N0   => 0,
            N1   => 1,
            N2   => 2,
            N3   => 3,
            N4   => 4,
            N5   => 5,
            N6   => 6,
            N7   => 7,
            N8   => 8,
            N9   => 9,
            _    => throw new ArgumentOutOfRangeException(nameof(c), $"Invalid character: {c}"),
        };
    }
}