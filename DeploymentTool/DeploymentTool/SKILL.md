# DeploymentTool Skill

Use this project skill when working on the DeploymentTool WPF app.

## What The App Does

- Lets the user select one or more codebases from `appsettings.json`.
- Scans the selected root for `*.csproj` files.
- Publishes selected projects to a chosen output folder.
- Compares two publish folders and lists new or modified files.
- Copies selected file changes into a patch folder.

## Key Files

- `ViewModels/MainViewModel.cs`: main UI workflow and commands.
- `Services/ConfigService.cs`: loads `appsettings.json`.
- `Services/ProjectScannerService.cs`: finds projects under a codebase.
- `Services/PublishService.cs`: runs `dotnet publish` for selected projects.
- `Services/FileCompareService.cs`: compares old and new publish output.
- `Services/FileCopyService.cs`: creates patch folders from selected files.
- `Models/*.cs`: app settings and UI-facing data models.

## Common Workflows

### Add Or Update A Codebase

1. Edit `appsettings.json`.
2. Update the `Codebases` list with a display name and root path.
3. Keep publish and patch output roots valid on the local machine.

### Change Publish Or Compare Behavior

- Keep the UI state in the view model.
- Keep file system and process work inside services.
- Preserve the selected-file behavior when filters are active.

### Validate Changes

- Build the solution with `dotnet build DeploymentTool.sln`.
- Run the app and test scan, publish, compare, and patch creation paths.

## Constraints

- Do not edit files under `bin/` or `obj/`.
- Keep changes consistent with the existing MVVM structure.
- Prefer small, focused edits over broad refactors.