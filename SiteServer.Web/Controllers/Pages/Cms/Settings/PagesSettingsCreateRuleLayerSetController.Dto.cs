using System.Collections.Generic;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto;
using SiteServer.Abstractions.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    public partial class PagesSettingsCreateRuleLayerSetController
    {
        public class GetRequest: ChannelRequest
        {
            public bool IsChannel { get; set; }
        }

        public class ChannelResult
        {
            public Channel Channel { get; set; }
            public IEnumerable<Select<string>> LinkTypes { get; set; }
            public IEnumerable<Select<string>> TaxisTypes { get; set; }
        }
    }
}