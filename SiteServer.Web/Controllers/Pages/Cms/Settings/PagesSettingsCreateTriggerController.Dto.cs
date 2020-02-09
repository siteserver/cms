using System.Collections.Generic;
using SiteServer.CMS.Dto;
using SiteServer.CMS.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    public partial class PagesSettingsCreateTriggerController
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