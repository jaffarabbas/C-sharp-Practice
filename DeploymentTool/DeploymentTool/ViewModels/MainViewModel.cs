using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using DeploymentTool.Helpers;
using DeploymentTool.Models;
using DeploymentTool.Services;

namespace DeploymentTool.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly ConfigService _config;
    private readonly ProjectScannerService _scanner;
    private readonly PublishService _publisher;
    private readonly FileCompareService _comparer;
    private readonly FileCopyService _copier;
    private readonly AppSettingsService _appSettingsService;

    private Codebase? _selectedCodebase;
    private string _publishOutputFolder = string.Empty;
    private string _logText = string.Empty;
    private string _logSearchText = string.Empty;
    private bool _isBusy;
    private string _newCompareFolder = string.Empty;
    private string _oldCompareFolder = string.Empty;
    private string _searchText = string.Empty;
    private string _patchName = string.Empty;
    private FilterOption _selectedFilterOption;
    private ConfigFileItem? _selectedConfigFile;
    private string _editorContent = string.Empty;
    private bool _isEditorDirty;
    private bool _loadingFile;
    private string _globalKey = string.Empty;
    private string _globalValue = string.Empty;
    private readonly Dictionary<string, string> _fileContents = new(StringComparer.OrdinalIgnoreCase);

    // ── Collections ────────────────────────────────────────────────────────────
    public ObservableCollection<Codebase>      Codebases   { get; } = [];
    public ObservableCollection<ProjectItem>   Projects    { get; } = [];
    public ObservableCollection<FileChange>    FileChanges { get; } = [];
    public ObservableCollection<FileTreeNode>  FileTree    { get; } = [];
    public ObservableCollection<ConfigFileItem> ConfigFiles { get; } = [];

    // ── Properties ─────────────────────────────────────────────────────────────
    public Codebase? SelectedCodebase
    {
        get => _selectedCodebase;
        set
        {
            if (SetField(ref _selectedCodebase, value) && value != null)
                _ = LoadProjectsAsync();
        }
    }

    public string PublishOutputFolder
    {
        get => _publishOutputFolder;
        set => SetField(ref _publishOutputFolder, value);
    }

    public string LogText
    {
        get => _logText;
        set
        {
            if (SetField(ref _logText, value))
                OnPropertyChanged(nameof(FilteredLogText));
        }
    }

    public string LogSearchText
    {
        get => _logSearchText;
        set
        {
            if (SetField(ref _logSearchText, value))
                OnPropertyChanged(nameof(FilteredLogText));
        }
    }

    public string FilteredLogText
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_logSearchText))
                return _logText;

            return string.Join("\n",
                _logText.Split('\n')
                        .Where(line => line.Contains(_logSearchText, StringComparison.OrdinalIgnoreCase)));
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetField(ref _isBusy, value))
                Application.Current.Dispatcher.InvokeAsync(CommandManager.InvalidateRequerySuggested);
        }
    }

    public string NewCompareFolder
    {
        get => _newCompareFolder;
        set
        {
            if (SetField(ref _newCompareFolder, value))
                Application.Current.Dispatcher.InvokeAsync(CommandManager.InvalidateRequerySuggested);
        }
    }

    public string OldCompareFolder
    {
        get => _oldCompareFolder;
        set
        {
            if (SetField(ref _oldCompareFolder, value))
                Application.Current.Dispatcher.InvokeAsync(CommandManager.InvalidateRequerySuggested);
        }
    }

    // ── Filter & Search ────────────────────────────────────────────────────────
    public IReadOnlyList<FilterOption> FilterOptions { get; }

    public FilterOption SelectedFilterOption
    {
        get => _selectedFilterOption;
        set
        {
            if (SetField(ref _selectedFilterOption, value) && FileChanges.Count > 0)
                BuildFileTree();
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetField(ref _searchText, value) && FileChanges.Count > 0)
                BuildFileTree();
        }
    }

    // ── Patch config ───────────────────────────────────────────────────────────
    public string PatchOutputRoot => _config.Settings.PatchOutputRoot;

    public string PatchName
    {
        get => _patchName;
        set
        {
            if (SetField(ref _patchName, value))
                Application.Current.Dispatcher.InvokeAsync(CommandManager.InvalidateRequerySuggested);
        }
    }

    // ── AppSettings tab properties ─────────────────────────────────────────────
    public ConfigFileItem? SelectedConfigFile
    {
        get => _selectedConfigFile;
        set
        {
            if (_selectedConfigFile == value) return;

            // Persist any unsaved editor edits to the in-memory cache before switching
            if (_selectedConfigFile != null && _isEditorDirty)
            {
                _fileContents[_selectedConfigFile.FilePath] = _editorContent;
                _selectedConfigFile.IsModified = true;
            }

            SetField(ref _selectedConfigFile, value);
            OnPropertyChanged(nameof(EditorTitle));
            if (value != null)
                _ = LoadFileContentAsync(value);
        }
    }

    public string GlobalKey
    {
        get => _globalKey;
        set
        {
            if (SetField(ref _globalKey, value))
                Application.Current.Dispatcher.InvokeAsync(CommandManager.InvalidateRequerySuggested);
        }
    }

    public string GlobalValue
    {
        get => _globalValue;
        set => SetField(ref _globalValue, value);
    }

    public string EditorContent
    {
        get => _editorContent;
        set
        {
            if (SetField(ref _editorContent, value) && !_loadingFile && !_isEditorDirty)
            {
                _isEditorDirty = true;
                OnPropertyChanged(nameof(IsEditorDirty));
                Application.Current.Dispatcher.InvokeAsync(CommandManager.InvalidateRequerySuggested);
            }
        }
    }

    public bool IsEditorDirty
    {
        get => _isEditorDirty;
        private set
        {
            if (SetField(ref _isEditorDirty, value))
                Application.Current.Dispatcher.InvokeAsync(CommandManager.InvalidateRequerySuggested);
        }
    }

    public string EditorTitle => _selectedConfigFile?.FileName ?? "(no file selected)";

    // ── Counts ─────────────────────────────────────────────────────────────────
    public int ModifiedCount => FileChanges.Count(f => f.Status == ChangeStatus.Modified);
    public int NewCount      => FileChanges.Count(f => f.Status == ChangeStatus.New);
    public int TotalCount    => FileChanges.Count;

    // ── Commands ────────────────────────────────────────────────────────────────
    public ICommand BrowseOutputFolderCommand     { get; }
    public ICommand BrowseNewCompareFolderCommand { get; }
    public ICommand BrowseOldCompareFolderCommand { get; }
    public ICommand SelectAllProjectsCommand      { get; }
    public ICommand ClearProjectsCommand          { get; }
    public ICommand PublishCommand                { get; }
    public ICommand CompareCommand                { get; }
    public ICommand SelectAllChangesCommand       { get; }
    public ICommand ClearChangesCommand           { get; }
    public ICommand CreatePatchCommand            { get; }

    // AppSettings tab
    public ICommand LoadConfigFilesCommand   { get; }
    public ICommand SelectAllFilesCommand    { get; }
    public ICommand ClearFilesCommand        { get; }
    public ICommand ApplyGlobalChangeCommand { get; }
    public ICommand SaveFileCommand          { get; }
    public ICommand SaveAllModifiedCommand   { get; }

    // ── Constructor ─────────────────────────────────────────────────────────────
    public MainViewModel()
    {
        _config              = new ConfigService();
        _scanner             = new ProjectScannerService();
        _publisher           = new PublishService();
        _comparer            = new FileCompareService();
        _copier              = new FileCopyService();
        _appSettingsService  = new AppSettingsService();

        _publishOutputFolder = _config.Settings.PublishOutputRoot;

        FilterOptions = [
            new FilterOption(FileFilterMode.All,          "All Files"),
            new FilterOption(FileFilterMode.ModifiedOnly, "Modified Only"),
            new FilterOption(FileFilterMode.NewOnly,      "New Files Only"),
        ];
        _selectedFilterOption = FilterOptions[0];
        _patchName = $"Patch_{DateTime.Now:yyyyMMdd_HHmm}";

        FileChanges.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(ModifiedCount));
            OnPropertyChanged(nameof(NewCount));
            OnPropertyChanged(nameof(TotalCount));
        };

        ConfigFiles.CollectionChanged += (_, _) =>
            Application.Current.Dispatcher.InvokeAsync(CommandManager.InvalidateRequerySuggested);

        BrowseOutputFolderCommand     = new RelayCommand(BrowseOutputFolder);
        BrowseNewCompareFolderCommand = new RelayCommand(() => BrowseCompareFolder(isNew: true));
        BrowseOldCompareFolderCommand = new RelayCommand(() => BrowseCompareFolder(isNew: false));
        SelectAllProjectsCommand      = new RelayCommand(() => SetAllProjects(true));
        ClearProjectsCommand          = new RelayCommand(() => SetAllProjects(false));
        SelectAllChangesCommand       = new RelayCommand(() => SetAllChanges(true));
        ClearChangesCommand           = new RelayCommand(() => SetAllChanges(false));

        PublishCommand = new RelayCommand(
            async () =>
            {
                try { await PublishAsync(); }
                catch (Exception ex) { AppendLog($"FATAL: {ex.Message}"); IsBusy = false; }
            },
            () => !IsBusy);

        CompareCommand = new RelayCommand(
            async () =>
            {
                try { await CompareAsync(); }
                catch (Exception ex) { AppendLog($"FATAL: {ex.Message}"); IsBusy = false; }
            },
            () => !string.IsNullOrWhiteSpace(_newCompareFolder) && !string.IsNullOrWhiteSpace(_oldCompareFolder) && !IsBusy);

        CreatePatchCommand = new RelayCommand(
            async () =>
            {
                try { await CreatePatchAsync(); }
                catch (Exception ex) { AppendLog($"FATAL: {ex.Message}"); IsBusy = false; }
            },
            () => !IsBusy && !string.IsNullOrWhiteSpace(_patchName) && FileChanges.Any(f => f.IsSelected));

        LoadConfigFilesCommand = new RelayCommand(
            async () =>
            {
                try { await LoadConfigFilesAsync(); }
                catch (Exception ex) { AppendLog($"FATAL: {ex.Message}"); IsBusy = false; }
            },
            () => !IsBusy);

        SelectAllFilesCommand = new RelayCommand(
            () => { foreach (var f in ConfigFiles) f.IsSelected = true; },
            () => ConfigFiles.Count > 0);

        ClearFilesCommand = new RelayCommand(
            () => { foreach (var f in ConfigFiles) f.IsSelected = false; },
            () => ConfigFiles.Count > 0);

        ApplyGlobalChangeCommand = new RelayCommand(
            async () =>
            {
                try { await ApplyGlobalChangeAsync(); }
                catch (Exception ex) { AppendLog($"FATAL: {ex.Message}"); IsBusy = false; }
            },
            () => !IsBusy && !string.IsNullOrWhiteSpace(_globalKey) && ConfigFiles.Any(f => f.IsSelected));

        SaveFileCommand = new RelayCommand(
            async () =>
            {
                try { await SaveFileAsync(); }
                catch (Exception ex) { AppendLog($"FATAL: {ex.Message}"); IsBusy = false; }
            },
            () => !IsBusy && _selectedConfigFile != null && (_isEditorDirty || _selectedConfigFile?.IsModified == true));

        SaveAllModifiedCommand = new RelayCommand(
            async () =>
            {
                try { await SaveAllModifiedAsync(); }
                catch (Exception ex) { AppendLog($"FATAL: {ex.Message}"); IsBusy = false; }
            },
            () => !IsBusy && ConfigFiles.Any(f => f.IsModified));

        foreach (var cb in _config.Settings.Codebases)
            Codebases.Add(cb);
    }

    // ── Private methods ─────────────────────────────────────────────────────────
    private async Task LoadProjectsAsync()
    {
        if (SelectedCodebase == null) return;

        IsBusy = true;
        Projects.Clear();
        AppendLog($"Scanning: {SelectedCodebase.Path}");

        try
        {
            var found = await _scanner.ScanAsync(SelectedCodebase.Path);
            foreach (var p in found)
                Projects.Add(p);
            AppendLog($"Found {found.Count} project(s).");
        }
        catch (Exception ex)
        {
            AppendLog($"ERROR scanning: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task PublishAsync()
    {
        var selected = Projects.Where(p => p.IsSelected).ToList();

        if (selected.Count == 0)
        {
            MessageBox.Show("Select at least one project.", "No Selection",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(PublishOutputFolder))
        {
            MessageBox.Show("Specify a publish output folder.", "Missing Folder",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        IsBusy = true;
        FileChanges.Clear();
        FileTree.Clear();
        AppendLog($"Publishing {selected.Count} project(s) to: {PublishOutputFolder}");

        try
        {
            await _publisher.PublishAsync(selected, PublishOutputFolder, AppendLog);
            NewCompareFolder = PublishOutputFolder;
            AppendLog("Publish completed.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CompareAsync()
    {
        if (!Directory.Exists(NewCompareFolder))
        {
            MessageBox.Show(
                "Set the NEW folder path first (the folder you just published to).",
                "New Folder Missing", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!Directory.Exists(OldCompareFolder))
        {
            MessageBox.Show(
                "Set the OLD folder path first (the previous version you are comparing against).",
                "Old Folder Missing", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        IsBusy = true;
        FileChanges.Clear();
        FileTree.Clear();
        AppendLog($"Comparing:\n  OLD: {OldCompareFolder}\n  NEW: {NewCompareFolder}");

        try
        {
            var changes = await _comparer.CompareAsync(OldCompareFolder, NewCompareFolder);
            foreach (var c in changes)
                FileChanges.Add(c);

            var modCount = changes.Count(c => c.Status == ChangeStatus.Modified);
            var newCount = changes.Count(c => c.Status == ChangeStatus.New);
            AppendLog($"Compare done — {modCount} modified  |  {newCount} new  |  {changes.Count} total");

            PatchName = $"Patch_{DateTime.Now:yyyyMMdd_HHmm}";
            BuildFileTree();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void BuildFileTree()
    {
        FileTree.Clear();

        var changes = _selectedFilterOption.Mode switch
        {
            FileFilterMode.ModifiedOnly => FileChanges.Where(f => f.Status == ChangeStatus.Modified).ToList(),
            FileFilterMode.NewOnly      => FileChanges.Where(f => f.Status == ChangeStatus.New).ToList(),
            _                           => FileChanges.ToList()
        };

        if (!string.IsNullOrWhiteSpace(_searchText))
            changes = changes
                .Where(f => f.RelativePath.Contains(_searchText, StringComparison.OrdinalIgnoreCase))
                .ToList();

        var nodeMap = new Dictionary<string, FileTreeNode>(StringComparer.OrdinalIgnoreCase);

        foreach (var fc in changes)
        {
            fc.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(FileChange.IsSelected))
                    Application.Current.Dispatcher.InvokeAsync(CommandManager.InvalidateRequerySuggested);
            };

            var parts = fc.RelativePath.Replace('\\', '/').Split('/');
            FileTreeNode? parent = null;

            for (int i = 0; i < parts.Length; i++)
            {
                bool isLeaf = i == parts.Length - 1;
                string key  = string.Join("/", parts[..(i + 1)]);

                if (!nodeMap.TryGetValue(key, out var node))
                {
                    node = new FileTreeNode
                    {
                        Name       = parts[i],
                        IsFolder   = !isLeaf,
                        FileChange = isLeaf ? fc : null,
                        Parent     = parent
                    };
                    nodeMap[key] = node;

                    if (parent == null) FileTree.Add(node);
                    else                parent.Children.Add(node);
                }

                parent = node;
            }
        }

        foreach (var root in FileTree)
            SyncFolderState(root);

        Application.Current.Dispatcher.InvokeAsync(CommandManager.InvalidateRequerySuggested);
    }

    private static void SyncFolderState(FileTreeNode node)
    {
        if (!node.IsFolder) return;
        foreach (var child in node.Children)
            SyncFolderState(child);
        node.UpdateFromChildren();
    }

    private async Task CreatePatchAsync()
    {
        var selected = FileChanges.Where(f => f.IsSelected).ToList();

        if (selected.Count == 0)
        {
            MessageBox.Show("No files selected for the patch.", "No Selection",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var patchFolder = Path.Combine(_config.Settings.PatchOutputRoot, PatchName.Trim());

        IsBusy = true;
        AppendLog($"Creating patch: {patchFolder}");

        try
        {
            await _copier.CreatePatchAsync(selected, patchFolder);
            AppendLog($"Patch created with {selected.Count} file(s):\n  {patchFolder}");
            MessageBox.Show($"Patch ready:\n{patchFolder}", "Patch Created",
                MessageBoxButton.OK, MessageBoxImage.Information);
            PatchName = $"Patch_{DateTime.Now:yyyyMMdd_HHmm}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void BrowseOutputFolder()
    {
        var dlg = new Microsoft.Win32.OpenFolderDialog { Title = "Select Publish Output Folder" };
        if (dlg.ShowDialog() == true)
            PublishOutputFolder = dlg.FolderName;
    }

    private void BrowseCompareFolder(bool isNew)
    {
        var title = isNew
            ? "Select NEW Publish Folder (what you just built)"
            : "Select OLD Folder (previous version to compare against)";

        var dlg = new Microsoft.Win32.OpenFolderDialog { Title = title };
        if (dlg.ShowDialog() != true) return;

        if (isNew) NewCompareFolder = dlg.FolderName;
        else       OldCompareFolder = dlg.FolderName;
    }

    private void SetAllProjects(bool selected)
    {
        foreach (var p in Projects) p.IsSelected = selected;
    }

    private void SetAllChanges(bool selected)
    {
        foreach (var root in FileTree)
            root.IsChecked = selected;
        Application.Current.Dispatcher.InvokeAsync(CommandManager.InvalidateRequerySuggested);
    }

    // ── AppSettings tab ─────────────────────────────────────────────────────────

    private async Task LoadConfigFilesAsync()
    {
        var selected = Projects.Where(p => p.IsSelected).ToList();
        if (selected.Count == 0)
        {
            MessageBox.Show("Select at least one project on the Publish tab first.",
                "No Projects Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        IsBusy = true;

        foreach (var f in ConfigFiles)
            f.PropertyChanged -= OnConfigFileItemChanged;

        ConfigFiles.Clear();
        _fileContents.Clear();

        // Reset editor
        _loadingFile  = true;
        EditorContent = string.Empty;
        _loadingFile  = false;
        SelectedConfigFile = null;
        IsEditorDirty = false;

        AppendLog($"Scanning config files in {selected.Count} project(s)…");

        try
        {
            var files = await _appSettingsService.LoadConfigFilesAsync(selected);
            foreach (var f in files)
            {
                f.PropertyChanged += OnConfigFileItemChanged;
                ConfigFiles.Add(f);
            }

            if (files.Count == 0)
            {
                AppendLog("No appsettings.json or web.config files found.");
                MessageBox.Show("No config files were found in the selected projects.",
                    "Nothing Found", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                AppendLog($"Found {files.Count} config file(s).");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadFileContentAsync(ConfigFileItem file)
    {
        IsBusy = true;
        try
        {
            // Use cached content if available (preserves in-memory edits / global changes)
            if (!_fileContents.TryGetValue(file.FilePath, out var content))
            {
                content = await File.ReadAllTextAsync(file.FilePath, System.Text.Encoding.UTF8);
                _fileContents[file.FilePath] = content;
            }

            _loadingFile  = true;
            EditorContent = content;
            _loadingFile  = false;
            IsEditorDirty = false;

            AppendLog($"Opened: {file.FilePath}");
        }
        catch (Exception ex)
        {
            AppendLog($"ERROR opening {file.FileName}: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveFileAsync()
    {
        var file = _selectedConfigFile;
        if (file == null) return;

        // Push current editor content to cache before saving
        var content = _editorContent;
        _fileContents[file.FilePath] = content;

        IsBusy = true;
        AppendLog($"Saving {file.FileName}…");

        try
        {
            await File.WriteAllTextAsync(file.FilePath, content,
                new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            file.IsModified = false;
            IsEditorDirty   = false;
            AppendLog($"Saved: {file.FilePath}");
        }
        catch (Exception ex)
        {
            AppendLog($"ERROR saving {file.FileName}: {ex.Message}");
            MessageBox.Show($"Could not save file:\n{ex.Message}", "Save Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ApplyGlobalChangeAsync()
    {
        var targets = ConfigFiles.Where(f => f.IsSelected).ToList();
        if (targets.Count == 0)
        {
            MessageBox.Show("Check at least one file in the list before applying.",
                "No Files Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(_globalKey))
        {
            MessageBox.Show("Enter a Global Key to search for.",
                "Missing Key", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Persist any in-progress editor edits to cache first
        if (_selectedConfigFile != null && _isEditorDirty)
        {
            _fileContents[_selectedConfigFile.FilePath] = _editorContent;
            _selectedConfigFile.IsModified = true;
        }

        IsBusy = true;
        int changedFiles = 0;
        AppendLog($"Applying '{_globalKey}' = '{_globalValue}' to {targets.Count} selected file(s)…");

        try
        {
            foreach (var file in targets)
            {
                if (!_fileContents.TryGetValue(file.FilePath, out var content))
                {
                    try
                    {
                        content = await File.ReadAllTextAsync(file.FilePath, System.Text.Encoding.UTF8);
                        _fileContents[file.FilePath] = content;
                    }
                    catch (Exception ex)
                    {
                        AppendLog($"ERROR reading {file.FileName}: {ex.Message}");
                        continue;
                    }
                }

                var updated = ApplyKeyChange(file, content, _globalKey, _globalValue);
                if (updated == content) continue;

                _fileContents[file.FilePath] = updated;
                file.IsModified = true;
                changedFiles++;

                // Refresh editor if this is the currently open file
                if (string.Equals(_selectedConfigFile?.FilePath, file.FilePath, StringComparison.OrdinalIgnoreCase))
                {
                    _loadingFile  = true;
                    EditorContent = updated;
                    _loadingFile  = false;
                    IsEditorDirty = false;
                }
            }

            if (changedFiles > 0)
                AppendLog($"'{_globalKey}' updated in {changedFiles} file(s). Use Save File / Save All to persist.");
            else
            {
                AppendLog($"Key '{_globalKey}' not found in any selected file.");
                MessageBox.Show($"Key '{_globalKey}' was not found in any of the {targets.Count} selected file(s).",
                    "No Matches", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveAllModifiedAsync()
    {
        // Flush editor edits to cache first
        if (_selectedConfigFile != null && _isEditorDirty)
        {
            _fileContents[_selectedConfigFile.FilePath] = _editorContent;
            _selectedConfigFile.IsModified = true;
        }

        var modified = ConfigFiles.Where(f => f.IsModified).ToList();
        if (modified.Count == 0)
        {
            MessageBox.Show("No modified files to save.", "Nothing to Save",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        IsBusy = true;
        AppendLog($"Saving {modified.Count} modified file(s)…");
        int saved = 0;

        try
        {
            foreach (var file in modified)
            {
                if (!_fileContents.TryGetValue(file.FilePath, out var content))
                    continue;

                try
                {
                    await File.WriteAllTextAsync(file.FilePath, content,
                        new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                    file.IsModified = false;
                    saved++;
                }
                catch (Exception ex)
                {
                    AppendLog($"ERROR saving {file.FileName}: {ex.Message}");
                }
            }

            // If the open file was just saved, clear the dirty flag
            if (_selectedConfigFile != null && !_selectedConfigFile.IsModified)
                IsEditorDirty = false;

            AppendLog($"Saved {saved}/{modified.Count} file(s).");
            MessageBox.Show($"Saved {saved} file(s) successfully.", "Save Complete",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static string ApplyKeyChange(ConfigFileItem file, string content, string key, string value)
        => file.FileType switch
        {
            "JSON" => JsonFlattenHelper.ApplyChange(content, key, value),
            "XML"  => XmlConfigHelper.ApplyChange(content, key, value),
            _      => content
        };

    private void OnConfigFileItemChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ConfigFileItem.IsModified) or nameof(ConfigFileItem.IsSelected))
            Application.Current.Dispatcher.InvokeAsync(CommandManager.InvalidateRequerySuggested);
    }

    private void AppendLog(string message)
    {
        Application.Current.Dispatcher.Invoke(() =>
            LogText += $"[{DateTime.Now:HH:mm:ss}] {message}\n");
    }
}
