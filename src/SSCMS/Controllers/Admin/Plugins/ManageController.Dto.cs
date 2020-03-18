using System.Collections.Generic;
using SSCMS.Abstractions;

namespace SSCMS.Controllers.Admin.Plugins
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
