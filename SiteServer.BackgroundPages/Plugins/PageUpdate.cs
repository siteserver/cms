using System;
using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Controllers.Sys.Packaging;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Plugins
{
    public class PageUpdate : BasePage
    {
        public static string GetRedirectUrl()
        {
            return PageUtils.GetPluginsUrl(nameof(PageUpdate), null);
        }

        public string AdminUrl => PageUtils.GetAdminDirectoryUrl(string.Empty);

        public string Packages
        {
            get
            {
                var list = new List<object>();

                var dict = PluginManager.GetPluginIdAndVersionDict();

                foreach (var pluginId in dict.Keys)
                {
                    var version = dict[pluginId];

                    var versionAndNotes = new
                    {
                        Id = pluginId,
                        Version = version
                    };

                    list.Add(versionAndNotes);
                }

                return TranslateUtils.JsonSerialize(list);
            }
        }

        public string PackageIds
        {
            get
            {
                var dict = PluginManager.GetPluginIdAndVersionDict();

                var list = dict.Keys.ToList();

                return TranslateUtils.ObjectCollectionToString(list);
            }
        }

        public string DownloadApiUrl => ApiRouteDownload.GetUrl(PageUtility.InnerApiUrl);

        public string UpdateApiUrl => ApiRouteUpdate.GetUrl(PageUtility.InnerApiUrl);

        public string ClearCacheApiUrl => ApiRouteClearCache.GetUrl(PageUtility.InnerApiUrl);

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            VerifyAdministratorPermissions(ConfigManager.Permissions.Plugins.Add, ConfigManager.Permissions.Plugins.Management);
        }
    }
}
