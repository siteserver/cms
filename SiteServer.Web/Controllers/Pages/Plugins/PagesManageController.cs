using System;
using System.Linq;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;

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
        public IHttpActionResult Get()
        {
            try
            {
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.PluginsPermissions.Add))
                {
                    return Unauthorized();
                }

                var dict = PluginManager.GetPluginIdAndVersionDict();
                var list = dict.Keys.ToList();
                var packageIds = TranslateUtils.ObjectCollectionToString(list);

                return Ok(new
                {
                    IsNightly = WebConfigUtils.IsNightlyUpdate,
                    SystemManager.PluginVersion,
                    AllPackages = PluginManager.AllPluginInfoList,
                    PackageIds = packageIds
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(RoutePluginId)]
        public IHttpActionResult Delete(string pluginId)
        {
            try
            {
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.PluginsPermissions.Add))
                {
                    return Unauthorized();
                }

                PluginManager.Delete(pluginId);
                rest.AddAdminLog("删除插件", $"插件:{pluginId}");

                CacheUtils.ClearAll();
                CacheDbUtils.Clear();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsReload)]
        public IHttpActionResult Reload()
        {
            try
            {
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.PluginsPermissions.Add))
                {
                    return Unauthorized();
                }

                CacheUtils.ClearAll();
                CacheDbUtils.Clear();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RoutePluginIdEnable)]
        public IHttpActionResult Enable(string pluginId)
        {
            try
            {
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.PluginsPermissions.Add))
                {
                    return Unauthorized();
                }

                var pluginInfo = PluginManager.GetPluginInfo(pluginId);
                if (pluginInfo != null)
                {
                    pluginInfo.IsDisabled = !pluginInfo.IsDisabled;
                    DataProvider.Plugin.UpdateIsDisabled(pluginId, pluginInfo.IsDisabled);
                    PluginManager.ClearCache();

                    rest.AddAdminLog(!pluginInfo.IsDisabled ? "禁用插件" : "启用插件", $"插件:{pluginId}");
                }

                CacheUtils.ClearAll();
                CacheDbUtils.Clear();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
