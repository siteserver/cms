using System;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    public partial class PagesSettingsUploadVideoController
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