using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Cms.Editor
{
    public partial class EditorController
    {
        public class GetRequest : ChannelRequest
        {
            public int ContentId { get; set; }
        }

        public class GetResult
        {
            public Content Content { get; set; }
            public Site Site { get; set; }
            public Channel Channel { get; set; }
            public IEnumerable<string> GroupNames { get; set; }
            public IEnumerable<string> TagNames { get; set; }
            public IEnumerable<TableStyle> Styles { get; set; }
            public List<Select<int>> CheckedLevels { get; set; }
        }

        public class PreviewRequest
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public int ContentId { get; set; }
            public Content Content { get; set; }
        }

        public class PreviewResult
        {
            public string Url { get; set; }
        }

        public class Translation
        {
            public int TransSiteId { get; set; }
            public int TransChannelId { get; set; }
            public TranslateContentType TransType { get; set; }
        }

        public class SaveRequest
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public int ContentId { get; set; }
            public Content Content { get; set; }
            public List<Translation> Translations { get; set; }
        }
    }
}
