using SSCMS.Plugins;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class ViewController
    {
        public class GetResult
        {
            public bool IsNightly { get; set; }
            public string Version { get; set; }
            public IPlugin LocalPlugin { get; set; }
            public string Content { get; set; }
            public string ChangeLog { get; set; }
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
