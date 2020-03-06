using System.Collections.Generic;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsCrossSiteTransChannelsController
    {
        public class GetResult
        {
            public Cascade<int> Channels { get; set; }
            public IEnumerable<Select<string>> TransTypes { get; set; }
            public IEnumerable<Select<string>> TransDoneTypes { get; set; }
        }

        public class GetOptionsRequest : ChannelRequest
        {
            public TransType TransType { get; set; }
            public int TransSiteId { get; set; }
        }

        public class GetOptionsResult
        {
            public string ChannelName { get; set; }
            public TransType TransType { get; set; }
            public bool IsTransSiteId { get; set; }
            public int TransSiteId { get; set; }
            public bool IsTransChannelIds { get; set; }
            public List<int> TransChannelIds { get; set; }
            public bool IsTransChannelNames { get; set; }
            public string TransChannelNames { get; set; }
            public bool IsTransIsAutomatic { get; set; }
            public bool TransIsAutomatic { get; set; }
            public TranslateContentType TransDoneType { get; set; }
            public List<Select<int>> TransSites { get; set; }
            public Cascade<int> TransChannels { get; set; }
        }

        public class SubmitRequest : ChannelRequest
        {
            public TransType TransType { get; set; }
            public int TransSiteId { get; set; }
            public List<int> TransChannelIds { get; set; }
            public string TransChannelNames { get; set; }
            public bool TransIsAutomatic { get; set; }
            public TranslateContentType TransDoneType { get; set; }
        }
    }
}