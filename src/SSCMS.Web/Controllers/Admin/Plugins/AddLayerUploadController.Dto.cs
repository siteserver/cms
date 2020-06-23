using SSCMS.Plugins;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class AddLayerUploadController
    {
        public class UploadResult
        {
            public IPlugin OldPlugin { set; get; }
            public IPlugin NewPlugin { set; get; }
            public string FileName { set; get; }
        }

        public class OverrideRequest
        {
            public string PluginId { set; get; }
            public string FileName { set; get; }
        }
    }
}
