using System.Collections.Generic;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
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