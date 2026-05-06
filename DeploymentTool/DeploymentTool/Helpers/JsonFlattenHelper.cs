using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace DeploymentTool.Helpers;

public static class JsonFlattenHelper
{
    private static readonly JsonSerializerOptions WriteOptions = new() { WriteIndented = true };

    // Lenient parse options: allow trailing commas and // comments (common in real appsettings.json)
    private static readonly JsonDocumentOptions ParseOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling     = JsonCommentHandling.Skip
    };

    // ── Flatten ────────────────────────────────────────────────────────────────

    public static Dictionary<string, string> Flatten(string json)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        if (JsonNode.Parse(json, nodeOptions: null, documentOptions: ParseOptions) is JsonObject root)
            FlattenObject(root, string.Empty, result);
        return result;
    }

    private static void FlattenObject(JsonObject obj, string prefix, Dictionary<string, string> result)
    {
        foreach (var (key, node) in obj)
        {
            var flatKey = string.IsNullOrEmpty(prefix) ? key : $"{prefix}:{key}";

            if (node is JsonObject nested)
                FlattenObject(nested, flatKey, result);
            else if (node is not JsonArray)          // arrays are skipped — preserved on save
                result[flatKey] = ToScalar(node);
        }
    }

    private static string ToScalar(JsonNode? node)
    {
        if (node is not JsonValue val) return string.Empty;
        return val.GetValueKind() switch
        {
            JsonValueKind.String => val.GetValue<string>(),
            JsonValueKind.True   => "true",
            JsonValueKind.False  => "false",
            JsonValueKind.Null   => string.Empty,
            _                    => val.ToJsonString()          // numbers: raw JSON representation
        };
    }

    // ── ApplyChange ───────────────────────────────────────────────────────────
    // Finds every key that matches (case-insensitive) and sets it to value.

    public static string ApplyChange(string json, string key, string value)
    {
        var flat    = Flatten(json);
        var updates = flat.Keys
            .Where(k => string.Equals(k, key, StringComparison.OrdinalIgnoreCase) ||
                        k.EndsWith(":" + key, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(k => k, _ => value, StringComparer.Ordinal);
        return updates.Count > 0 ? Rebuild(json, updates) : json;
    }

    // ── Rebuild ────────────────────────────────────────────────────────────────
    // Only keys present in `updates` are modified; everything else is preserved.

    public static string Rebuild(string originalJson, Dictionary<string, string> updates)
    {
        var root = JsonNode.Parse(originalJson, nodeOptions: null, documentOptions: ParseOptions) as JsonObject ?? new JsonObject();
        foreach (var (flatKey, value) in updates)
            SetValue(root, flatKey.Split(':'), 0, value);
        return root.ToJsonString(WriteOptions);
    }

    private static void SetValue(JsonObject obj, string[] parts, int index, string value)
    {
        var key = parts[index];

        if (index == parts.Length - 1)
        {
            // Preserve the original JSON value-kind so we don't change bool/number to string
            if (obj[key] is JsonValue existing)
            {
                obj[key] = existing.GetValueKind() switch
                {
                    JsonValueKind.True or JsonValueKind.False
                        when bool.TryParse(value, out var b)
                        => JsonValue.Create(b),

                    JsonValueKind.Number
                        when long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var l)
                        => JsonValue.Create(l),

                    JsonValueKind.Number
                        when double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var d)
                        => JsonValue.Create(d),

                    _ => JsonValue.Create(value)
                };
            }
            else
            {
                obj[key] = JsonValue.Create(value);
            }
            return;
        }

        if (obj[key] is not JsonObject nested)
        {
            nested    = new JsonObject();
            obj[key] = nested;
        }
        SetValue(nested, parts, index + 1, value);
    }
}
