using System;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    public partial class PagesSettingsUploadImageController
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