using System.ComponentModel;
using System.IO;

namespace DeploymentTool.Models;

public class ConfigFileItem : INotifyPropertyChanged
{
    private bool _isSelected;
    private bool _isModified;

    public ConfigFileItem(string filePath, string serviceName)
    {
        FilePath    = filePath;
        ServiceName = serviceName;
        FileName    = Path.GetFileName(filePath);
        FileType    = Path.GetExtension(filePath).Equals(".json", StringComparison.OrdinalIgnoreCase)
                      ? "JSON" : "XML";
    }

    public string FileName    { get; }
    public string ServiceName { get; }
    public string FilePath    { get; }
    public string FileType    { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return;
            _isSelected = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
        }
    }

    public bool IsModified
    {
        get => _isModified;
        set
        {
            if (_isModified == value) return;
            _isModified = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsModified)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
