using System;
using System.Runtime.CompilerServices;

namespace foriver4725.FormulaCalculator
{
    public static class FormulaValidator
    {
        // NOTE:
        // - This validator intentionally ignores spaces (it compresses the input).
        // - It does NOT support decimals ('.' / ',') in the current spec.
        // - Behavior is kept identical to the original implementation for the provided tests.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsDigit(char c) => (uint)(c - '0') <= 9u;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsOperator(char c)
            => c is '+' or '-' or '*' or '/' or '^';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsParen(char c)
            => c is '(' or ')';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidFormula(this ReadOnlySpan<char> formulaArg, byte maxNumberDigit = 8)
        {
            // Compress: remove spaces into a stack buffer.
            // During compression we also:
            // - Validate charset
            // - Check digit run length
            // - Check adjacency rules around parentheses
            // - Validate parentheses order + "contains at least one digit" rule in O(n) using a stack
            int srcLen = formulaArg.Length;
            if (srcLen == 0)
                return false;

            Span<char> buf = stackalloc char[srcLen];

            // Stack for "this parenthesis group (and its descendants) contains at least one digit"
            // Depth never exceeds compressed length, so srcLen is safe.
            Span<byte> parenHasDigit = stackalloc byte[srcLen];

            int len = 0;
            int depth = 0;
            int digitRun = 0;

            // We track previous *compressed* char to validate adjacency in the compressed domain.
            char prev = '\0';
            bool hasPrev = false;

            for (int i = 0; i < srcLen; i++)
            {
                char c = formulaArg[i];
                if (c == ' ')
                    continue;

                // Charset validation (fast path branching)
                bool isDigit = IsDigit(c);
                bool isOp = !isDigit && IsOperator(c);
                bool isParen = !isDigit && !isOp && IsParen(c);

                if (!isDigit && !isOp && !isParen)
                    return false;

                // Adjacency checks with parentheses (in compressed form)
                // - "2(" is invalid
                // - ")2" is invalid
                // - ")(" is invalid
                if (hasPrev)
                {
                    if (IsDigit(prev) && c == '(') return false;
                    if (prev == ')' && IsDigit(c)) return false;
                    if (prev == ')' && c == '(') return false;
                }

                // Digit run length (maxNumberDigit)
                if (isDigit)
                {
                    digitRun++;
                    if (digitRun > maxNumberDigit)
                        return false;

                    // Mark digit existence for parentheses groups (O(1) amortized):
                    // We mark the innermost group, and propagate "has digit" upward when closing.
                    if (depth > 0)
                        parenHasDigit[depth - 1] = 1;
                }
                else
                {
                    digitRun = 0;
                }

                // Parentheses validation + "must contain at least one digit" rule in O(n)
                if (isParen)
                {
                    if (c == '(')
                    {
                        // push
                        parenHasDigit[depth] = 0;
                        depth++;
                    }
                    else
                    {
                        // ')': must have a matching '('
                        if (depth <= 0)
                            return false;

                        // pop
                        byte childHasDigit = parenHasDigit[depth - 1];
                        if (childHasDigit == 0)
                            return false;

                        depth--;

                        // Propagate digit existence to parent group.
                        // This preserves the original behavior where digits inside nested parentheses
                        // also satisfy the "outer parentheses contains at least one digit" requirement.
                        if (depth > 0)
                            parenHasDigit[depth - 1] = 1;
                    }
                }

                buf[len++] = c;
                prev = c;
                hasPrev = true;
            }

            // Empty after compression => invalid
            if (len == 0)
                return false;

            // Unmatched '(' remaining => invalid
            if (depth != 0)
                return false;

            ReadOnlySpan<char> formula = buf[..len];

            // Operator placement validation (same rules as the original)
            for (int i = 0; i < len; i++)
            {
                char op = formula[i];
                if (!IsOperator(op))
                    continue;

                // Right side must exist for every operator
                if (i >= len - 1)
                    return false;

                char right = formula[i + 1];

                if (op is '+' or '-')
                {
                    // Left side: may be absent, or must be Number / '(' / ')'
                    if (i > 0)
                    {
                        char left = formula[i - 1];
                        if (!IsDigit(left) && left != '(' && left != ')')
                            return false;
                    }

                    // Right side: must be Number or '('
                    if (!IsDigit(right) && right != '(')
                        return false;
                }
                else
                {
                    // For *, /, ^ : left side must exist and be Number or ')'
                    if (i == 0)
                        return false;

                    char left = formula[i - 1];
                    if (!IsDigit(left) && left != ')')
                        return false;

                    // Right side: must be Number or '('
                    if (!IsDigit(right) && right != '(')
                        return false;
                }
            }

            return true;
        }
    }
}