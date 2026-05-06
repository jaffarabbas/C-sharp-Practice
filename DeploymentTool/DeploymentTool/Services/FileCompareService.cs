using System.IO;
using System.Security.Cryptography;
using DeploymentTool.Models;

namespace DeploymentTool.Services;

public class FileCompareService
{
    public async Task<List<FileChange>> CompareAsync(string oldFolder, string newFolder)
    {
        return await Task.Run(() =>
        {
            var changes = new List<FileChange>();

            foreach (var newFile in Directory.EnumerateFiles(newFolder, "*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(newFolder, newFile);
                var oldFile = Path.Combine(oldFolder, relativePath);
                var newInfo = new FileInfo(newFile);

                if (!File.Exists(oldFile))
                {
                    changes.Add(new FileChange
                    {
                        RelativePath = relativePath,
                        FullPath = newFile,
                        Status = ChangeStatus.New,
                        LastWriteTime = newInfo.LastWriteTime
                    });
                }
                else
                {
                    var oldInfo = new FileInfo(oldFile);

                    // Compare by size first (fast), then by content hash (accurate).
                    // Timestamps are intentionally ignored — copying files resets them on Windows.
                    if (newInfo.Length != oldInfo.Length || !FilesAreEqual(newFile, oldFile))
                    {
                        changes.Add(new FileChange
                        {
                            RelativePath  = relativePath,
                            FullPath      = newFile,
                            Status        = ChangeStatus.Modified,
                            LastWriteTime = newInfo.LastWriteTime
                        });
                    }
                    else
                    {
                        changes.Add(new FileChange
                        {
                            RelativePath  = relativePath,
                            FullPath      = newFile,
                            Status        = ChangeStatus.Unchanged,
                            LastWriteTime = newInfo.LastWriteTime,
                            IsSelected    = false
                        });
                    }
                }
            }

            // newest-first so the most recently touched files are visible at the top
            changes.Sort((a, b) => b.LastWriteTime.CompareTo(a.LastWriteTime));
            return changes;
        });
    }

    private static bool FilesAreEqual(string pathA, string pathB)
    {
        using var hashAlg = MD5.Create();
        using var streamA = File.OpenRead(pathA);
        using var streamB = File.OpenRead(pathB);
        var hashA = hashAlg.ComputeHash(streamA);
        hashAlg.Initialize();
        var hashB = hashAlg.ComputeHash(streamB);
        return hashA.SequenceEqual(hashB);
    }
}
