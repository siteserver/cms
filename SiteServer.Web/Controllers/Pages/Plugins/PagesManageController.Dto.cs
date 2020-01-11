using System.Collections.Generic;
using SiteServer.CMS.Plugin;

namespace SiteServer.API.Controllers.Pages.Plugins
{
    public partial class PagesManageController
    {
        public class GetResult
        {
            public bool IsNightly { get; set; }
            public string PluginVersion { get; set; }
            public List<PluginInstance> AllPackages { get; set; }
            public string PackageIds { get; set; }
        }
    }
}
