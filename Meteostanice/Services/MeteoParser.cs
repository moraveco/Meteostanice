namespace Meteostanice.Services;

using System.Text.Json;
using System.Xml.Linq;

public class MeteoParser
{
    public string? ParseXmlToJson(string xml)
    {
        var doc = XDocument.Parse(xml);

        var dict = doc.Root?
            .Elements()
            .ToDictionary(
                el => el.Name.LocalName,
                el => el.Value
            );

        if (dict == null || dict.Count == 0)
            return null;

        return JsonSerializer.Serialize(dict, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }
}