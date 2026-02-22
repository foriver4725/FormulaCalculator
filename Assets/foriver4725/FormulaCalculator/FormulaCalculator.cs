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
        /// <param name="formula"> The formula to be calculated. </param>
        /// <param name="clampMin"> The minimum value to clamp the result to. </param>
        /// <param name="clampMax"> The maximum value to clamp the result to. </param>
        /// <param name="doSkipValidation"> If true, skips validation checks. (Be careful!) </param>
        /// <param name="maxNumberDigit"> Valid when doSkipValidation is false.<br/>The maximum number of digits allowed when concatenating numbers (as long as it remains within the range of int). </param>
        /// <returns> The result of the calculation as a double. If the formula is invalid or an error occurs during calculation (such as division by zero), returns double.NaN. </returns>
        public static double Calculate(
            this ReadOnlySpan<char> formula,
            double clampMin = short.MinValue,
            double clampMax = short.MaxValue,
            bool doSkipValidation = false,
            byte maxNumberDigit = 8
        )
        {
            Span<char> RemoveNone_result = stackalloc char[formula.Length];
            int RemoveNone_resultLength = RemoveNone(formula, RemoveNone_result);
            if (RemoveNone_resultLength <= 0) return double.NaN; // Empty formula
            RemoveNone_result = RemoveNone_result[..RemoveNone_resultLength];

            if (!doSkipValidation)
            {
                if (!IsValidFormula(RemoveNone_result, maxNumberDigit))
                    return double.NaN;
            }

            Span<int> ConnectNumbers_result = stackalloc int[RemoveNone_result.Length];
            int ConnectNumbers_resultLength = ConnectNumbers(RemoveNone_result, ConnectNumbers_result);
            ConnectNumbers_result = ConnectNumbers_result[..ConnectNumbers_resultLength];

            double result = CalculateImpl(ConnectNumbers_result);
            if (double.IsNaN(result)) return double.NaN;
            result = Math.Clamp(result, clampMin, clampMax);

            return result;
        }

        // Check if the formula does not contain any invalid characters
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidFormula(ReadOnlySpan<char> formula, byte maxNumberDigit)
        {
            // [Is Whole OK?]
            // Check if the formula does not contain any invalid characters
            {
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

        // Remove 'None' and compress
        // Write the result to 'result' and return its length
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int RemoveNone(ReadOnlySpan<char> source, Span<char> result)
        {
            int length = 0;
            foreach (char e in source)
            {
                if (e == Element.NONE) continue;
                result[length++] = e;
            }

            return length;
        }

        // Connect numbers to form a collection of integers
        // Write the result to 'result' and return its length
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ConnectNumbers(ReadOnlySpan<char> source, Span<int> result)
        {
            int connectedNumber = -1;

            int length = 0;
            for (int i = 0; i < source.Length; i++)
            {
                char e = source[i];
                int eAsInt = e.ToInt();

                if (e.ToType() != Element.Type.Number)
                {
                    if (connectedNumber != -1)
                    {
                        result[length++] = connectedNumber;
                        connectedNumber = -1;
                    }

                    result[length++] = eAsInt;
                    continue;
                }

                if (connectedNumber == -1) connectedNumber = eAsInt;
                else connectedNumber = connectedNumber * 10 + eAsInt;
            }

            if (connectedNumber != -1)
                result[length++] = connectedNumber;

            return length;
        }

        // Finally calculate
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double CalculateImpl(ReadOnlySpan<int> source)
        {
            // Stacks for values and operators
            Span<double> values = stackalloc double[source.Length];
            Span<double> ops = stackalloc double[source.Length];

            // Tops of the stacks (the index of the next free slot = the length of the stack)
            int vTop = 0;
            int oTop = 0;

            int i = 0;

            while (i < source.Length)
            {
                double token = source[i];

                // Check for unary operators (+ and -)
                bool isUnary =
                    (token is (Element.ID_OA or Element.ID_OS)) &&
                    (i == 0 || source[i - 1] is Element.ID_PL);
                if (isUnary)
                {
                    i++;

                    // If the next token is "(",
                    // treat unary as multiplying by ±1
                    if (source[i] is Element.ID_PL)
                    {
                        values[vTop++] = (token is Element.ID_OS) ? -1 : 1;
                        ops[oTop++] = Element.ID_OM; // implicit multiplication
                        continue;
                    }

                    double value = source[i++];

                    if (token is Element.ID_OS)
                        value = -value;

                    values[vTop++] = value;
                    continue;
                }

                // If the token is a number, push it to the value stack
                if (token.IsNumber())
                {
                    values[vTop++] = token;
                    i++;
                    continue;
                }

                // If the token is "("
                if (token is Element.ID_PL)
                {
                    ops[oTop++] = token;
                    i++;
                    continue;
                }

                // If the token is ")"
                if (token is Element.ID_PR)
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
                    i++;
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
                i++;
            }

            // Final evaluation
            while (oTop > 0)
            {
                if (!ApplyTop(values, vTop, ops, oTop))
                    return double.NaN;

                vTop--;
                oTop--;
            }

            return values[0];
        }

        // Apply the operator at the top of the operator stack to the top two values on the value stack
        // Pop the operator and the two values, compute the result, and push it back to the value stack
        // Returns false if an error occurs (such as division by zero), otherwise true.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ApplyTop(
            Span<double> values, int vTop,
            Span<double> ops, int oTop)
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