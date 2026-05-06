using System.IO;
using System.Xml.Linq;

namespace DeploymentTool.Helpers;

public static class XmlConfigHelper
{
    // ── Read ──────────────────────────────────────────────────────────────────

    public static Dictionary<string, string> ReadAppSettings(string filePath)
    {
        var doc = XDocument.Load(filePath);
        return doc
            .Descendants("appSettings")
            .FirstOrDefault()
            ?.Elements("add")
            .Where(e => e.Attribute("key") != null)
            .ToDictionary(
                e => e.Attribute("key")!.Value,
                e => e.Attribute("value")?.Value ?? string.Empty,
                StringComparer.Ordinal)
            ?? new Dictionary<string, string>(StringComparer.Ordinal);
    }

    // ── ApplyChange (in-memory) ───────────────────────────────────────────────

    public static string ApplyChange(string xml, string key, string value)
    {
        try
        {
            var doc         = XDocument.Parse(xml);
            var appSettings = doc.Descendants("appSettings").FirstOrDefault();
            if (appSettings == null) return xml;

            bool found = false;
            foreach (var el in appSettings.Elements("add"))
            {
                var k = el.Attribute("key")?.Value;
                if (k != null && string.Equals(k, key, StringComparison.OrdinalIgnoreCase))
                {
                    el.SetAttributeValue("value", value);
                    found = true;
                }
            }

            if (!found) return xml;

            using var sw = new StringWriter();
            doc.Save(sw);
            return sw.ToString();
        }
        catch
        {
            return xml;
        }
    }

    // ── Write ─────────────────────────────────────────────────────────────────
    // Called from a background thread (Task.Run); keeps XML declaration + indentation.

    public static void WriteSync(string filePath, Dictionary<string, string> updates)
    {
        var doc         = XDocument.Load(filePath);
        var appSettings = doc.Descendants("appSettings").FirstOrDefault();
        if (appSettings == null) return;

        foreach (var element in appSettings.Elements("add"))
        {
            var key = element.Attribute("key")?.Value;
            if (key != null && updates.TryGetValue(key, out var newValue))
                element.SetAttributeValue("value", newValue);
        }

        doc.Save(filePath);
    }
}
