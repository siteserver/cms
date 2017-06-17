using System.Collections;
using System.Collections.Generic;

namespace BaiRong.Core.Tabs
{
    public class TabManager
	{
        public static List<Tab> GetTopMenuTabs()
        {
            var list = new List<Tab>();

            var menuPath = PathUtils.GetMenusPath("Top.config");
            if (!FileUtils.IsFileExists(menuPath)) return list;

            var tabs = TabCollection.GetTabs(menuPath);
            foreach (var parent in tabs.Tabs)
            {
                list.Add(parent);
            }

            return list;
        }

        public static bool IsValid(Tab tab, IList permissionList)
        {
            if (tab.HasPermissions)
            {
                if (permissionList != null && permissionList.Count > 0)
                {
                    var tabPermissions = tab.Permissions.Split(',');
                    foreach (var tabPermission in tabPermissions)
                    {
                        if (permissionList.Contains(tabPermission))
                            return true;
                    }
                }

                //ITab valid, but invalid role set
                return false;
            }

            //ITab valid, but no roles
            return true;
        }
	}
}
