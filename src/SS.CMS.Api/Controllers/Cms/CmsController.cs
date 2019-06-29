using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.Cms
{
    [ApiController]
    [AllowAnonymous]
    [Route("cms")]
    public partial class CmsController : ControllerBase
    {
        public const string RouteInfo = "info";

        private readonly ISettingsManager _settingsManager;
        private readonly IUserManager _userManager;
        private readonly IConfigRepository _configRepository;

        public CmsController(ISettingsManager settingsManager, IUserManager userManager, IConfigRepository configRepository)
        {
            _settingsManager = settingsManager;
            _userManager = userManager;
            _configRepository = configRepository;
        }

        [Authorize]
        [HttpGet(RouteInfo)]
        public ActionResult<InfoResponse> GetInfo()
        {
            return new InfoResponse
            {
                ServerName = Dns.GetHostName().ToUpper(), // 系统主机名
                ContentRootPath = _settingsManager.ContentRootPath, // 系统根目录"
                WebRootPath = _settingsManager.WebRootPath, // 站点根目录
                AdminHostName = Request.Host.Value, // 后台域名
                RemoteIpAddress = PageUtils.GetIpAddress(Request.HttpContext.Connection.RemoteIpAddress.ToString()), // 访问IP
                TargetFramework = _settingsManager.TargetFramework, // .NET Core 版本
                ProductVersion = _settingsManager.ProductVersion, // CMS 版本
                PluginVersion = _settingsManager.PluginVersion, // Plugin 版本
                UpdateDate = _configRepository.Instance.UpdateDate, // 最近升级时间
                DatabaseType = _settingsManager.DatabaseType.Value, // 数据库类型
                Database = _configRepository.Database.DatabaseName // 数据库
            };
        }
    }
}