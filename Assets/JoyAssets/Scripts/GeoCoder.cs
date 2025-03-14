using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Xml.Linq;
using System.Net.Http;
using System.Text;

public static class GeoCoder
{
    // Start is called before the first frame update
   

    public static Vector2 RequestLatLong(string address)
    {
        string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key={1}&address={0}&sensor=false", System.Uri.EscapeDataString(address), "");

        WebRequest request = WebRequest.Create(requestUri);
        WebResponse response = request.GetResponse();
        XDocument xdoc = XDocument.Load(response.GetResponseStream());
        
        //return xdoc.ToString();
        
        XElement result = xdoc.Element("GeocodeResponse").Element("result");
        XElement locationElement = result.Element("geometry").Element("location");
        XElement lat = locationElement.Element("lat");
        XElement lng = locationElement.Element("lng");

        Vector2 latLong = Vector2.zero;
        latLong.x = float.Parse(InnerXML(lat));
        latLong.y = float.Parse(InnerXML(lng));
        return latLong;
    }

    public static string InnerXML(this XElement el)
    {
        var reader = el.CreateReader();
        reader.MoveToContent();
        return reader.ReadInnerXml();
    }

}
