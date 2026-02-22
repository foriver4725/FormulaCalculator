using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace foriver4725.FormulaCalculator.Profiling
{
    internal sealed class GameManager : MonoBehaviour
    {
        [SerializeField] private TMP_InputField formulaInputField;
        [SerializeField] private Slider loopAmountSlider;
        [SerializeField] private TextMeshProUGUI loopAmountText;
        [SerializeField] private Toggle doSkipValidationToggle;
        [SerializeField] private TMP_InputField profilerLabelInputField;

        private void Awake()
        {
            formulaInputField.text = "1+2*3/(4-5)";
            loopAmountSlider.onValueChanged.AddListener(value =>
                loopAmountText.text = "Loop Amount : " + value.ToString("#,0"));
            loopAmountSlider.value = 1_000_000;
            doSkipValidationToggle.isOn = false;
            profilerLabelInputField.text = "### FormulaCalculator.Calculate() ###";
        }

        private void Update()
        {
            try
            {
                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    Run(
                        formulaInputField.text.AsSpan(),
                        (ulong)loopAmountSlider.value,
                        doSkipValidationToggle.isOn,
                        profilerLabelInputField.text
                    );
                }
                else if (Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        private static void Run(ReadOnlySpan<char> formula, ulong loopAmount, bool doSkipValidation,
            string profilerLabel)
        {
            // Warmup
            for (int i = 0; i < 8; i++)
                _ = formula.Calculate(doSkipValidation: doSkipValidation);

            Profiler.BeginSample(profilerLabel);

            for (ulong i = 0; i < loopAmount; i++)
                _ = formula.Calculate(doSkipValidation: doSkipValidation);

            Profiler.EndSample();
        }
    }
}