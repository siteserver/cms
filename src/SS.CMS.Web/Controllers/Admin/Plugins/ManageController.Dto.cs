using System.Collections.Generic;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Plugins
{
    public partial class ManageController
    {
        public class GetResult
        {
            public bool IsNightly { get; set; }
            public string PluginVersion { get; set; }
            public List<IPluginInstance> AllPackages { get; set; }
            public string PackageIds { get; set; }
        }
    }
}
