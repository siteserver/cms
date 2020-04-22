using System.Collections.Generic;
using SSCMS.Dto;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Home
{
    public partial class ContentAddController
    {
        public class GetRequest : ChannelRequest
        {
            public int ContentId { get; set; }
        }

        public class SiteResult
        {
            public int Id { get; set; }
            public string SiteName { get; set; }
            public string SiteUrl { get; set; }
        }

        public class ChannelResult
        {
            public int Id { get; set; }
            public string ChannelName { get; set; }
        }

        public class GetResult
        {
            public User User { get; set; }
            public Config Config { get; set; }
            public List<SiteResult> Sites { get; set; }
            public List<ChannelResult> Channels { get; set; }
            public SiteResult Site { get; set; }
            public ChannelResult Channel { get; set; }

            public IEnumerable<string> AllGroupNames { get; set; }
            public List<string> AllTagNames { get; set; }
            public List<TableStyle> Styles { get; set; }
            public List<KeyValuePair<int, string>> CheckedLevels { get; set; }
            public int CheckedLevel { get; set; }
            public Content Content { get; set; }
        }
    }
}
