using System;
using CesiumForUnity;
using TMPro;
using UnityEngine;

public class AddressRequester : MonoBehaviour {
    [SerializeField]
    private CesiumGlobeAnchor camAnchor;

    [SerializeField]
    private GeoCoder_V2 geoCoder;

    [SerializeField]
    private TMP_InputField addressInput;

    [SerializeField]
    private CF_PlayerController PC;

    private Vector2 latLong;


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
        // Debug.Log($"Received\tLat: [{_latLong.x}] \n" +
        //           $"\t \t \tLong: [{_latLong.y}]");

        if (PC == null) {
            Debug.LogError("Playercharacter is null", this);
            return;
        }

        PC.SetGlobalPosition(_latLong);
        // camAnchor.longitudeLatitudeHeight = new double3(_latLong.y, _latLong.x, 100);
        //waterAnchor.longitudeLatitudeHeight = new double3(latLong.y, latLong.x, 100);
        // camAnchor.gameObject.GetComponent<WaterPlaneSpawner>().CenterWaterOnCamera();
    }

    public bool IsSetup() {
        return geoCoder != null && addressInput != null && PC;
    }

    public void FindCurrentAddress(Action<string> Callback) {
        if (!geoCoder) {
            Debug.LogError("GeoCoder is null", this);
            return;
        }

        if (!PC) {
            Debug.LogError("PlayerCharacter is null", this);
            return;
        }

        geoCoder.RequestReverseAddressAsync(PC.GetGlobalPosition(), Callback);
    }
}