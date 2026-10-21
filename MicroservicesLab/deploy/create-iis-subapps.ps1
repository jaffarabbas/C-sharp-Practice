# Office-style layout: ONE IIS site with the Razor portal as the main app and every other app
# as a sub-application (/gateway, /employee-api, ...). All apps read one server config folder.
# Run in PowerShell as Administrator. Safe to run again: existing pools/apps are left as they are.

param(
    [string]$DeployRoot = "C:\Deploy\MicroserviceTest2",
    [string]$SiteName = "MicroserviceTest2",
    [int]$Port = 8900
)

$ErrorActionPreference = "Stop"

$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole(
    [Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) { throw "Run this script in PowerShell as Administrator." }

if (-not (Test-Path "$env:windir\System32\inetsrv\aspnetcorev2.dll")) {
    Write-Warning "ASP.NET Core Hosting Bundle is not installed. Apps will return HTTP 500 until you install it from https://dotnet.microsoft.com/download/dotnet/10.0 and run iisreset."
}

Import-Module WebAdministration

$configDir = Join-Path $DeployRoot "SharedConfig"
$mainApp = "WebPortal"
# sub-application path -> published folder name
$subApps = [ordered]@{
    "gateway"      = "ApiGateway"
    "employee-api" = "Employee.Api"
    "payroll-api"  = "Payroll.Api"
    "leave-api"    = "Leave.Api"
}

function New-AppPool([string]$name) {
    $pool = "$SiteName-$name"
    if (-not (Test-Path "IIS:\AppPools\$pool")) {
        New-WebAppPool -Name $pool | Out-Null
        Set-ItemProperty "IIS:\AppPools\$pool" -Name managedRuntimeVersion -Value ""   # "No Managed Code"
        # Every app finds the server-wide config folder through this one variable.
        Add-WebConfigurationProperty -PSPath "MACHINE/WEBROOT/APPHOST" `
            -Filter "system.applicationHost/applicationPools/add[@name='$pool']/environmentVariables" `
            -Name "." -Value @{ name = "SHARED_CONFIG_DIR"; value = $configDir }
    }
    # Let the pool identity read the shared config folder.
    icacls $configDir /grant "IIS AppPool\${pool}:(OI)(CI)R" | Out-Null
    return $pool
}

if (-not (Test-Path $configDir)) { throw "$configDir not found. Publish first." }

# Main app = the site root
$mainPool = New-AppPool $mainApp
if (Get-Website -Name $SiteName) {
    Write-Host "$SiteName : site already exists, left unchanged"
} else {
    New-Website -Name $SiteName -Port $Port -PhysicalPath (Join-Path $DeployRoot $mainApp) -ApplicationPool $mainPool | Out-Null
    Write-Host "$SiteName : created on http://localhost:$Port/"
}

# Sub-applications
foreach ($path in $subApps.Keys) {
    $folder = Join-Path $DeployRoot $subApps[$path]
    if (-not (Test-Path $folder)) { Write-Warning "/$path : $folder not found, skipped."; continue }
    $pool = New-AppPool $subApps[$path]
    if (Get-WebApplication -Site $SiteName -Name $path) {
        Write-Host "/$path : already exists, left unchanged"
    } else {
        New-WebApplication -Site $SiteName -Name $path -PhysicalPath $folder -ApplicationPool $pool | Out-Null
        Write-Host "/$path : created"
    }
}

Write-Host "`nOpen http://localhost:$Port/ - the portal should list employee/payroll/leave as 200 OK."
Write-Host "Global settings: edit $configDir\appsettings.Shared.Production.json, then recycle the app pools."
