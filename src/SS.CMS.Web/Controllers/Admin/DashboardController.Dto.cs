namespace SS.CMS.Web.Controllers.Admin
{
    public partial class DashboardController
    {
        public class GetResult
        {
            public string Version { get; set; }
            public string LastActivityDate { get; set; }
            public string UpdateDate { get; set; }
            public string AdminWelcomeHtml { get; set; }
        }

        public class Checking
        {
            public string SiteName { get; set; }
            public int Count { get; set; }
        }
    }
}