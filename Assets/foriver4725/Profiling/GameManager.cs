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

        private ulong _loopAmount = 1;

        private void Awake()
        {
            loopAmountSlider.onValueChanged.AddListener(value =>
            {
                value *= 0.1f;
                _loopAmount = (ulong)Mathf.Pow(10, value);
                loopAmountText.text = $"Loop Amount : 1e{value:N1}";
            });

            formulaInputField.text = "1+2*3/(4-5)";
            loopAmountSlider.onValueChanged.Invoke(loopAmountSlider.value = 70); // 1e7.0
            doSkipValidationToggle.isOn = true;
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
                        _loopAmount,
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