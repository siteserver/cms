using System.Collections.Generic;
using SSCMS.Abstractions;
using SSCMS.Abstractions.Dto;
using SSCMS.Abstractions.Dto.Request;

namespace SSCMS.Controllers.Admin.Cms.Editor
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
            public int TransChannelId { get; set; }
            public TranslateContentType TransType { get; set; }
        }
    }
}