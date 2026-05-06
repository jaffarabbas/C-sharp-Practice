using System.Diagnostics;
using System.IO;
using DeploymentTool.Models;

namespace DeploymentTool.Services;

public class PublishService
{
    public async Task PublishAsync(
        IEnumerable<ProjectItem> projects,
        string outputRoot,
        Action<string> log,
        CancellationToken ct = default)
    {
        var tasks = projects.Select(p => PublishOneAsync(p, outputRoot, log, ct));
        await Task.WhenAll(tasks);
    }

    private static async Task PublishOneAsync(
        ProjectItem project,
        string outputRoot,
        Action<string> log,
        CancellationToken ct)
    {
        var outputPath = Path.Combine(outputRoot, project.Name);

        var psi = new ProcessStartInfo("dotnet")
        {
            Arguments = $"publish \"{project.Path}\" -c Release -o \"{outputPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = psi };
        process.OutputDataReceived += (_, e) => { if (e.Data != null) log($"[{project.Name}] {e.Data}"); };
        process.ErrorDataReceived += (_, e) => { if (e.Data != null) log($"[{project.Name}] ERR: {e.Data}"); };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync(ct);
        log($"[{project.Name}] Exit code: {process.ExitCode}");
    }
}
