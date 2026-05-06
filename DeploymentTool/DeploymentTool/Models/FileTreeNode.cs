using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DeploymentTool.Models;

public class FileTreeNode : INotifyPropertyChanged
{
    private bool? _isChecked = true;

    public string Name { get; init; } = string.Empty;
    public bool IsFolder { get; init; }
    public FileChange? FileChange { get; init; }
    public FileTreeNode? Parent { get; set; }
    public ObservableCollection<FileTreeNode> Children { get; } = [];

    public bool? IsChecked
    {
        get => _isChecked;
        set => SetIsChecked(value, updateChildren: true, updateParent: true);
    }

    public void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
    {
        if (_isChecked == value) return;
        _isChecked = value;
        OnPropertyChanged(nameof(IsChecked));

        if (!IsFolder && FileChange != null && value.HasValue)
            FileChange.IsSelected = value.Value;

        if (updateChildren && value.HasValue)
            foreach (var child in Children)
                child.SetIsChecked(value, updateChildren: true, updateParent: false);

        if (updateParent)
            Parent?.UpdateFromChildren();
    }

    internal void UpdateFromChildren()
    {
        if (Children.Count == 0) return;
        bool allChecked  = Children.All(c => c._isChecked == true);
        bool noneChecked = Children.All(c => c._isChecked == false);
        SetIsChecked(allChecked ? true : noneChecked ? false : null, updateChildren: false, updateParent: true);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
