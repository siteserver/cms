using System.Collections.Generic;
using SiteServer.Abstractions;
using SiteServer.CMS.Dto;
using SiteServer.CMS.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Editor
{
    public partial class PagesEditorLayerTranslateController
    {
        public class GetResult
        {
            public List<Select<int>> TransSites { get; set; }
        }

        public class GetOptionsRequest : ChannelRequest
        {
            public int TransSiteId { get; set; }
        }

        public class GetOptionsResult
        {
            public Cascade<int> TransChannels { get; set; }
        }

        public class SubmitRequest : ChannelRequest
        {
            public int TransSiteId { get; set; }
            public int TransChannelId { get; set; }
            public TranslateContentType TransType { get; set; }
        }
    }
}