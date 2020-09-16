using SSCMS.Dto;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsUploadFileController
    {
        public class SubmitRequest : SiteRequest
        {
            public string FileUploadDirectoryName { get; set; }
            public DateFormatType FileUploadDateFormatString { get; set; }
            public bool IsFileUploadChangeFileName { get; set; }
            public string FileUploadExtensions { get; set; }
            public int FileUploadTypeMaxSize { get; set; }
        }
    }
}