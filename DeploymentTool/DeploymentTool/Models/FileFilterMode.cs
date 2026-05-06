namespace DeploymentTool.Models;

public enum FileFilterMode { All, ModifiedOnly, NewOnly }

public record FilterOption(FileFilterMode Mode, string Label);
