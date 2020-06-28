$pathRoot = $PSScriptRoot
$pathApp = $PSScriptRoot + '\bin\iis'
$pluginId = 'sscms.block'

# Stop the AppPool
New-Item -Path ($pathApp + '\app_offline.htm')

iisreset

iisreset

dotnet build -o $pathApp
Copy-Item -Path ($pathRoot + '\wwwroot') -Destination ($pathApp) -Recurse -Force
Copy-Item -Path ($pathRoot + '\Web.Debug.config') -Destination ($pathApp + '\Web.config')

dotnet publish ($PSScriptRoot + '\plugins\' + $pluginId) -o ($pathApp + '\plugins\' + $pluginId)

# Restart the AppPool
Remove-Item -Path ($pathApp + '\app_offline.htm')
