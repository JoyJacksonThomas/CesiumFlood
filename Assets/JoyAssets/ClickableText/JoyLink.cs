using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class JoyLink : MonoBehaviour , IPointerClickHandler
{
    float speed = 100;

    RectTransform rect;
    [SerializeField]
    TextMeshProUGUI mainText;

    [SerializeField]
    bool isLooping;

    // Start is called before the first frame update
    void Start()
    {
        rect = gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            int clickLink = TMP_TextUtilities.FindIntersectingLink(mainText, Input.mousePosition, null);
            if (clickLink == 0)
            {
                Application.OpenURL(mainText.textInfo.linkInfo[clickLink].GetLinkID());
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (eventData.button == PointerEventData.InputButton.Left)
        {
        }
    }
}
