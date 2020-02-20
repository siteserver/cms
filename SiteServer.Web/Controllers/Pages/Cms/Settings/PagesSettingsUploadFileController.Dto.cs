using System;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    public partial class PagesSettingsUploadFileController
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