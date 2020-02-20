using System.Collections.Generic;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto;
using SiteServer.Abstractions.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    public partial class PagesSettingsCreateRuleController
    {
        public class GetResult
        {
            public Cascade<int> Channel { get; set; }
        }

        public class ChannelResult
        {
            public Channel Channel { get; set; }
            public IEnumerable<Select<string>> LinkTypes { get; set; }
            public string FilePath { get; set; }
            public string ChannelFilePathRule { get; set; }
            public string ContentFilePathRule { get; set; }
        }

        public class SubmitRequest : ChannelRequest
        {
            public string LinkUrl { get; set; }
            public LinkType LinkType { get; set; }
            public string FilePath { get; set; }
            public string ChannelFilePathRule { get; set; }
            public string ContentFilePathRule { get; set; }
        }
    }
}