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
        internal const char PL = '(';
        internal const char PR = ')';
        internal const char NONE = ' ';

        // Assign values that are unlikely to appear in calculations as IDs for symbols.
        internal const int ID_OA = 0x7fffffff;
        internal const int ID_OS = 0x7ffffffe;
        internal const int ID_OM = 0x7ffffffd;
        internal const int ID_OD = 0x7ffffffc;
        internal const int ID_PL = 0x7ffffffb;
        internal const int ID_PR = 0x7ffffffa;
        internal const int ID_NONE = 0x7ffffff9;

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
            or OA or OS or OM or OD
            or PL or PR
            or NONE;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Type ToType(this char c) => c switch
        {
            N0 or N1 or N2 or N3 or N4 or N5 or N6 or N7 or N8 or N9 => Type.Number,
            OA or OS or OM or OD => Type.Operator,
            PL or PR => Type.Paragraph,
            NONE => Type.None,
            _ => throw new ArgumentOutOfRangeException(nameof(c), $"Invalid character: {c}"),
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int ToInt(this char c) => c switch
        {
            N0 => 0,
            N1 => 1,
            N2 => 2,
            N3 => 3,
            N4 => 4,
            N5 => 5,
            N6 => 6,
            N7 => 7,
            N8 => 8,
            N9 => 9,
            OA => ID_OA,
            OS => ID_OS,
            OM => ID_OM,
            OD => ID_OD,
            PL => ID_PL,
            PR => ID_PR,
            NONE => ID_NONE,
            _ => throw new ArgumentOutOfRangeException(nameof(c), $"Invalid character: {c}"),
        };
    }
}
