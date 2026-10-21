# Replaces the 5 separate IIS sites from the first deployment (C:\Deploy\MicroserviceTest) with the
# office layout: one site per deployment, WebPortal as the main app, everything else as sub-applications.
#   MicroserviceTest   -> http://localhost:8800/  (+ /gateway, /employee-api, /payroll-api, /leave-api)
#   MicroserviceTest2  -> http://localhost:8900/  (same)
# Run in PowerShell as Administrator. Safe to run again.

$ErrorActionPreference = "Stop"

$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole(
    [Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) { throw "Run this script in PowerShell as Administrator." }

Import-Module WebAdministration

$oldRoot = "C:\Deploy\MicroserviceTest"

# 1. Remove the old separate sites, but only the ones that still point at the first deployment's folders.
foreach ($name in "WebPortal", "ApiGateway", "Employee.Api", "Payroll.Api", "Leave.Api") {
    $site = Get-Website -Name $name
    if ($site) {
        $expected = Join-Path $oldRoot $name
        if ([Environment]::ExpandEnvironmentVariables($site.physicalPath).TrimEnd('\') -ne $expected) {
            Write-Warning "Site '$name' points to $($site.physicalPath), not $expected. Left untouched."
            continue
        }
        Remove-Website -Name $name
        Write-Host "Removed old site '$name'"
    }

    $poolInUse = (Get-Website | Where-Object applicationPool -eq $name) -or
                 (Get-WebApplication | Where-Object applicationPool -eq $name)
    if ((Test-Path "IIS:\AppPools\$name") -and -not $poolInUse) {
        Remove-WebAppPool -Name $name
        Write-Host "Removed old app pool '$name'"
    }
}

# 2. Build both deployments as main app + sub-applications.
& "$PSScriptRoot\create-iis-subapps.ps1" -DeployRoot $oldRoot -SiteName "MicroserviceTest" -Port 8800
& "$PSScriptRoot\create-iis-subapps.ps1" -DeployRoot "C:\Deploy\MicroserviceTest2" -SiteName "MicroserviceTest2" -Port 8900
