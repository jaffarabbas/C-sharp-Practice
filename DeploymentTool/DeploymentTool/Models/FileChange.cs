using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DeploymentTool.Models;

public enum ChangeStatus { New, Modified, Unchanged }

public class FileChange : INotifyPropertyChanged
{
    private bool _isSelected = true;

    public string RelativePath { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public ChangeStatus Status { get; set; }
    public DateTime LastWriteTime { get; set; }

    public string StatusLabel => Status switch
    {
        ChangeStatus.New       => "New",
        ChangeStatus.Modified  => "Modified",
        _                      => "Unchanged"
    };
    public string LastWriteTimeLabel => LastWriteTime.ToString("yyyy-MM-dd  HH:mm:ss");

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return;
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
