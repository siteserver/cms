using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersLayerPasswordController
    {
        public class GetRequest
        {
            public int UserId { get; set; }
        }

        public class GetResult
        {
            public User User { get; set; }
        }

        public class SubmitRequest
        {
            public int UserId { get; set; }
            public string Password { get; set; }
        }
    }
}
