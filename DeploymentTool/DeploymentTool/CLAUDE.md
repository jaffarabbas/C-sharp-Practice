# CLAUDE.md

This repository contains a WPF desktop app for scanning, publishing, comparing, and patching .NET codebases.

## Working Rules

- Treat `ViewModels/MainViewModel.cs` as the main orchestration point for UI behavior.
- Keep process execution, file comparison, and file copying inside the service layer.
- Use `appsettings.json` as the source of truth for codebase paths and output roots.
- Avoid changes in generated folders such as `bin/` and `obj/`.
- Preserve the current MVVM structure unless the user explicitly asks for a broader refactor.

## Build And Run

- Build: `dotnet build DeploymentTool.sln`
- Run: `dotnet run --project DeploymentTool/DeploymentTool.csproj`

## Implementation Notes

- `ConfigService` loads settings from the app base directory, so `appsettings.json` must be copied to output.
- `ProjectScannerService` finds all `*.csproj` files under the selected codebase root.
- `PublishService` shells out to `dotnet publish` in Release mode.
- `FileCompareService` marks files as `New` or `Modified` by comparing the current publish output to a previous folder.
- `FileCopyService` creates a patch folder and copies only selected changes.

## UI Expectations

- Selected projects are published together.
- Compare is available whenever both the New and Old folder paths are set (works standalone, no prior publish required).
- The file-change filter should continue to respect the visible rows when selecting or clearing changes.