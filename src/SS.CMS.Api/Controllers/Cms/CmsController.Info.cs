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
        }

        [AllowAnonymous]
        [HttpGet(Route)]
        public async Task<ActionResult<InfoResponse>> GetInfo()
        {
            var isInstalled = !string.IsNullOrEmpty(_settingsManager.DatabaseConnectionString);

            var isContentRootPathWritable = false;
            var isWebRootPathWritable = false;

            if (!isInstalled)
            {
                try
                {
                    var filePath = PathUtils.Combine(_settingsManager.ContentRootPath, "version.txt");
                    await FileUtils.WriteTextAsync(filePath, _settingsManager.ProductVersion);

                    var ioPermission = new FileIOPermission(FileIOPermissionAccess.Write, _settingsManager.ContentRootPath);
                    ioPermission.Demand();

                    isContentRootPathWritable = true;
                }
                catch
                {
                    // ignored
                }

                try
                {
                    var filePath = PathUtils.Combine(_settingsManager.WebRootPath, "index.html");
                    await FileUtils.WriteTextAsync(filePath, Constants.Html5Empty);

                    var ioPermission = new FileIOPermission(FileIOPermissionAccess.Write, _settingsManager.WebRootPath);
                    ioPermission.Demand();

                    isWebRootPathWritable = true;
                }
                catch
                {
                    // ignored
                }
            }

            return new InfoResponse
            {
                IsInstalled = isInstalled,
                ApiUrl = PageUtils.Combine(Request.Host.Value, Constants.ApiPrefix),
                ApiServerName = Dns.GetHostName().ToUpper(), // 系统主机名
                ProductVersion = _settingsManager.ProductVersion,
                ContentRootPath = _settingsManager.ContentRootPath,
                WebRootPath = _settingsManager.WebRootPath,
                TargetFramework = _settingsManager.TargetFramework,
                IsContentRootPathWritable = isContentRootPathWritable,
                IsWebRootPathWritable = isWebRootPathWritable,
                RemoteIpAddress = PageUtils.GetIpAddress(Request.HttpContext.Connection.RemoteIpAddress.ToString()), // 访问IP
                PluginVersion = _settingsManager.PluginVersion, // Plugin 版本
                DatabaseType = _settingsManager.DatabaseType.Value, // 数据库类型
                Database = _configRepository.Database.DatabaseName // 数据库
            };
        }
    }
}
