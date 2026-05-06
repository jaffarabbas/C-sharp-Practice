namespace DeploymentTool.Models;

public class AppSettings
{
    public List<Codebase> Codebases { get; set; } = [];
    public string PublishOutputRoot { get; set; } = string.Empty;
    public string PatchOutputRoot { get; set; } = string.Empty;
}
