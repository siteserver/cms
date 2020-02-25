using System.Collections.Generic;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Configs
{
    public partial class ConfigsHomeMenuController
    {
        public class GetResult
        {
            public List<UserMenu> UserMenus { get; set; }
            public List<UserGroup> Groups { get; set; }
        }

        public class UserMenusResult
        {
            public List<UserMenu> UserMenus { get; set; }
        }

        public class SubmitRequest
        {
            public int Id { get; set; }
            public bool IsGroup { get; set; }
            public List<int> GroupIds { get; set; }
            public bool Disabled { get; set; }
            public int ParentId { get; set; }
            public int Taxis { get; set; }
            public string Text { get; set; }
            public string IconClass { get; set; }
            public string Href { get; set; }
            public string Target { get; set; }
        }
    }
}
