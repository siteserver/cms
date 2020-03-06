using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsPasswordController
    {
        public class GetRequest
        {
            public int UserId { get; set; }
        }

        public class GetResult
        {
            public Administrator Administrator { get; set; }
        }

        public class SubmitRequest
        {
            public int UserId { get; set; }
            public string Password { get; set; }
        }
    }
}
