using System.Collections.Generic;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsCreateRuleController
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