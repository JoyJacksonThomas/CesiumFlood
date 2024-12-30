using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddressRequester : MonoBehaviour
{
    [TextArea]
    public string latLong;
    // Start is called before the first frame update
    void Start()
    {
        latLong = GeoCoder.RequestLatLong();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
