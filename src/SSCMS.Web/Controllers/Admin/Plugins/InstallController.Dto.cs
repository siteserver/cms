namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class InstallController
    {
        public class GetResult
        {
            public bool IsNightly { get; set; }
            public string Version { get; set; }
        }

        public class DownloadRequest
        {
            public string PluginId { get; set; }
            public string Version { get; set; }
        }

        public class UploadRequest
        {
            public string PluginId { get; set; }
            public string Version { get; set; }
        }
    }
}
