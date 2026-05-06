using System.IO;
using DeploymentTool.Models;

namespace DeploymentTool.Services;

public class ProjectScannerService
{
    public async Task<List<ProjectItem>> ScanAsync(string rootPath)
    {
        return await Task.Run(() =>
        {
            var projects = new List<ProjectItem>();

            foreach (var file in Directory.EnumerateFiles(rootPath, "*.csproj", SearchOption.AllDirectories))
            {
                projects.Add(new ProjectItem
                {
                    Name = Path.GetFileNameWithoutExtension(file),
                    Path = file
                });
            }

            return projects;
        });
    }
}
