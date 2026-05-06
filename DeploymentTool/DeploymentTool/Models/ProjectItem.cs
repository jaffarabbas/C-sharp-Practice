using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DeploymentTool.Models;

public class ProjectItem : INotifyPropertyChanged
{
    private bool _isSelected;

    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;

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
