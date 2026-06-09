using System;
using UnityEngine;
using UnityEngine.UI;

namespace CesiumFlood {
    public enum MapMode {
        Photoreal,
        OpenSource
    }

    /// <summary>
    /// Owns the "Use Photorealistic Tiles" checkbox in the settings menu. Persists the chosen
    /// mode and raises <see cref="OnMapModeChanged"/> so the tileset switching (added later) can
    /// react. Checked = Photoreal, unchecked = Open Source.
    /// </summary>
    public class MapModeSettings : MonoBehaviour {
        private const string PrefKey = "UsePhotorealisticTiles";
        private const bool DefaultUsePhotoreal = false;

        [SerializeField]
        private Toggle usePhotorealisticTilesToggle;

        public static event Action<MapMode> OnMapModeChanged;

        public MapMode CurrentMode { get; private set; }

        /// <summary>Reads the persisted map mode (single source of truth for the pref key/default).</summary>
        public static MapMode GetPersistedMode() {
            bool usePhotoreal = PlayerPrefs.GetInt(PrefKey, DefaultUsePhotoreal ? 1 : 0) == 1;
            return usePhotoreal ? MapMode.Photoreal : MapMode.OpenSource;
        }

        private void Start() {
            CurrentMode = GetPersistedMode();
            bool usePhotoreal = CurrentMode == MapMode.Photoreal;

            if (usePhotorealisticTilesToggle) {
                usePhotorealisticTilesToggle.SetIsOnWithoutNotify(usePhotoreal);
                usePhotorealisticTilesToggle.onValueChanged.AddListener(OnToggleChanged);
            }

            OnMapModeChanged?.Invoke(CurrentMode);
        }

        private void OnDestroy() {
            if (usePhotorealisticTilesToggle) {
                usePhotorealisticTilesToggle.onValueChanged.RemoveListener(OnToggleChanged);
            }
        }

        private void OnToggleChanged(bool usePhotoreal) {
            CurrentMode = usePhotoreal ? MapMode.Photoreal : MapMode.OpenSource;
            PlayerPrefs.SetInt(PrefKey, usePhotoreal ? 1 : 0);
            PlayerPrefs.Save();
            OnMapModeChanged?.Invoke(CurrentMode);
        }
    }
}
