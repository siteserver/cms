using SSCMS.Dto;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsUploadVideoController
    {
        public class SubmitRequest : SiteRequest
        {
            public string VideoUploadDirectoryName { get; set; }
            public DateFormatType VideoUploadDateFormatString { get; set; }
            public bool IsVideoUploadChangeFileName { get; set; }
            public string VideoUploadExtensions { get; set; }
            public int VideoUploadTypeMaxSize { get; set; }
        }
    }
}