using System.Collections.Generic;
using UnityEngine;

public class InputDisplay : MonoBehaviour {
    [SerializeField]
    private GameObject hintPrefab;


    [SerializeField]
    private Transform modeContainer;

    [SerializeField]
    private Transform commonContainer;

    [Header("Common Controls")]
    [SerializeField]
    private List<InputHintData> commonHints;

    [Header("Mode Specific Controls")]
    [SerializeField]
    private List<InputHintData> walkHints;

    [SerializeField]
    private List<InputHintData> jetSkiHints;

    [SerializeField]
    private List<InputHintData> droneHints;

    private readonly List<GameObject> spawnedHints = new();

    public void Awake() {
        for (int i = 0; i < modeContainer.childCount; i++) {
            GameObject hint = modeContainer.GetChild(i).gameObject;
            if (hint != null) {
                Destroy(hint);
            }
        }

        for (int i = 0; i < commonContainer.childCount; i++) {
            GameObject hint = commonContainer.GetChild(i).gameObject;
            if (hint != null) {
                Destroy(hint);
            }
        }
    }

    public void SetMode(MovementType mode) {
        if (hintPrefab == null || modeContainer == null) {
            Debug.LogWarning($"InputDisplay: SetMode failed — hintPrefab: {hintPrefab}, container: {modeContainer}",
                this);
            return;
        }

        ClearHints();

        if (commonHints != null) {
            foreach (InputHintData hint in commonHints) {
                AddHint(hint, commonContainer);
            }
        }

        List<InputHintData> specificHints = mode switch {
            MovementType.Walk => walkHints,
            MovementType.JetSki => jetSkiHints,
            MovementType.Drone => droneHints,
            _ => null
        };

        if (specificHints != null) {
            foreach (InputHintData hint in specificHints) {
                AddHint(hint, modeContainer);
            }
        }
    }

    private void AddHint(InputHintData hintData, Transform container) {
        if (hintPrefab == null || container == null || hintData == null) {
            Debug.LogWarning(
                $"InputDisplay: AddHint failed — hintPrefab: {hintPrefab}, container: {container}, hintData: {hintData}",
                this);
            return;
        }

        GameObject instance = Instantiate(hintPrefab, container);
        instance.SetActive(true);
        InputHintDisplay display = instance.GetComponent<InputHintDisplay>();
        if (display == null) {
            Debug.LogWarning("InputDisplay: hintPrefab is missing InputHintDisplay component", this);
        } else {
            display.Setup(hintData);
        }

        spawnedHints.Add(instance);
    }

    private void ClearHints() {
        foreach (GameObject hint in spawnedHints) {
            if (hint != null) {
                Destroy(hint);
            }
        }

        spawnedHints.Clear();
    }
}