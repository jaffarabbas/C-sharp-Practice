# DeploymentTool Installer

This project builds an MSI installer for DeploymentTool.

## What it does

- Publishes the WPF app as `win-x64` self-contained
- Packages the publish output into an MSI
- Installs to `Program Files\DeploymentTool`
- Creates a Desktop shortcut to `DeploymentTool.exe`

## Build installer

From the repository root:

```powershell
dotnet build .\DeploymentTool.Installer\DeploymentTool.Installer.wixproj -c Release
```

The MSI output will be under:

- `DeploymentTool.Installer\bin\x64\Release\`

## Install on any PC

1. Copy the MSI file to the target PC.
2. Run the MSI as Administrator.
3. The app is installed and shortcut appears on Desktop.

## Notes

- This installer is currently built for `win-x64`.
- Update `Version` in `Product.wxs` for each new release.
