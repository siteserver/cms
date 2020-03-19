using SSCMS.Dto.Request;

namespace SSCMS.Web.Controllers.Admin.Shared
{
    public partial class FileLayerUploadController
    {
        public class UploadRequest : SiteRequest
        {
            public bool IsChangeFileName { get; set; }
        }

        public class UploadResult
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string Url { get; set; }
        }
    }
}
