using CesiumForUnity;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class AddressRequester : MonoBehaviour {
    public Vector2 latLong;

    public CesiumGlobeAnchor camAnchor;

    public GeoCoder_V2 geoCoder;

    public TMP_InputField addressInput;

    public void SearchAddress() {
        if (geoCoder == null || addressInput == null) {
            Debug.LogError("GeoCoder or AddressInput is null", this);
        }

        Debug.Log("Searching for " + addressInput.text);
        geoCoder.RequestLatLongAsync(addressInput.text, OnAddressResponse);
    }

    private void OnAddressResponse(Vector2 _latLong) {
        camAnchor.longitudeLatitudeHeight = new double3(_latLong.y, _latLong.x, 100);
        //waterAnchor.longitudeLatitudeHeight = new double3(latLong.y, latLong.x, 100);
        camAnchor.gameObject.GetComponent<WaterPlaneSpawner>().CenterWaterOnCamera();
    }
}