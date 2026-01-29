using CesiumFlood;
using TMPro;
using UnityEngine;

public class CurrentAddressDisplay : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI AddressTextBox;

    private void Start() {
        InvokeRepeating(nameof(LookupCurrentAddress), 0f, 3f);
    }

    private void LookupCurrentAddress() {
        AddressRequester Requester = UIManager.Instance.GetAddressRequester();

        if (Requester && !Requester.IsSetup()) {
            Debug.LogError("AddressRequester was not setup correctly. Current Address will not update", this);
            CancelInvoke(nameof(LookupCurrentAddress));
            return;
        }

        Requester.FindCurrentAddress(HandleFoundAddress);
    }

    private void HandleFoundAddress(string Result) {
        AddressTextBox.text = Result;
    }
}