using SSCMS.Models;

namespace SSCMS.Web.Controllers.Home
{
    public partial class LoginController
    {
        public class GetResult
        {
            public string HomeTitle { get; set; }
        }

        public class CheckRequest
        {
            public string Token { get; set; }
            public string Value { get; set; }
        }

        public class LoginRequest
        {
            public string Account { get; set; }
            public string Password { get; set; }
            public bool IsPersistent { get; set; }
        }

        public class LoginResult
        {
            public User User { get; set; }
            public string Token { get; set; }
        }
    }
}
