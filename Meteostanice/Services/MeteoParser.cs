namespace Meteostanice.Services;

using System;
using System.Xml;
using Newtonsoft.Json;

public class MeteoParser
{
    public string? ParseXmlToJson(string xml)
    {
        try
        {
            // in professional practice, XmlDocument is used for 1:1 conversion
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            
            // newtonsoft.json converts the entire xml tree to json automatically
            return JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);
        }
        catch (Exception)
        {
            // return null if xml is invalid
            return null;
        }
    }
}