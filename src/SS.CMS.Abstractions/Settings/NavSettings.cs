using System.Collections.Generic;

namespace SS.CMS.Abstractions.Settings
{
    public class NavSettings
    {
        public PermissionsSettings Permissions { get; set; }

        public IList<Menu> Menus { get; set; }
    }
}