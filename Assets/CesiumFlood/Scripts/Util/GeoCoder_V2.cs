using System;
using System.Collections;
using Leguar.TotalJSON;
using UnityEngine;
using UnityEngine.Networking;

public class GeoCoder_V2 : MonoBehaviour {
    public void RequestLatLongAsync(string address, Action<Vector2> callback) {
        StartCoroutine(RequestNominatimLatLong(address, callback));
    }


    private IEnumerator RequestNominatimLatLong(string address, Action<Vector2> callback) {
        string requestUri = string.Format("https://nominatim.openstreetmap.org/search?q={0}&format=jsonv2",
            Uri.EscapeDataString(address));

        UnityWebRequest webRequest = UnityWebRequest.Get(requestUri);

        // Request and wait for the desired page.
        webRequest.SetRequestHeader("User-Agent", "YouthMappers Cesium Flood");

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
                Debug.Log("Received: " + webRequest.downloadHandler.text);
                callback(ProcessSuccess(webRequest.downloadHandler.text));
                break;
        }


        callback(new LatLong().ToVector2());
    }

    private Vector2 ProcessSuccess(string jsonString) {
        JArray results = JArray.ParseString(jsonString);


        float x = float.Parse(results.GetJSON(0).GetString("lat"));
        float y = float.Parse(results.GetJSON(0).GetString("lon"));


        Debug.Log(x.ToString());
        Debug.Log(y.ToString());
        // LatLong result = JsonUtility.FromJson<LatLong>(jsonString);
        return new Vector2(x, y);
    }

    private class LatLong {
        public float lat;
        public float lon;

        public Vector2 ToVector2() {
            return new Vector2(lat, lon);
        }
    }
}