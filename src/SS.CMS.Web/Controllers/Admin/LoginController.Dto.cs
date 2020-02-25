using System;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin
{
    public partial class LoginController
    {
        public class GetResult
        {
            public bool Success { get; set; }
            public string RedirectUrl { get; set; }
            public string ProductVersion { get; set; }
            public string AdminTitle { get; set; }
        }

        public class LoginRequest
        {
            public string Account { get; set; }
            public string Password { get; set; }
            public bool IsAutoLogin { get; set; }
        }

        public class LoginResult
        {
            public Administrator Administrator { get; set; }
            public string AccessToken { get; set; }
            public DateTime ExpiresAt { get; set; }
            public string SessionId { get; set; }
            public bool IsEnforcePasswordChange { get; set; }
        }
    }
}
