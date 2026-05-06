using System.ComponentModel;
using System.IO;

namespace DeploymentTool.Models;

public class AppSettingItem : INotifyPropertyChanged
{
    private string _value;
    private bool _isChanged;

    public AppSettingItem(string key, string value, string filePath, string serviceName)
    {
        Key         = key;
        _value      = value;
        FilePath    = filePath;
        ServiceName = serviceName;
    }

    public string Key         { get; }
    public string FilePath    { get; }
    public string ServiceName { get; }
    public string FileName    => Path.GetFileName(FilePath);

    public string Value
    {
        get => _value;
        set
        {
            if (_value == value) return;
            _value    = value;
            IsChanged = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
    }

    public bool IsChanged
    {
        get => _isChanged;
        set
        {
            if (_isChanged == value) return;
            _isChanged = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChanged)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
