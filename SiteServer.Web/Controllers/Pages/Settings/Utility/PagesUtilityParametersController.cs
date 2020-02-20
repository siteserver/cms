using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Repositories;
using Datory;
using SiteServer.API.Context;
using SiteServer.CMS.Framework;

namespace SiteServer.API.Controllers.Pages.Settings.Utility
{
    
    [RoutePrefix("pages/settings/utilityParameters")]
    public class PagesUtilityParametersController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUtilityParameters))
                {
                    return Unauthorized();
                }

                var config = await DataProvider.ConfigRepository.GetAsync();

                var parameterList = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("系统主机名", Dns.GetHostName().ToUpper()),
                    new KeyValuePair<string, string>("系统根目录地址", WebConfigUtils.PhysicalApplicationPath),
                    new KeyValuePair<string, string>("系统程序目录地址", PathUtility.PhysicalSiteServerPath),
                    new KeyValuePair<string, string>(".NET 框架", SystemManager.TargetFramework),
                    new KeyValuePair<string, string>(".NET CLR 版本", SystemManager.EnvironmentVersion),
                    new KeyValuePair<string, string>("SiteServer CMS 版本", SystemManager.ProductVersion),
                    new KeyValuePair<string, string>("SiteServer.Abstractions 版本", SystemManager.PluginVersion),
                    new KeyValuePair<string, string>("最近升级时间", DateUtils.GetDateAndTimeString(config.UpdateDate)),
                    new KeyValuePair<string, string>("数据库类型", WebConfigUtils.DatabaseType.GetValue()),
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
