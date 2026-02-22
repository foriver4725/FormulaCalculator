using System;
using System.Runtime.CompilerServices;

namespace foriver4725.FormulaCalculator
{
    using Element = FormulaElement;

    public static class FormulaCalculator
    {
        /// <summary>
        /// Calculate the formula given as a string and return the result as a double.<br/>
        /// - Supported characters: 0-9, +, -, *, /, (, ), and space (used for ignoring).<br/>
        /// </summary>
        /// <param name="formula"> The formula to be calculated.<br/></param>
        /// <returns> The result of the calculation as a double.<br/>
        /// If the formula is invalid or an error occurs during calculation (such as division by zero), returns double.NaN.<br/>
        /// The result can be abnormally large or small (such as when division by a very small number occurs),<br/>
        ///   so it is recommended to clamp the result if necessary.<br/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Calculate(this ReadOnlySpan<char> formula)
        {
            // Stacks for values and operators
            Span<double> values = stackalloc double[formula.Length];
            Span<char> ops = stackalloc char[formula.Length];

            // Tops of the stacks (the index of the next free slot = the length of the stack)
            int vTop = 0;
            int oTop = 0;

            // Build an integer while reading consecutive digits.
            // -1 means "not building a number now".
            int connectedNumber = -1;

            // Track the previous meaningful token to detect unary operators (+ and -)
            // 0: start, 1: number, 2: '(', 3: ')', 4: operator
            byte prevType = 0;

            for (int i = 0; i < formula.Length; i++)
            {
                char c = formula[i];

                // Ignore spaces
                if (c == Element.NONE)
                    continue;

                // If the token is a number, build it
                if (c is (>= Element.N0 and <= Element.N9))
                {
                    int digit = c - Element.N0;
                    connectedNumber = (connectedNumber == -1) ? digit : (connectedNumber * 10 + digit);
                    prevType = 1;
                    continue;
                }

                // If we were building a number, push it to the value stack now
                if (connectedNumber != -1)
                {
                    values[vTop++] = connectedNumber;
                    connectedNumber = -1;
                }

                // If the token is "("
                if (c == Element.PL)
                {
                    ops[oTop++] = Element.PL;
                    prevType = 2;
                    continue;
                }

                // If the token is ")"
                if (c == Element.PR)
                {
                    while (oTop > 0 && ops[oTop - 1] is not Element.PL)
                    {
                        if (!ApplyTop(values, vTop, ops, oTop))
                            return double.NaN;

                        vTop--;
                        oTop--;
                    }

                    if (oTop == 0)
                        return double.NaN; // unmatched

                    oTop--; // remove '('
                    prevType = 3;
                    continue;
                }

                // Now the token must be an operator (+ - * /)

                // Check for unary operators (+ and -)
                // Unary is valid at the beginning or right after '('
                bool isUnary =
                    (c is Element.OA or Element.OS) &&
                    (prevType is (0 or 2));

                if (isUnary)
                {
                    // Look ahead ignoring spaces
                    int j = i + 1;
                    while (j < formula.Length && formula[j] == Element.NONE) j++;
                    if (j >= formula.Length) return double.NaN;

                    // If the next token is "(",
                    // treat unary as multiplying by ±1
                    if (formula[j] == Element.PL)
                    {
                        values[vTop++] = (c is Element.OS) ? -1 : 1;
                        ops[oTop++] = Element.OM; // implicit multiplication
                        prevType = 4;
                        continue;
                    }

                    // Otherwise the next token must be a number; push signed number
                    // We do not consume the digits here; we let the main loop build them.
                    // Push 0 and use binary op to simulate unary: 0 ± number
                    values[vTop++] = 0;
                    ops[oTop++] = c; // + or -
                    prevType = 4;
                    continue;
                }

                // Process operator (+ - * / ^)
                while (oTop > 0 && ShouldReduce(ops[oTop - 1], c))
                {
                    if (!ApplyTop(values, vTop, ops, oTop))
                        return double.NaN;

                    vTop--;
                    oTop--;
                }

                ops[oTop++] = c;
                prevType = 4;
            }

            // Push the last connected number if exists
            if (connectedNumber != -1)
            {
                values[vTop++] = connectedNumber;
            }

            // Final evaluation
            while (oTop > 0)
            {
                if (!ApplyTop(values, vTop, ops, oTop))
                    return double.NaN;

                vTop--;
                oTop--;
            }

            return (vTop == 1) ? values[0] : double.NaN;
        }

        // Apply the operator at the top of the operator stack to the top two values on the value stack
        // Pop the operator and the two values, compute the result, and push it back to the value stack
        // Returns false if an error occurs (such as division by zero), otherwise true.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ApplyTop(
            Span<double> values, int vTop,
            Span<char> ops, int oTop
        )
        {
            char op = ops[oTop - 1];
            double b = values[vTop - 1];
            double a = values[vTop - 2];

            double result;

            if (op is Element.OA)
                result = a + b;
            else if (op is Element.OS)
                result = a - b;
            else if (op is Element.OM)
                result = a * b;
            else if (op is Element.OD)
            {
                if (b == 0)
                    return false;
                result = a / b;
            }
            else if (op is Element.OP)
            {
                // 0^b : only allowed if b > 0
                if (a == 0)
                {
                    if (b <= 0)
                        return false;
                    result = 0;
                }
                else
                {
                    if (IsInteger(b))
                    {
                        int e = (int)Math.Round(b);

                        if (e < 0)
                        {
                            // a != 0, so we can safely compute a^(-e) and take reciprocal
                            result = 1.0 / PowInt(a, -e);
                        }
                        else
                        {
                            result = PowInt(a, e);
                        }
                    }
                    else
                    {
                        // If the exponent is not an integer, we can directly use Math.Pow,
                        // but we need to check for negative base with non-integer exponent, which is not allowed in real numbers.
                        if (a < 0) return false;
                        result = Math.Pow(a, b);
                    }
                }
            }
            else
                return false;

            values[vTop - 2] = result;
            return true;
        }

        // Fast integer power function using exponentiation by squaring.
        // This is used to optimize cases like x^3, x^4, etc., which are common in formulas.
        // The exponent must be a non-negative integer.
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

        // Return the precedence of the operator. Higher value means higher precedence.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Precedence(char op) => op switch
        {
            Element.OA or Element.OS => 1,
            Element.OM or Element.OD => 2,
            Element.OP               => 3,
            _                        => 0,
        };

        // Return true if the operator on the stack should be reduced before pushing the incoming operator.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ShouldReduce(char stackOp, char incomingOp)
        {
            int sp = Precedence(stackOp);
            int ip = Precedence(incomingOp);

            if (sp > ip) return true;
            if (sp < ip) return false;

            // Same precedence: reduce only if the incoming operator is left-associative.
            // '^' is right-associative.
            return !IsRightAssociative(incomingOp);
        }

        // Return true if the operator is right-associative (such as '^'), otherwise false (such as '+', '-', '*', '/').
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsRightAssociative(char op)
            => op is Element.OP; // '^'

        // Return true if the double is an integer (within a small tolerance), otherwise false.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsInteger(double x)
            => Math.Abs(x - Math.Round(x)) < 1e-12;
    }
}