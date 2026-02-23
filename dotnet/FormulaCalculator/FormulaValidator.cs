using System;
using System.Runtime.CompilerServices;

namespace foriver4725.FormulaCalculator
{
    public static class FormulaValidator
    {
        private enum ElementType : byte
        {
            Number = 0,
            Operator = 1,
            Paragraph = 2,
            None = 3,
            Invalid = 255,
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ElementType GetElementType(this char c) => c switch
        {
            >= '0' and <= '9'               => ElementType.Number,
            '+' or '-' or '*' or '/' or '^' => ElementType.Operator,
            '(' or ')'                      => ElementType.Paragraph,
            ' '                             => ElementType.None,
            _                               => ElementType.Invalid,
        };

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
                    if (e is ' ')
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
                    if (e.GetElementType() is ElementType.Invalid)
                        return false;
            }

            // [Is Number OK?]
            // Check if a number does not come immediately outside the parentheses
            // Check if there is no consecutive numbers exceeding a certain length
            {
                for (int i = 0; i < formula.Length - 1; i++)
                {
                    char e = formula[i], f = formula[i + 1];

                    if (e.GetElementType() is ElementType.Number && f is '(')
                        return false;
                    if (e is ')' && f.GetElementType() is ElementType.Number)
                        return false;
                }

                byte continuousCount = 0;
                foreach (char e in formula)
                {
                    if (e.GetElementType() is not ElementType.Number)
                    {
                        continuousCount = 0;
                        continue;
                    }

                    if (++continuousCount > maxNumberDigit)
                        return false;
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

                    if (e.GetElementType() is not ElementType.Operator)
                        continue;

                    if (e is ('+' or '-'))
                    {
                        if (i > 0)
                        {
                            char left = formula[i - 1];
                            if (left.GetElementType() is not ElementType.Number
                                && left is not ('(' or ')'))
                                return false;
                        }

                        if (i < formula.Length - 1)
                        {
                            char right = formula[i + 1];
                            if (right.GetElementType() is not ElementType.Number
                                && right is not '(')
                                return false;
                        }
                        else return false;
                    }
                    else
                    {
                        if (i > 0)
                        {
                            char left = formula[i - 1];
                            if (left.GetElementType() is not ElementType.Number
                                && left is not ')')
                                return false;
                        }
                        else return false;

                        if (i < formula.Length - 1)
                        {
                            char right = formula[i + 1];
                            if (right.GetElementType() is not ElementType.Number
                                && right is not '(')
                                return false;
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
                    if (e is '(') n++;
                    else if (e is ')') n--;

                    if (n < 0) return false;
                }

                if (n != 0) return false;

                for (int i = 0; i < formula.Length; i++)
                {
                    if (formula[i] is '(')
                    {
                        int j = i + 1;
                        while (j < formula.Length)
                        {
                            if (formula[j] is ')') break;
                            j++;
                        }

                        bool hasNumber = false;
                        for (int k = i, _n = 0; _n < j - i + 1; k++, _n++)
                        {
                            char e = formula[k];
                            if (e.GetElementType() is ElementType.Number)
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
                    if (e is ')' && f is '(') return false;
                }
            }

            return true;
        }
    }
}