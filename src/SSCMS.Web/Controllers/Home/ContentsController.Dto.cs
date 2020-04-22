using System.Collections.Generic;
using SSCMS.Dto;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Home
{
    public partial class ContentsController
    {
        public class ListRequest : ChannelRequest
        {
            public int Page { get; set; }
        }

        public class Permissions
        {
            public bool IsAdd { get; set; }
            public bool IsDelete { get; set; }
            public bool IsEdit { get; set; }
            public bool IsTranslate { get; set; }
            public bool IsCheck { get; set; }
            public bool IsCreate { get; set; }
            public bool IsChannelEdit { get; set; }
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
        }

        public class ListResult
        {
            public List<Content> Contents { get; set; }
            public int Count { get; set; }
            public int Pages { get; set; }
            public Permissions Permissions { get; set; }
            public List<ContentColumn> Columns { get; set; }
        }
    }
}
