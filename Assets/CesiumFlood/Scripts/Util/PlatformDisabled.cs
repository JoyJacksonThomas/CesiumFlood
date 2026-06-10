using UnityEngine;

/// <summary>
/// Disables the GameObject this is attached to on the selected build platforms.
/// </summary>
public class PlatformDisabled : MonoBehaviour {
    [SerializeField]
    private bool disableOnWeb;

    [SerializeField]
    private bool disableOnPC;

    private void Awake() {
#if UNITY_WEBGL
        if (disableOnWeb) {
            gameObject.SetActive(false);
        }
#elif UNITY_STANDALONE
        if (disableOnPC) {
            gameObject.SetActive(false);
        }
#endif
    }
}