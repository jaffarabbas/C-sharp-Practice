using System.IO;
using System.Text.Json;

namespace TestProject1.Utilities
{
    public static class JsonHelper
    {
        public static T ReadJson<T>(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(json)!;
        }
    }
}
