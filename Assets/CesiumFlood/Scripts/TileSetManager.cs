using System.Collections.Generic;
using UnityEngine;

namespace CesiumFlood {
    /// <summary>
    ///     Switches between the photorealistic and open-source tilesets by toggling two lists of
    ///     GameObjects. Reacts to <see cref="MapModeSettings.OnMapModeChanged" /> at runtime and
    ///     applies the persisted mode on startup, so the correct set is active even if the settings
    ///     menu is loaded later (or never).
    /// </summary>
    public class TileSetManager : MonoBehaviour {
        [SerializeField]
        [Tooltip("Objects active in Photoreal mode (e.g. Google Photorealistic 3D Tiles).")]
        private List<GameObject> photorealObjects = new();

        [SerializeField]
        [Tooltip("Objects active in Open Source mode (e.g. OSM buildings + terrain).")]
        private List<GameObject> openSourceObjects = new();

        private void OnEnable() {
            MapModeSettings.OnMapModeChanged += ApplyMode;
        }

        private void OnDisable() {
            MapModeSettings.OnMapModeChanged -= ApplyMode;
        }

        private void Start() {
            ApplyMode(MapModeSettings.GetPersistedMode());
        }

        private void ApplyMode(MapMode mode) {
            bool usePhotoreal = mode == MapMode.Photoreal;
            SetActive(photorealObjects, usePhotoreal);
            SetActive(openSourceObjects, !usePhotoreal);
        }

        private static void SetActive(List<GameObject> objects, bool active) {
            foreach (GameObject go in objects) {
                if (go != null) {
                    go.SetActive(active);
                }
            }
        }
    }
}