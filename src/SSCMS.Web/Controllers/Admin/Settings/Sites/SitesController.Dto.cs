using System.Collections.Generic;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesController
    {
        public class GetResult
        {
            public IEnumerable<SiteType> SiteTypes { get; set; }
            public List<Site> Sites { get; set; }
            public int RootSiteId { get; set; }
            public List<string> TableNames { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public string SiteDir { get; set; }
            public bool DeleteFiles { get; set; }
        }

        public class EditRequest : SiteRequest
        {
            public string SiteDir { get; set; }
            public string SiteName { get; set; }
            public int ParentId { get; set; }
            public int Taxis { get; set; }
            public TableRule TableRule { get; set; }
            public string TableChoose { get; set; }
            public string TableHandWrite { get; set; }
        }

        public class SitesResult
        {
            public List<Site> Sites { get; set; }
        }
    }
}