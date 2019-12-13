using System.Collections.Generic;
using SiteServer.Abstractions;
using SiteServer.CMS.Dto;

namespace SiteServer.API.Controllers.Pages.Cms.Editor
{
    public partial class PagesEditorLayerWordController
    {
        public class ConfigRequest
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public int ContentId { get; set; }
        }

        public class UploadResult
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }
    }
}
