# DeploymentTool Context

DeploymentTool is a .NET 8 WPF app that helps compare publish outputs and build patch folders for selected projects.

## User Flow

1. Load codebase definitions from `appsettings.json`.
2. Select a codebase to scan for projects.
3. Pick one or more projects to publish.
4. Publish them to a target output folder.
5. Compare the new output against an older publish folder.
6. Select changed files and copy them into a patch folder.

## Architecture

- `MainWindow.xaml` and `MainWindow.xaml.cs` provide the UI shell.
- `ViewModels/MainViewModel.cs` owns the commands, state, filtering, and log text.
- `Services/` contains all file system and process logic.
- `Models/` contains simple view and configuration models.

## Configuration Shape

- `Codebases`: a list of named root paths to scan for projects.
- `PublishOutputRoot`: base folder for publish output.
- `PatchOutputRoot`: base folder for generated patches.

## Important Behavior

- The file change list is filtered to show only modified files by default.
- Selecting and clearing changes should operate on the visible rows, not the hidden ones.
- Compare sorts results newest first so recent changes stay near the top.
- Patch creation copies the selected files while preserving relative paths.

## Notes For Future Changes

- Keep UI state changes in the view model.
- Keep external process calls and file I/O in services.
- If the compare logic changes, confirm that selection, sorting, and filter counts still behave as expected.