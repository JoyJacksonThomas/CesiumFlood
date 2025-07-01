using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CesiumForUnity;
using Unity.Mathematics;
using TMPro;
public class WaterSlider : MonoBehaviour
{
    public WaterPlaneSpawner waterSpawner;
    Slider waterLevelSlider;




    public float CurrentWaterHeight;
    public float[] WaterHeights;

    [TextArea]
    public string[] WaterHeightMessages;
    public TextMeshProUGUI TextBox;
    

    [Header("UI")]
    public RectTransform fillBox;
    public Vector2 fillBoxHeightMinMax;

    // Start is called before the first frame update
    void Awake()
    {
        waterLevelSlider = GetComponent<Slider>();
        waterLevelSlider.onValueChanged.AddListener(OnSliderValueChanged);
        UpdateSliderValue();
    }

    void OnSliderValueChanged(float value) {
        AdjustWaterSlider(value);
        UpdateWaterHeight();
    }

    // Update is called once per frame
    void Update()
    {
        // UpdateWaterHeight();
    }

    public void AdjustWaterSlider(float lerpValue)
    {
        CurrentWaterHeight = Mathf.Lerp(0, WaterHeights[WaterHeights.Length - 1], lerpValue);


        float height = Mathf.Lerp(fillBoxHeightMinMax.x, fillBoxHeightMinMax.y, waterLevelSlider.normalizedValue);
        fillBox.sizeDelta = new Vector2(fillBox.sizeDelta.x, height);

    }

    public void AdjustWaterToPresets(int direction)
    {
        if (direction == 0)
            return;
        else
        {
            direction /= Mathf.Abs(direction);
            
            for(int i = 0; i < WaterHeights.Length - 1; i++)
            {
                if (        direction > 0 && (CurrentWaterHeight >= WaterHeights[i] && CurrentWaterHeight < WaterHeights[i + 1])
                        ||  direction < 0 && (CurrentWaterHeight > WaterHeights[i] && CurrentWaterHeight <= WaterHeights[i + 1])     )
                {
                    
                    int index = direction > 0 ? Mathf.Clamp(i + direction, 0, WaterHeights.Length - 1) : Mathf.Clamp(i + 1 + direction, 0, WaterHeights.Length - 1);
                    CurrentWaterHeight = WaterHeights[index];

                    UpdateSliderValue();
                    UpdateTextBox(index);
                    return;
                }
            }
        }
    }


    public void UpdateWaterHeight()
    {
        if (waterSpawner != null)
            waterSpawner.WaterHeight = CurrentWaterHeight;
    }

    void UpdateSliderValue()
    {
        waterLevelSlider.value = Mathf.InverseLerp(0, WaterHeights[WaterHeights.Length - 1], CurrentWaterHeight);
    }

    void UpdateTextBox(int messageIndex)
    {
        TextBox.text = WaterHeightMessages[messageIndex];
    }

}
