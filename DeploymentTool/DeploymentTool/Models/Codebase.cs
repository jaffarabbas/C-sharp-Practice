namespace DeploymentTool.Models;

public class Codebase
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;

    public override string ToString() => Name;
}
