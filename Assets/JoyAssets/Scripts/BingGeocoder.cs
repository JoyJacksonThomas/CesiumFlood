//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//
//public class BingGeocoder : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {
//        
//    }
//
//    // Update is called once per frame
//    void Update()
//    {
//        
//    }
//}

//using Windows.Services.Maps;
//using Windows.Devices.Geolocation;
//...
//private async void geocodeButton_Click(object sender, RoutedEventArgs e)
//{
//    // The address or business to geocode.
//    string addressToGeocode = "Microsoft";

//    // The nearby location to use as a query hint.
//    BasicGeoposition queryHint = new BasicGeoposition();
//    queryHint.Latitude = 47.643;
//    queryHint.Longitude = -122.131;
//    Geopoint hintPoint = new Geopoint(queryHint);

//    // Geocode the specified address, using the specified reference point
//    // as a query hint. Return no more than 3 results.
//    MapLocationFinderResult result =
//          await MapLocationFinder.FindLocationsAsync(
//                            addressToGeocode,
//                            hintPoint,
//                            3);

//    // If the query returns results, display the coordinates
//    // of the first result.
//    if (result.Status == MapLocationFinderStatus.Success)
//    {
//        tbOutputText.Text = "result = (" +
//              result.Locations[0].Point.Position.Latitude.ToString() + "," +
//              result.Locations[0].Point.Position.Longitude.ToString() + ")";
//    }
//}
