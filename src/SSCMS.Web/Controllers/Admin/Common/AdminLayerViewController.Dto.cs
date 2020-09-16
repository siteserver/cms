using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Common
{
    public partial class AdminLayerViewController
    {
        public class GetRequest
        {
            public int AdminId { get; set; }
            public string UserName { get; set; }
        }

        public class GetResult
        {
            public Administrator Administrator { get; set; }
            public string Level { get; set; }
            public bool IsSuperAdmin { get; set; }
            public string SiteNames { get; set; }
            public bool IsOrdinaryAdmin { get; set; }
            public string RoleNames { get; set; }
        }
    }
}
