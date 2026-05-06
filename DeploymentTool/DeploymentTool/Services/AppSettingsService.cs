using System.IO;
using DeploymentTool.Models;

namespace DeploymentTool.Services;

public class AppSettingsService
{
    private static readonly string[] Patterns = ["appsettings.json", "web.config"];

    public Task<List<ConfigFileItem>> LoadConfigFilesAsync(IList<ProjectItem> projects)
        => Task.Run(() => Discover(projects));

    private static List<ConfigFileItem> Discover(IList<ProjectItem> projects)
    {
        var result = new List<ConfigFileItem>();

        foreach (var project in projects)
        {
            var dir = Path.GetDirectoryName(project.Path);
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
                continue;

            foreach (var pattern in Patterns)
            foreach (var file in FindFiles(dir, pattern))
                result.Add(new ConfigFileItem(file, project.Name));
        }

        return result;
    }

    private static IEnumerable<string> FindFiles(string dir, string pattern)
    {
        return Directory
            .EnumerateFiles(dir, pattern, new EnumerationOptions
            {
                IgnoreInaccessible    = true,
                RecurseSubdirectories = true,
                MaxRecursionDepth     = 4
            })
            .Where(f =>
            {
                var rel = Path.GetRelativePath(dir, f);
                return !rel.StartsWith("bin", StringComparison.OrdinalIgnoreCase)
                    && !rel.StartsWith("obj", StringComparison.OrdinalIgnoreCase);
            });
    }
}
