using System;
using System.Runtime.CompilerServices;

namespace foriver4725.FormulaCalculator
{
    internal static class SpanExtension
    {
        // Delete specified indices.
        // Due to the priority of extension methods, span is not ReadOnlySpan.
        // The length of result must be span.Length - indices.Length.
        // indices must be sorted in ascending order and have no duplicates.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void DeleteIndicesUnsafely<T>(this Span<T> span, ReadOnlySpan<int> indices, Span<T> result)
        {
            // Checking value
            int indicesIndex = 0;
            int targetIndex = indices[indicesIndex];

            for (int i = 0; i < span.Length; i++)
            {
                if (i == targetIndex)
                {
                    indicesIndex++;
                    if (indicesIndex < indices.Length)
                        targetIndex = indices[indicesIndex];
                }
                else
                {
                    result[i - indicesIndex] = span[i];
                }
            }
        }

        // Delete specified indices. ( [beginIndex, beginIndex + length) )
        // Due to the priority of extension methods, span is not ReadOnlySpan.
        // The length of result must be span.Length - indices.Length.
        // indices must be sorted in ascending order and have no duplicates.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void DeleteIndicesUnsafely<T>(this Span<T> span, int beginIndex, int length, Span<T> result)
        {
            Span<int> indices = stackalloc int[length];
            for (int i = beginIndex; i < beginIndex + length; i++)
                indices[i - beginIndex] = i;

            span.DeleteIndicesUnsafely(indices, result);
        }
    }
}
