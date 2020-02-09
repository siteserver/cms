using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Plugins
{
    
    [RoutePrefix("pages/plugins/manage")]
    public partial class PagesManageController : ApiController
    {
        private const string Route = "";
        private const string RoutePluginId = "{pluginId}";
        private const string RouteActionsReload = "actions/reload";
        private const string RoutePluginIdEnable = "{pluginId}/actions/enable";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
            {
                return Request.Unauthorized<GetResult>();
            }

            var dict = await PluginManager.GetPluginIdAndVersionDictAsync();
            var list = dict.Keys.ToList();
            var packageIds = Utilities.ToString(list);

            return new GetResult
            {
                IsNightly = WebConfigUtils.IsNightlyUpdate,
                PluginVersion = SystemManager.PluginVersion,
                AllPackages = await PluginManager.GetAllPluginInfoListAsync(),
                PackageIds = packageIds
            };
        }

        [HttpDelete, Route(RoutePluginId)]
        public async Task<BoolResult> Delete(string pluginId)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
            {
                return Request.Unauthorized<BoolResult>();
            }

            PluginManager.Delete(pluginId);
            await auth.AddAdminLogAsync("删除插件", $"插件:{pluginId}");

            CacheUtils.ClearAll();
            await DataProvider.DbCacheRepository.ClearAsync();

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteActionsReload)]
        public async Task<BoolResult> Reload()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
            {
                return Request.Unauthorized<BoolResult>();
            }

            CacheUtils.ClearAll();
            await DataProvider.DbCacheRepository.ClearAsync();

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RoutePluginIdEnable)]
        public async Task<BoolResult> Enable(string pluginId)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var pluginInfo = await PluginManager.GetPluginInfoAsync(pluginId);
            if (pluginInfo != null)
            {
                pluginInfo.IsDisabled = !pluginInfo.IsDisabled;
                await DataProvider.PluginRepository.UpdateIsDisabledAsync(pluginId, pluginInfo.IsDisabled);
                PluginManager.ClearCache();

                await auth.AddAdminLogAsync(!pluginInfo.IsDisabled ? "禁用插件" : "启用插件", $"插件:{pluginId}");
            }

            CacheUtils.ClearAll();
            await DataProvider.DbCacheRepository.ClearAsync();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
