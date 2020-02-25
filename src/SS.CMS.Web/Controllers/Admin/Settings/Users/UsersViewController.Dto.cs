using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersViewController
    {
        public class GetRequest
        {
            public int UserId { get; set; }

            public string UserName { get; set; }
        }

        public class GetResult
        {
            public User User { get; set; }
            public string GroupName { get; set; }
        }
    }
}
