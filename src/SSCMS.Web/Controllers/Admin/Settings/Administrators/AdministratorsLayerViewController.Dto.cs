using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsLayerViewController
    {
        public class GetRequest
        {
            public int UserId { get; set; }

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
