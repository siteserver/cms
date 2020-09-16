using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsAccessTokensLayerViewController
    {
        public class GetResult
        {
            public AccessToken Token { get; set; }
            public string AccessToken { get; set; }
        }

        public class RegenerateResult
        {
            public string AccessToken { get; set; }
        }
    }
}
