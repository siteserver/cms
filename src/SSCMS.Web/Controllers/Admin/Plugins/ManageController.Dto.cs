using System.Collections.Generic;
using SSCMS.Plugins;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class ManageController
    {
        public class GetResult
        {
            public bool IsNightly { get; set; }
            public string Version { get; set; }
            public IEnumerable<IPlugin> AllPlugins { get; set; }
        }

        public class DisableRequest
        {
            public string PluginId { get; set; }
            public bool Disabled { get; set; }
        }

        public class DeleteRequest
        {
            public string PluginId { get; set; }
        }
    }
}
