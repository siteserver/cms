using System.Collections.Generic;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class AddController
    {
        public class GetResult
        {
            public bool IsNightly { get; set; }
            public string Version { get; set; }
            public IEnumerable<string> PackageIds { get; set; }
        }
    }
}
