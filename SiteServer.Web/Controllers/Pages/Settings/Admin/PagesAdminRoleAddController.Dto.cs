namespace SiteServer.API.Controllers.Pages.Settings.Admin
{
    public partial class PagesAdminRoleAddController
    {
        public class Permission
        {
            public string Name { get; set; }

            public string Text { get; set; }

            public bool Selected { get; set; }
        }
    }
    
}