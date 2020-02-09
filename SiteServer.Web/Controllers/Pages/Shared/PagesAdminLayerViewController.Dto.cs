using SiteServer.Abstractions;

namespace SiteServer.API.Controllers.Pages.Shared
{
    public partial class PagesAdminLayerViewController
    {
        public class GetRequest
        {
            public int AdminId { get; set; }
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
