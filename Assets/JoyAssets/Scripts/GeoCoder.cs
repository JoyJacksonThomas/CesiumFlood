using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Xml.Linq;

public static class GeoCoder
{
    // Start is called before the first frame update
   

    public static string RequestLatLong()
    {
        string address = "Mystic, CT";
        string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key={1}&address={0}&sensor=false", System.Uri.EscapeDataString(address), System.Uri.EscapeDataString("") );

        WebRequest request = WebRequest.Create(requestUri);
        WebResponse response = request.GetResponse();
        XDocument xdoc = XDocument.Load(response.GetResponseStream());

        return xdoc.ToString();

        XElement result = xdoc.Element("GeocodeResponse").Element("result");
        XElement locationElement = result.Element("geometry").Element("location");
        XElement lat = locationElement.Element("lat");
        XElement lng = locationElement.Element("lng");

        return InnerXML(lat);
    }

    public static string InnerXML(this XElement el)
    {
        var reader = el.CreateReader();
        reader.MoveToContent();
        return reader.ReadInnerXml();
    }

}
