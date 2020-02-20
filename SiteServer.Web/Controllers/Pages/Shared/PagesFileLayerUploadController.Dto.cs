using SiteServer.Abstractions.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Shared
{
    public partial class PagesFileLayerUploadController
    {
        public class UploadRequest: SiteRequest
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
