using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class LinkOpener : MonoBehaviour {

    [SerializeField]
    private string link;

    public void OpenLink()
    {
        Application.OpenURL(link);
    }
}
