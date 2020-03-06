using System.Collections.Generic;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesLayerSelectController
    {
        public class GetResult
        {
            public List<Site> Sites { get; set; }
            public int RootSiteId { get; set; }
            public List<string> TableNames { get; set; }
        }
    }
}