using System;
using System.Net;
using System.Security.Permissions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.Cms
{
    public partial class CmsController
    {
        public const string Route = "";

        public class InfoResponse
        {
            public bool IsInstalled { get; set; }
            public string ApiUrl { get; set; }
            public string ApiServerName { get; set; }
            public string ProductVersion { get; set; }
            public string DotNetVersion { get; set; }
            public string ContentRootPath { get; set; }
            public string WebRootPath { get; set; }
            public bool IsContentRootPathWritable { get; set; }
            public bool IsWebRootPathWritable { get; set; }
            public string TargetFramework { get; set; }
            public string RemoteIpAddress { get; set; }
            public string PluginVersion { get; set; }
            public DateTime? UpdateDate { get; set; }
            public string DatabaseType { get; set; }
            public string Database { get; set; }
            public string CacheType { get; set; }
        }

        [AllowAnonymous]
        [HttpGet(Route)]
        public ActionResult<InfoResponse> GetInfo()
        {
            var isInstalled = !string.IsNullOrEmpty(_settingsManager.DatabaseConnectionString);

            // var isContentRootPathWritable = false;
            // var isWebRootPathWritable = false;

            // if (!isInstalled)
            // {
            //     try
            //     {
            //         var filePath = PathUtils.Combine(_settingsManager.ContentRootPath, "version.txt");
            //         await FileUtils.WriteTextAsync(filePath, _settingsManager.ProductVersion);

            //         var ioPermission = new FileIOPermission(FileIOPermissionAccess.Write, _settingsManager.ContentRootPath);
            //         ioPermission.Demand();

            //         isContentRootPathWritable = true;
            //     }
            //     catch
            //     {
            //         // ignored
            //     }

            //     try
            //     {
            //         var filePath = PathUtils.Combine(_settingsManager.WebRootPath, "index.html");
            //         await FileUtils.WriteTextAsync(filePath, Constants.Html5Empty);

            //         var ioPermission = new FileIOPermission(FileIOPermissionAccess.Write, _settingsManager.WebRootPath);
            //         ioPermission.Demand();

            //         isWebRootPathWritable = true;
            //     }
            //     catch
            //     {
            //         // ignored
            //     }
            // }

            var apiUrl = PageUtils.Combine(Request.Host.Value, Constants.ApiPrefix);
            var apiServerName = Dns.GetHostName().ToUpper();
            var productVersion = _settingsManager.ProductVersion;
            var contentRootPath = _settingsManager.ContentRootPath;
            var webRootPath = _settingsManager.WebRootPath;
            var targetFramework = _settingsManager.TargetFramework;
            var remoteIpAddress = PageUtils.GetIpAddress(Request.HttpContext.Connection.RemoteIpAddress);
            var pluginVersion = _settingsManager.PluginVersion;
            var databaseType = _settingsManager.DatabaseType.Value;
            var database = _configRepository.Database.DatabaseName;
            var cacheType = _settingsManager.CacheType.Value;

            return new InfoResponse
            {
                IsInstalled = isInstalled,
                ApiUrl = apiUrl,
                ApiServerName = apiServerName,
                ProductVersion = productVersion,
                ContentRootPath = contentRootPath,
                WebRootPath = webRootPath,
                TargetFramework = targetFramework,
                RemoteIpAddress = remoteIpAddress,
                PluginVersion = pluginVersion,
                DatabaseType = databaseType,
                Database = database,
                CacheType = cacheType
            };
        }
    }
}
