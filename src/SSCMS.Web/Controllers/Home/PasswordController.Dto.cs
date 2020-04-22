using SSCMS.Models;

namespace SSCMS.Web.Controllers.Home
{
    public partial class PasswordController
    {
        public class GetResult
        {
            public User User { get; set; }
        }

        public class SubmitRequest
        {
            public string Password { get; set; }
        }
    }
}
