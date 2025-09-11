using UnityEngine;
using UnityEditor;

namespace foriver4725.FormulaCalculator.Profiling
{
    internal sealed class GameManager : MonoBehaviour
    {
        [SerializeField] private bool doSkipValidation;

        private const string ProfilerName = "### FormulaCalculator.Calculate() ###";
        private const string Formula = "1+2*3/(4-5)";
        private const ulong LoopAmount = 1_000_000;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Warmup
                for (int i = 0; i < 8; i++)
                    _ = Formula.Calculate(doSkipValidation: doSkipValidation);

                UnityEngine.Profiling.Profiler.BeginSample(ProfilerName);

                for (ulong i = 0; i < LoopAmount; i++)
                    _ = Formula.Calculate(doSkipValidation: doSkipValidation);

                UnityEngine.Profiling.Profiler.EndSample();

                EditorApplication.isPlaying = false;
            }
        }
    }
}
