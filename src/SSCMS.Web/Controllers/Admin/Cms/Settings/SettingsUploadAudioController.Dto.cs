using SSCMS.Dto;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsUploadAudioController
    {
        public class SubmitRequest : SiteRequest
        {
            public string AudioUploadDirectoryName { get; set; }
            public DateFormatType AudioUploadDateFormatString { get; set; }
            public bool IsAudioUploadChangeFileName { get; set; }
            public string AudioUploadExtensions { get; set; }
            public int AudioUploadTypeMaxSize { get; set; }
        }
    }
}