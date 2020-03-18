using System.Collections.Generic;
using SSCMS.Abstractions;
using SSCMS.Abstractions.Dto;
using SSCMS.Abstractions.Dto.Request;

namespace SSCMS.Controllers.Admin.Cms.Settings
{
    public partial class SettingsCreateRuleLayerSetController
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