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
                if (!IsWholeOK(RemoveNone_result)) return double.NaN;
                if (!IsNumberOK(RemoveNone_result, maxNumberDigit)) return double.NaN;
                if (!IsOperatorOK(RemoveNone_result)) return double.NaN;
                if (!IsParagraphOK(RemoveNone_result)) return double.NaN;
            }

            Span<int> ConnectNumbers_result = stackalloc int[RemoveNone_result.Length];
            int ConnectNumbers_resultLength = ConnectNumbers(RemoveNone_result, ConnectNumbers_result);
            ConnectNumbers_result = ConnectNumbers_result[..ConnectNumbers_resultLength];

            Span<double> ConvertToDouble_result = stackalloc double[ConnectNumbers_result.Length];
            ConvertToDouble(ConnectNumbers_result, ConvertToDouble_result);

            double result = CalculateFinally(ConvertToDouble_result);
            if (double.IsNaN(result)) return double.NaN;
            result = Math.Clamp(result, clampMin, clampMax);

            return result;
        }

        // Check if the formula does not contain any invalid characters
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsWholeOK(ReadOnlySpan<char> formula)
        {
            foreach (char e in formula)
                if (!e.IsValidChar())
                    return false;

            return true;
        }

        // Check if a number does not come immediately outside the parentheses
        // Check if there is no consecutive numbers exceeding a certain length
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsNumberOK(ReadOnlySpan<char> formula, byte maxNumberDigit)
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

            return true;
        }

        // Check for operators "+", "-" that "the previous element exists and is either a number, '(', or ')', or the previous element does not exist" and "the next element exists and is either a number or '('"
        // Check for operators other than "+", "-" that "the previous element exists and is either a number or ')'" and "the next element exists and is either a number or '('"
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsOperatorOK(ReadOnlySpan<char> formula)
        {
            for (int i = 0; i < formula.Length; i++)
            {
                char e = formula[i];

                if (e.ToType() != Element.Type.Operator) continue;

                if (e == Element.OA || e == Element.OS)
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

            return true;
        }

        // Check if all parentheses match and are in the correct order
        // Check if there is at least one number inside the parentheses
        // Check if the arrangement of ")(" does not exist
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsParagraphOK(ReadOnlySpan<char> formula)
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

        // Convert to a collection of real numbers
        // Also convert the IDs of symbols as they are
        // Write the result to 'result' (its length is assumed to be the same as source)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ConvertToDouble(ReadOnlySpan<int> source, Span<double> result)
        {
            for (int i = 0; i < source.Length; i++)
                result[i] = source[i];
        }

        // Finally calculate
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double CalculateFinally(ReadOnlySpan<double> source)
        {
            Span<double> sourceSpan = stackalloc double[source.Length];
            source.CopyTo(sourceSpan);

            return CalculateImpl(sourceSpan);
        }

        // Core logic of calculation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double CalculateImpl(ReadOnlySpan<double> source)
        {
            Span<double> _source = stackalloc double[source.Length];
            source.CopyTo(_source);

            // Remove parentheses

            // Search for "(" from the left
            int i = 0;
            while (i < _source.Length)
            {
                if (_source[i] != Element.ID_PL)
                {
                    i++;
                    continue;
                }

                // Search to the right and find the corresponding ")"
                int n = 0;
                for (int j = i + 1; j < _source.Length; j++)
                {
                    double e = _source[j];
                    if (e != Element.ID_PR)
                    {
                        if (e == Element.ID_PL) n++;
                        continue;
                    }

                    if (n >= 1)
                    {
                        n--;
                        continue;
                    }

                    // Recursively calculate the content inside "()" and update _source
                    {
                        // Calculate the content inside "()"
                        double value = CalculateImpl(_source[(i + 1)..j]);
                        if (double.IsNaN(value)) return double.NaN;

                        // Remove "()" and insert the calculation result into _source
                        int shrinkSize = j - i;
                        Span<double> newSpan = stackalloc double[_source.Length - shrinkSize];
                        _source.DeleteIndicesUnsafely(i + 1, shrinkSize, newSpan);
                        newSpan[i] = value;
                        _source = newSpan;
                    }

                    break;
                }
            }

            // There are no parentheses (or there were none to begin with), so perform arithmetic operations
            return CalculateRaw(_source);
        }

        // Calculate the expression assuming there are no parentheses
        //TODO: There are similar code blocks below, so consider refactoring them into a method.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double CalculateRaw(ReadOnlySpan<double> source)
        {
            Span<double> _source = stackalloc double[source.Length];
            source.CopyTo(_source);

            // Sign (positive/negative) of the first number
            double first = _source[0];
            if (first == Element.ID_OA)
            {
                _source = _source[1..];
            }
            else if (first == Element.ID_OS)
            {
                _source[1] = -_source[1];
                _source = _source[1..];
            }

            // Multiplication and division
            for (int i = 0; i < _source.Length; i++)
            {
                if (_source[i] == Element.ID_OM)
                {
                    double value = _source[i - 1] * _source[i + 1];
                    ReduceBinaryOperation(_source, i, value);
                    _source = _source[..^2];
                    i--;
                }
                else if (_source[i] == Element.ID_OD)
                {
                    if (_source[i + 1] == 0) return double.NaN;

                    double value = _source[i - 1] / _source[i + 1];
                    ReduceBinaryOperation(_source, i, value);
                    _source = _source[..^2];
                    i--;
                }
            }

            // Addition and subtraction
            for (int i = 0; i < _source.Length; i++)
            {
                if (_source[i] == Element.ID_OA)
                {
                    double value = _source[i - 1] + _source[i + 1];
                    ReduceBinaryOperation(_source, i, value);
                    _source = _source[..^2];
                    i--;
                }
                else if (_source[i] == Element.ID_OS)
                {
                    double value = _source[i - 1] - _source[i + 1];
                    ReduceBinaryOperation(_source, i, value);
                    _source = _source[..^2];
                    i--;
                }
            }

            return _source[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReduceBinaryOperation(Span<double> source, int operatorIndex, double operationResult)
        {
            Span<double> newSpan = stackalloc double[source.Length - 2];
            source.DeleteIndicesUnsafely(stackalloc[] { operatorIndex, operatorIndex + 1 }, newSpan);
            newSpan[operatorIndex - 1] = operationResult;
            newSpan.CopyTo(source);
        }
    }
}