using System.Collections.Generic;
using SS.CMS.Plugins;

namespace SS.CMS.Web.Controllers.Admin.Plugins
{
    public partial class ManageController
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
