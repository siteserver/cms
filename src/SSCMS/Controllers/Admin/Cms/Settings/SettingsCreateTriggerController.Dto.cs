using System.Collections.Generic;
using SSCMS.Abstractions.Dto;
using SSCMS.Abstractions.Dto.Request;

namespace SSCMS.Controllers.Admin.Cms.Settings
{
    public partial class SettingsCreateTriggerController
    {
        public class GetResult
        {
            public Cascade<int> Channel { get; set; }
        }

        public class SubmitRequest : ChannelRequest
        {
            public bool IsCreateChannelIfContentChanged { get; set; }
            public List<int> CreateChannelIdsIfContentChanged { get; set; }
        }
    }
}