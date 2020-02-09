using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SiteServer.CMS.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    public partial class PagesSettingsContentController
    {
        public class SubmitRequest : SiteRequest
        {
            public bool IsSaveImageInTextEditor { get; set; }
            public bool IsAutoPageInTextEditor { get; set; }
            public int AutoPageWordNum { get; set; }
            public bool IsContentTitleBreakLine { get; set; }
            public bool IsContentSubTitleBreakLine { get; set; }
            public bool IsAutoCheckKeywords { get; set; }
            public int CheckContentLevel { get; set; }
            public int CheckContentDefaultLevel { get; set; }
        }
    }
}