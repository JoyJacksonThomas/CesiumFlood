using UnityEngine;

/// <summary>
///     Third-person orbit camera. Look input drives yaw and pitch; the rig smoothly follows a
///     target while the camera child trails a fixed distance behind the pivot.
///     All framing happens in LateUpdate so it is applied after the target has moved/rotated for
///     the frame, which keeps the view from stuttering relative to the rendered image.
/// </summary>
public class CF_CameraController : MonoBehaviour {
    [Header("Target")]
    [SerializeField]
    private Transform target;

    [SerializeField]
    [Tooltip("Camera that trails behind the pivot. Offset along its local -Z.")]
    private Camera mCamera;

    [SerializeField]
    [Tooltip("Height above the target that the camera pivots around.")]
    private float pivotHeight = 1.8f;

    [SerializeField]
    [Tooltip("Distance the camera trails behind the pivot.")]
    private float distance = 19f;

    [Header("Look")]
    [SerializeField]
    private float sensitivity = 8f;

    [SerializeField]
    private float minPitch = -65f;

    [SerializeField]
    private float maxPitch = 90f;

    [Header("Follow")]
    [SerializeField]
    [Tooltip("How quickly the rig catches up to the target position. Higher = snappier.")]
    private float followSharpness = 20f;

    private float pitch;
    private float yaw;

    private void Start() {
        // Apply the persisted look sensitivity (set via the settings menu); falls back to the
        // value configured on the prefab. Shared key with SensitivitySettings.
        sensitivity = PlayerPrefs.GetFloat("LookSensitivity", sensitivity);

        Vector3 angles = transform.eulerAngles;
        pitch = angles.x;
        yaw = angles.y;

        if (mCamera != null) {
            mCamera.transform.localPosition = new Vector3(0f, 0f, -distance);
        }
    }

    private void LateUpdate() {
        if (target == null) {
            return;
        }

        // Frame-rate independent follow toward the pivot point above the target.
        Vector3 desiredPosition = target.position + Vector3.up * pivotHeight;
        float t = 1f - Mathf.Exp(-followSharpness * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, t);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    /// <summary>Feeds look input. x = horizontal (yaw), y = vertical (pitch).</summary>
    public void OnLook(Vector2 input) {
        yaw += input.x * sensitivity * Time.deltaTime;
        pitch += input.y * sensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    public void SetSensitivity(float value) {
        sensitivity = value;
    }

    /// <summary>World-space look direction derived from yaw/pitch (no transform-order dependency).</summary>
    public Vector3 GetLookDirection() {
        return Quaternion.Euler(pitch, yaw, 0f) * Vector3.forward;
    }
}
