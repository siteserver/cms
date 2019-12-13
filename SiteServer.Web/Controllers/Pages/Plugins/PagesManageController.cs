using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Plugins
{
    
    [RoutePrefix("pages/plugins/manage")]
    public class PagesManageController : ApiController
    {
        private const string Route = "";
        private const string RoutePluginId = "{pluginId}";
        private const string RouteActionsReload = "actions/reload";
        private const string RoutePluginIdEnable = "{pluginId}/actions/enable";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
                {
                    return Unauthorized();
                }

                var dict = await PluginManager.GetPluginIdAndVersionDictAsync();
                var list = dict.Keys.ToList();
                var packageIds = TranslateUtils.ObjectCollectionToString(list);

                return Ok(new
                {
                    IsNightly = WebConfigUtils.IsNightlyUpdate,
                    SystemManager.PluginVersion,
                    AllPackages = await PluginManager.GetAllPluginInfoListAsync(),
                    PackageIds = packageIds
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(RoutePluginId)]
        public async Task<IHttpActionResult> Delete(string pluginId)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
                {
                    return Unauthorized();
                }

                PluginManager.Delete(pluginId);
                await request.AddAdminLogAsync("删除插件", $"插件:{pluginId}");

                CacheUtils.ClearAll();
                await DataProvider.DbCacheRepository.ClearAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsReload)]
        public async Task<IHttpActionResult> Reload()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
                {
                    return Unauthorized();
                }

                CacheUtils.ClearAll();
                await DataProvider.DbCacheRepository.ClearAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RoutePluginIdEnable)]
        public async Task<IHttpActionResult> Enable(string pluginId)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
                {
                    return Unauthorized();
                }

                var pluginInfo = await PluginManager.GetPluginInfoAsync(pluginId);
                if (pluginInfo != null)
                {
                    pluginInfo.IsDisabled = !pluginInfo.IsDisabled;
                    await DataProvider.PluginRepository.UpdateIsDisabledAsync(pluginId, pluginInfo.IsDisabled);
                    PluginManager.ClearCache();

                    await request.AddAdminLogAsync(!pluginInfo.IsDisabled ? "禁用插件" : "启用插件", $"插件:{pluginId}");
                }

                CacheUtils.ClearAll();
                await DataProvider.DbCacheRepository.ClearAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
