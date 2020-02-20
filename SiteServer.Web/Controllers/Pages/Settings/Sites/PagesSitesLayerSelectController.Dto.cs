using System.Collections.Generic;
using SiteServer.Abstractions;

namespace SiteServer.API.Controllers.Pages.Settings.Sites
{
    public partial class PagesSitesLayerSelectController
    {
        public class GetResult
        {
            public List<Site> Sites { get; set; }
            public int RootSiteId { get; set; }
            public List<string> TableNames { get; set; }
        }
    }
}