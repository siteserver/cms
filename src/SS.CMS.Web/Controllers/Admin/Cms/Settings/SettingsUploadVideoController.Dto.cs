using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsUploadVideoController
    {
        public class SubmitRequest : SiteRequest
        {
            public string VideoUploadDirectoryName { get; set; }
            public DateFormatType VideoUploadDateFormatString { get; set; }
            public bool IsVideoUploadChangeFileName { get; set; }
            public string VideoUploadTypeCollection { get; set; }
            public int VideoUploadTypeMaxSize { get; set; }
        }
    }
}