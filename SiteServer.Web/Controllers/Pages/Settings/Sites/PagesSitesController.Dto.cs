using System.Collections.Generic;
using SiteServer.Abstractions;

namespace SiteServer.API.Controllers.Pages.Settings.Sites
{
    public partial class PagesSitesController
    {
        public class GetResult
        {
            public List<Site> Sites { get; set; }
            public int RootSiteId { get; set; }
            public List<string> TableNames { get; set; }
        }

        public class ActionResult
        {
            public List<Site> Sites { get; set; }
        }
    }
}