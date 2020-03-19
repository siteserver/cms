using System;
using SSCMS.Dto.Request;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsCreateController
    {
        public class SubmitRequest : SiteRequest
        {
            public bool IsCreateContentIfContentChanged { get; set; }
            public bool IsCreateChannelIfChannelChanged { get; set; }
            public bool IsCreateShowPageInfo { get; set; }
            public bool IsCreateIe8Compatible { get; set; }
            public bool IsCreateBrowserNoCache { get; set; }
            public bool IsCreateJsIgnoreError { get; set; }
            public bool IsCreateWithJQuery { get; set; }
            public bool IsCreateDoubleClick { get; set; }
            public int CreateStaticMaxPage { get; set; }
            public bool IsCreateUseDefaultFileName { get; set; }
            public string CreateDefaultFileName { get; set; }
            public bool IsCreateStaticContentByAddDate { get; set; }
            public DateTime CreateStaticContentAddDate { get; set; }
        }
    }
}