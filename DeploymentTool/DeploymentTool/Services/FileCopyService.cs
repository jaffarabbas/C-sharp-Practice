using System.IO;
using DeploymentTool.Models;

namespace DeploymentTool.Services;

public class FileCopyService
{
    public async Task CreatePatchAsync(
        IEnumerable<FileChange> files,
        string patchFolder)
    {
        Directory.CreateDirectory(patchFolder);

        var tasks = files.Select(f => CopyFileAsync(f, patchFolder));
        await Task.WhenAll(tasks);
    }

    private static async Task CopyFileAsync(FileChange file, string patchFolder)
    {
        var destPath = Path.Combine(patchFolder, file.RelativePath);
        var destDir = Path.GetDirectoryName(destPath)!;

        Directory.CreateDirectory(destDir);

        await using var src = new FileStream(
            file.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true);
        await using var dst = new FileStream(
            destPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true);

        await src.CopyToAsync(dst);
    }
}
