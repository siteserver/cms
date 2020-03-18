using SSCMS.Abstractions;
using SSCMS.Abstractions.Dto.Request;

namespace SSCMS.Controllers.Admin.Cms.Settings
{
    public partial class SettingsUploadFileController
    {
        public class SubmitRequest : SiteRequest
        {
            public string FileUploadDirectoryName { get; set; }
            public DateFormatType FileUploadDateFormatString { get; set; }
            public bool IsFileUploadChangeFileName { get; set; }
            public string FileUploadTypeCollection { get; set; }
            public int FileUploadTypeMaxSize { get; set; }
        }
    }
}