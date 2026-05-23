using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInputHint", menuName = "CesiumFlood/Input Hint Data")]
public class InputHintData : ScriptableObject
{
    public List<Sprite> icons = new List<Sprite>();
    public string label;
}