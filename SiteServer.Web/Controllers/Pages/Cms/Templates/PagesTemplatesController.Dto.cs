using System.Collections.Generic;
using SiteServer.API.Result;
using SiteServer.CMS.Dto;
using SiteServer.CMS.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Templates
{
    public partial class PagesTemplatesController
    {
        public class SearchRequest : PageRequest
        {
            public int SiteId { get; set; }
            public string LogType { get; set; }
            public string UserName { get; set; }
            public string Keyword { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }

        public class SiteLogResult : Abstractions.SiteLog
        {
            public string WebUrl { get; set; }

            public string SiteName { get; set;  }
        }

        public class SiteLogPageResult : PageResult<SiteLogResult>
        {
            public int IndexPageTemplateCount { get; set; }
            public int ChannelTemplateCount { get; set; }
            public int ContentTemplateCount { get; set; }
            public int FileTemplateCount { get; set; }
            public IEnumerable<Select<int>> SiteOptions { get; set; }
        }
    }
}
