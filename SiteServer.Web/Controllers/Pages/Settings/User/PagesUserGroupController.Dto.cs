using System.Collections.Generic;
using SiteServer.Abstractions;

namespace SiteServer.API.Controllers.Pages.Settings.User
{
    public partial class PagesUserGroupController
    {
        public class GetResult
        {
            public IEnumerable<UserGroup> Groups { get; set; }
            public IEnumerable<string> AdminNames { get; set; }
        }
    }
}