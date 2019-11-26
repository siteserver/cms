namespace SiteServer.API.Controllers.Pages.Settings.User
{
    public partial class PagesUserViewController
    {
        public class GetRequest
        {
            public int UserId { get; set; }

            public string UserName { get; set; }
        }

        public class GetResult
        {
            public CMS.Model.User User { get; set; }
            public string GroupName { get; set; }
        }
    }
}
