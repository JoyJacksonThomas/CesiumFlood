using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using Leguar.TotalJSON;
using UnityEngine;
using UnityEngine.Networking;

public class GeoCoder_V2 : MonoBehaviour {
    public void RequestLatLongAsync(string address, Action<Vector2> callback) {
        StartCoroutine(RequestNominatimLatLong(address, callback));
    }

    // Reverse geocoding: from lat/lon to a human-readable address (display_name)
    public void RequestReverseAddressAsync(Vector2 latLon, Action<string> callback) {
        StartCoroutine(RequestNominatimReverse(latLon, callback));
    }


    private IEnumerator RequestNominatimLatLong(string address, Action<Vector2> callback) {
        string requestUri = string.Format("https://nominatim.openstreetmap.org/search?q={0}&format=jsonv2",
            Uri.EscapeDataString(address));

        UnityWebRequest webRequest = UnityWebRequest.Get(requestUri);

        // Request and wait for the desired page.
        webRequest.SetRequestHeader("User-Agent", "YouthMappers Cesium Flood");
        bool success = false;
        yield return webRequest.SendWebRequest();

        switch (webRequest.result) {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError("Error: " + webRequest.error);
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError("HTTP Error: " + webRequest.error);
                break;
            case UnityWebRequest.Result.Success:
                // Debug.Log("Received: " + webRequest.downloadHandler.text);
                success = true;
                callback(ProcessGeocodeSuccess(webRequest.downloadHandler.text));
                break;
        }

        if (!success) {
            Debug.LogError("Failed to get address: " + webRequest.error);
            callback(new LatLong().ToVector2());
        }
    }

    private IEnumerator RequestNominatimReverse(Vector2 latLon, Action<string> callback) {
        string requestUri = string.Format(
            "https://nominatim.openstreetmap.org/reverse?lat={0}&lon={1}&format=jsonv2",
            latLon.x.ToString(CultureInfo.InvariantCulture),
            latLon.y.ToString(CultureInfo.InvariantCulture));

        UnityWebRequest webRequest = UnityWebRequest.Get(requestUri);

        webRequest.SetRequestHeader("User-Agent", "YouthMappers Cesium Flood");
        bool success = false;
        yield return webRequest.SendWebRequest();

        switch (webRequest.result) {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError("Error: " + webRequest.error);
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError("HTTP Error: " + webRequest.error);
                break;
            case UnityWebRequest.Result.Success:
                // Debug.Log("Received: " + webRequest.downloadHandler.text);
                success = true;
                callback(ProcessReverseSuccess(webRequest.downloadHandler.text));
                break;
        }

        if (!success) {
            Debug.LogError("Failed to reverse lookup: " + webRequest.error);
            callback(string.Empty);
        }
    }

    private Vector2 ProcessGeocodeSuccess(string jsonString) {
        JArray results = JArray.ParseString(jsonString);
        if (results.IsEmpty()) {
            Debug.LogError("result was empty");
            return new Vector2(0, 0);
        }

        float x = float.Parse(results.GetJSON(0).GetString("lat"));
        float y = float.Parse(results.GetJSON(0).GetString("lon"));

        // LatLong result = JsonUtility.FromJson<LatLong>(jsonString);
        return new Vector2(x, y);
    }

    private string ProcessReverseSuccess(string jsonString) {
        JSON result = JSON.ParseString(jsonString);
        // // Prefer the full display name if available.
        // if (result.ContainsKey("display_name")) {
        //     return result.GetString("display_name");
        // }

        // Fallback: try to compose from address object if present
        try {
            JSON address = result.GetJSON("address");
            // Attempt a simple composition of common fields
            string name = result.ContainsKey("name") ? result.GetString("name") : "";
            // if no name, use amenity
            if (string.IsNullOrEmpty(name)) {
                name = address.ContainsKey("amenity") ? address.GetString("amenity") : "";
            }

            // add new line if name or amenity was found
            if (!string.IsNullOrEmpty(name)) {
                name += "\n";
            }

            string number = address.ContainsKey("house_number") ? address.GetString("house_number") : "";
            string road = address.ContainsKey("road") ? address.GetString("road") : "";
            // new line
            string neighborhood = address.ContainsKey("neighborhood") ? address.GetString("neighborhood") + "\n" : "";
            // new line if neighborhood
            string city = address.ContainsKey("city") ? address.GetString("city") :
                address.ContainsKey("town") ? address.GetString("town") :
                address.ContainsKey("village") ? address.GetString("village") : "";
            string state = address.ContainsKey("state") ? address.GetString("state") : "";
            string postcode = address.ContainsKey("postcode") ? address.GetString("postcode") : "";
            // new line
            string country = address.ContainsKey("country") ? address.GetString("country") : "";

            string composed = string.Join(" ",
                new[] { name, number, road, neighborhood, "\n", city, state, postcode, "\n", country }
                    .Where(s => !string.IsNullOrEmpty(s)));
            return composed;
        }
        catch (Exception) {
            // If anything goes wrong, just return raw JSON for debugging
            return jsonString;
        }
    }

    private class LatLong {
        public float lat;
        public float lon;

        public Vector2 ToVector2() {
            return new Vector2(lat, lon);
        }
    }
}