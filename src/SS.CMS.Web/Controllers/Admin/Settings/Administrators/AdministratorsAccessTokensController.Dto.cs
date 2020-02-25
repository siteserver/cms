using System.Collections.Generic;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsAccessTokensController
    {
        public class ListResult
        {
            public List<AccessToken> Tokens { get; set; }
            public List<string> AdminNames { get; set; }
            public List<string> Scopes { get; set; }
            public string AdminName { get; set; }
        }

        public class TokensResult
        {
            public List<AccessToken> Tokens { get; set; }
        }
    }
}
