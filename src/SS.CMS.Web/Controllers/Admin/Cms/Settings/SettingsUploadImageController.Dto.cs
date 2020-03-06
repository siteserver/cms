using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsUploadImageController
    {
        public class SubmitRequest : SiteRequest
        {
            public string ImageUploadDirectoryName { get; set; }
            public DateFormatType ImageUploadDateFormatString { get; set; }
            public bool IsImageUploadChangeFileName { get; set; }
            public string ImageUploadTypeCollection { get; set; }
            public int ImageUploadTypeMaxSize { get; set; }
            public int PhotoSmallWidth { get; set; }
            public int PhotoMiddleWidth { get; set; }
        }
    }
}