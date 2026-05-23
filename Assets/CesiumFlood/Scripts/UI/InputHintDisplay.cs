using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputHintDisplay : MonoBehaviour
{
    [SerializeField] private Image[] icons = new Image[4];
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private InputHintData currentData;

    private void OnValidate()
    {
        if (currentData != null)
            Apply(currentData);
    }

    public void Setup(InputHintData data)
    {
        if (data == null)
        {
            Debug.LogWarning("InputHintDisplay: Setup called with null data", this);
            return;
        }

        currentData = data;
        Apply(data);
    }

    private void Apply(InputHintData data)
    {
        for (int i = 0; i < icons.Length; i++)
        {
            if (icons[i] == null) continue;

            if (data.icons != null && i < data.icons.Count && data.icons[i] != null)
            {
                icons[i].sprite = data.icons[i];
                icons[i].gameObject.SetActive(true);
            }
            else
            {
                icons[i].gameObject.SetActive(false);
            }
        }

        if (labelText != null)
        {
            labelText.text = data.label ?? string.Empty;
        }
    }
}