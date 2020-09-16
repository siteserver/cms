using System.Collections.Generic;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Common.Form
{
    public partial class LayerFileUploadController
    {
        public class Options
        {
            public bool IsChangeFileName { get; set; }
            public bool IsLibrary { get; set; }
        }

        public class UploadRequest : SiteRequest
        {
            public bool IsChangeFileName { get; set; }
        }

        public class UploadResult
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }

        public class SubmitRequest : Options
        {
            public int SiteId { get; set; }
            public List<string> FilePaths { get; set; }
        }

        public class SubmitResult
        {
            public string FileUrl { get; set; }
            public string FileVirtualUrl { get; set; }
        }
    }
}
