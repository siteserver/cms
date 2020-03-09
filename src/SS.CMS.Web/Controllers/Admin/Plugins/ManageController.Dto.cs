using System.Collections.Generic;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Plugins;

namespace SS.CMS.Web.Controllers.Admin.Plugins
{
    public partial class ManageController
    {
        public class GetResult
        {
            public bool IsNightly { get; set; }
            public string PluginVersion { get; set; }
            public IEnumerable<IPackageMetadata> EnabledPackages { get; set; }
            public string PackageIds { get; set; }
        }
    }
}
