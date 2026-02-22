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
        public static double Calculate(this ReadOnlySpan<char> formula)
        {
            // Stacks for values and operators
            Span<double> values = stackalloc double[formula.Length];
            Span<double> ops = stackalloc double[formula.Length];

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
                if (c.ToType() == Element.Type.Number)
                {
                    int digit = c.ToInt(); // 0..9
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
                    ops[oTop++] = Element.ID_PL;
                    prevType = 2;
                    continue;
                }

                // If the token is ")"
                if (c == Element.PR)
                {
                    while (oTop > 0 && ops[oTop - 1] is not Element.ID_PL)
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
                int token = c.ToInt();

                // Check for unary operators (+ and -)
                // Unary is valid at the beginning or right after '('
                bool isUnary =
                    (token is Element.ID_OA or Element.ID_OS) &&
                    (prevType == 0 || prevType == 2);

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
                        values[vTop++] = (token is Element.ID_OS) ? -1 : 1;
                        ops[oTop++] = Element.ID_OM; // implicit multiplication
                        prevType = 4;
                        continue;
                    }

                    // Otherwise the next token must be a number; push signed number
                    // We do not consume the digits here; we let the main loop build them.
                    // Push 0 and use binary op to simulate unary: 0 ± number
                    values[vTop++] = 0;
                    ops[oTop++] = token; // + or -
                    prevType = 4;
                    continue;
                }

                // Process operator (+ - * /)
                while (oTop > 0 && Precedence(ops[oTop - 1]) >= Precedence(token))
                {
                    if (!ApplyTop(values, vTop, ops, oTop))
                        return double.NaN;

                    vTop--;
                    oTop--;
                }

                ops[oTop++] = token;
                prevType = 4;
            }

            // Push the last connected number if exists
            if (connectedNumber != -1)
            {
                values[vTop++] = connectedNumber;
                connectedNumber = -1;
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

        // Check if the formula does not contain any invalid characters
        // You can use this method to validate the formula before calling Calculate()
        // to avoid unnecessary calculations on invalid formulas.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidFormula(this ReadOnlySpan<char> formulaArg, byte maxNumberDigit = 8)
        {
            // Remove 'None' and compress
            Span<char> formula = stackalloc char[formulaArg.Length];
            {
                int length = 0;
                foreach (char e in formulaArg)
                {
                    if (e == Element.NONE)
                        continue;
                    formula[length++] = e;
                }

                formula = formula[..length];
            }

            // [Is Whole OK?]
            // Check if the formula does not contain any invalid characters,
            // but contains at least one valid character (to prevent empty formula)
            {
                if (formula.IsEmpty)
                    return false;

                foreach (char e in formula)
                    if (!e.IsValidChar())
                        return false;
            }

            // [Is Number OK?]
            // Check if a number does not come immediately outside the parentheses
            // Check if there is no consecutive numbers exceeding a certain length
            {
                for (int i = 0; i < formula.Length - 1; i++)
                {
                    char e = formula[i], f = formula[i + 1];
                    if (e.ToType() == Element.Type.Number && f == Element.PL) return false;
                    if (e == Element.PR && f.ToType() == Element.Type.Number) return false;
                }

                byte continuousCount = 0;
                foreach (char e in formula)
                {
                    if (e.ToType() != Element.Type.Number)
                    {
                        continuousCount = 0;
                        continue;
                    }

                    if (++continuousCount > maxNumberDigit) return false;
                }
            }

            // [Is Operator OK?]
            // Check for operators "+", "-" that
            //   "the previous element exists and is either a number, '(', or ')', or the previous element does not exist" and
            //   "the next element exists and is either a number or '('"
            // Check for operators other than "+", "-" that
            //   "the previous element exists and is either a number or ')'" and
            //   "the next element exists and is either a number or '('"
            {
                for (int i = 0; i < formula.Length; i++)
                {
                    char e = formula[i];

                    if (e.ToType() != Element.Type.Operator) continue;

                    if (e is (Element.OA or Element.OS))
                    {
                        if (i > 0)
                        {
                            char left = formula[i - 1];
                            if (left.ToType() != Element.Type.Number && left != Element.PL && left != Element.PR)
                                return false;
                        }

                        if (i < formula.Length - 1)
                        {
                            char right = formula[i + 1];
                            if (right.ToType() != Element.Type.Number && right != Element.PL) return false;
                        }
                        else return false;
                    }
                    else
                    {
                        if (i > 0)
                        {
                            char left = formula[i - 1];
                            if (left.ToType() != Element.Type.Number && left != Element.PR) return false;
                        }
                        else return false;

                        if (i < formula.Length - 1)
                        {
                            char right = formula[i + 1];
                            if (right.ToType() != Element.Type.Number && right != Element.PL) return false;
                        }
                        else return false;
                    }
                }
            }

            // [Is Paragraph OK?]
            // Check if all parentheses match and are in the correct order
            // Check if there is at least one number inside the parentheses
            // Check if the arrangement of ")(" does not exist
            {
                int n = 0;
                foreach (char e in formula)
                {
                    if (e == Element.PL) n++;
                    else if (e == Element.PR) n--;

                    if (n < 0) return false;
                }

                if (n != 0) return false;

                for (int i = 0; i < formula.Length; i++)
                {
                    if (formula[i] == Element.PL)
                    {
                        int j = i + 1;
                        while (j < formula.Length)
                        {
                            if (formula[j] == Element.PR) break;
                            j++;
                        }

                        bool hasNumber = false;
                        for (int k = i, _n = 0; _n < j - i + 1; k++, _n++)
                        {
                            char e = formula[k];
                            if (e.ToType() == Element.Type.Number)
                            {
                                hasNumber = true;
                                break;
                            }
                        }

                        if (!hasNumber) return false;
                    }
                }

                for (int i = 0; i < formula.Length - 1; i++)
                {
                    char e = formula[i], f = formula[i + 1];
                    if (e == Element.PR && f == Element.PL) return false;
                }
            }

            return true;
        }

        // Apply the operator at the top of the operator stack to the top two values on the value stack
        // Pop the operator and the two values, compute the result, and push it back to the value stack
        // Returns false if an error occurs (such as division by zero), otherwise true.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ApplyTop(
            Span<double> values, int vTop,
            Span<double> ops, int oTop
        )
        {
            double op = ops[oTop - 1];
            double b = values[vTop - 1];
            double a = values[vTop - 2];

            double result;

            if (op is Element.ID_OA)
                result = a + b;
            else if (op is Element.ID_OS)
                result = a - b;
            else if (op is Element.ID_OM)
                result = a * b;
            else if (op is Element.ID_OD)
            {
                if (b == 0) return false;
                result = a / b;
            }
            else
                return false;

            values[vTop - 2] = result;
            return true;
        }

        // Return the precedence of the operator. Higher value means higher precedence.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Precedence(double op) => op switch
        {
            Element.ID_OA or Element.ID_OS => 1,
            Element.ID_OM or Element.ID_OD => 2,
            _                              => 0,
        };
    }
}