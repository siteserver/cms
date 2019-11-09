using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings.Utility
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/utilityParameters")]
    public class PagesUtilityParametersController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Utility))
                {
                    return Unauthorized();
                }

                var parameterList = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("系统主机名", Dns.GetHostName().ToUpper()),
                    new KeyValuePair<string, string>("系统根目录地址", WebConfigUtils.PhysicalApplicationPath),
                    new KeyValuePair<string, string>("系统程序目录地址", PathUtils.PhysicalSiteServerPath),
                    new KeyValuePair<string, string>("域名", PageUtils.GetHost()),
                    new KeyValuePair<string, string>("访问IP", PageUtils.GetIpAddress()),
                    new KeyValuePair<string, string>(".NET 框架", SystemManager.TargetFramework),
                    new KeyValuePair<string, string>(".NET CLR 版本", SystemManager.EnvironmentVersion),
                    new KeyValuePair<string, string>("SiteServer CMS 版本", SystemManager.ProductVersion),
                    new KeyValuePair<string, string>("SiteServer.Plugin 版本", SystemManager.PluginVersion),
                    new KeyValuePair<string, string>("最近升级时间", DateUtils.GetDateAndTimeString(ConfigManager.Instance.UpdateDate)),
                    new KeyValuePair<string, string>("数据库类型", WebConfigUtils.DatabaseType.Value),
                    new KeyValuePair<string, string>("数据库名称", SqlUtils.GetDatabaseNameFormConnectionString(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
                };

                return Ok(new
                {
                    Value = parameterList
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
