$pathRoot = $PSScriptRoot
$pathApp = $PSScriptRoot + '\bin\Debug\netcoreapp3.1'
$pluginId = 'sscms.advertisement'

# Stop the AppPool
New-Item -Path ($pathApp + '\app_offline.htm')

iisreset

iisreset

dotnet build
Copy-Item -Path ($pathRoot + '\wwwroot') -Destination ($pathApp) -Recurse -Force
Copy-Item -Path ($pathRoot + '\Web.Debug.config') -Destination ($pathApp + '\Web.config')

dotnet build ($PSScriptRoot + '\plugins\' + $pluginId) -o ($pathApp + '\plugins\' + $pluginId)

# Restart the AppPool
Remove-Item -Path ($pathApp + '\app_offline.htm')
