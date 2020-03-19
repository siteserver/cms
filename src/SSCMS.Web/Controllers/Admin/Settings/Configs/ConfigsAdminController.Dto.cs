namespace SSCMS.Web.Controllers.Admin.Settings.Configs
{
    public partial class ConfigsAdminController
    {
        public class GetResult
        {
            public string AdminTitle { get; set; }
            public string AdminLogoUrl { get; set; }
            public string AdminWelcomeHtml { get; set; }
        }

        public class SubmitRequest
        {
            public string AdminTitle { get; set; }
            public string AdminLogoUrl { get; set; }
            public string AdminWelcomeHtml { get; set; }
        }
    }
}
