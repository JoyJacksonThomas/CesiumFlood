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
        if (geoCoder == null) {
            Debug.LogError("GeoCoder is null", this);
            return;
        }

        if (addressInput == null) {
            Debug.LogError("AddressInput is null", this);
            return;
        }


        Debug.Log("Searching for " + addressInput.text);
        geoCoder.RequestLatLongAsync(addressInput.text, OnAddressResponse);
    }

    private void OnAddressResponse(Vector2 _latLong) {
        Debug.Log($"Received\tLat: [{_latLong.x}] \n" +
                  $"\t \t \tLong: [{_latLong.y}]");

        if (camAnchor == null) {
            Debug.LogError("CameraAnchor is null", this);
            return;
        }

        camAnchor.longitudeLatitudeHeight = new double3(_latLong.y, _latLong.x, 100);
        //waterAnchor.longitudeLatitudeHeight = new double3(latLong.y, latLong.x, 100);
        // camAnchor.gameObject.GetComponent<WaterPlaneSpawner>().CenterWaterOnCamera();
    }
}