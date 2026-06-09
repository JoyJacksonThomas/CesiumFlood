using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CesiumFlood {
    /// <summary>
    ///     Drives the look-sensitivity slider in the settings menu. Persists the value and applies it
    ///     to the linked <see cref="CF_CameraController" />. The camera controller is linked as a scene
    ///     reference on the SettingsMenu instance (not in the prefab asset).
    /// </summary>
    public class SensitivitySettings : MonoBehaviour {
        private const string PrefKey = "LookSensitivity";

        [SerializeField]
        private Slider sensitivitySlider;

        [SerializeField]
        private CF_CameraController cameraController;

        [SerializeField]
        private TMP_Text sensitivityValueText;

        [SerializeField]
        private float minSensitivity = 0.1f;

        [SerializeField]
        private float maxSensitivity = 8f;

        [SerializeField]
        private float defaultSensitivity = 2f;

        private void Start() {
            float sensitivity = PlayerPrefs.GetFloat(PrefKey, defaultSensitivity);
            ApplySensitivity(sensitivity);

            if (sensitivitySlider) {
                sensitivitySlider.wholeNumbers = false;
                sensitivitySlider.minValue = minSensitivity;
                sensitivitySlider.maxValue = maxSensitivity;
                sensitivitySlider.SetValueWithoutNotify(sensitivity);
                sensitivitySlider.onValueChanged.AddListener(OnSliderChanged);
            }

            UpdateValueLabel(sensitivity);
        }

        private void OnDestroy() {
            if (sensitivitySlider) {
                sensitivitySlider.onValueChanged.RemoveListener(OnSliderChanged);
            }
        }

        private void OnSliderChanged(float value) {
            // Snap to 0.1 increments so the displayed/stored value matches the slider steps.
            value = Mathf.Round(value * 10f) / 10f;
            if (sensitivitySlider) {
                sensitivitySlider.SetValueWithoutNotify(value);
            }

            PlayerPrefs.SetFloat(PrefKey, value);
            PlayerPrefs.Save();
            ApplySensitivity(value);
            UpdateValueLabel(value);
        }

        private void ApplySensitivity(float value) {
            if (cameraController) {
                cameraController.SetSensitivity(value);
            }
        }

        private void UpdateValueLabel(float value) {
            if (sensitivityValueText) {
                sensitivityValueText.SetText("{0:0.0}", value);
            }
        }
    }
}