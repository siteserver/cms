using System.Collections.Generic;
using SSCMS.Dto;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Cms.Editor
{
    public partial class EditorLayerTranslateController
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
            public List<int> TransChannelIds { get; set; }
            public TranslateContentType TransType { get; set; }
        }

        public class TransChannel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class SubmitResult
        {
            public List<TransChannel> Channels { get; set; }
        }
    }
}