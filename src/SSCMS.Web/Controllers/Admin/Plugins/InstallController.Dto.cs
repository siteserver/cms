using System.Collections.Generic;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class InstallController
    {
        public class GetResult
        {
            public bool IsNightly { get; set; }
            public string Version { get; set; }
            public List<string> DownloadPlugins { get; set; }
        }

        public class DownloadRequest
        {
            public string PackageId { get; set; }
            public string Version { get; set; }
        }

        public class UploadRequest
        {
            public string PackageId { get; set; }
            public string Version { get; set; }
            public string PackageType { get; set; }
        }
    }
}
