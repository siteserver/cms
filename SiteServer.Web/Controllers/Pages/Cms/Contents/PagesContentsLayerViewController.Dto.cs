using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    public partial class PagesContentsLayerViewController
    {
        public class GetRequest: ChannelRequest
        {
            public int ContentId { get; set; }
        }

        public class GetResult
        {
            public Content Content { get; set; }
            public string ChannelName { get; set; }
            public string State { get; set; }
            public List<ContentColumn> Columns { get; set; }
            public string SiteUrl { get; set; }
            public IEnumerable<string> GroupNames { get; set; }
            public IEnumerable<string> TagNames { get; set; }
            public List<ContentColumn> EditorColumns { get; set; }
        }
    }
}