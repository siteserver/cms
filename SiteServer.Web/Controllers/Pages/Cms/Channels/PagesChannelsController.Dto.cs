using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.API.Controllers.Pages.Cms.Channels
{
    public partial class PagesChannelsController
    {
        public class ChannelIdsRequest : SiteRequest
        {
            public List<int> ChannelIds { get; set; }
        }

        public class AppendRequest : SiteRequest
        {
            public bool IsIndexName { get; set; }
            public string Channels { get; set; }
        }
    }
}
