using System.Collections;
using BaiRong.Core.Tabs;

namespace BaiRong.Core
{
	public class ProductFileUtils
	{
        private ProductFileUtils()
	    {
            
	    }

        public static ArrayList GetMenuTopArrayList()
        {
            var tabArrayList = new ArrayList();

            var menuPath = PathUtils.GetMenusPath("Top.config");

            if (FileUtils.IsFileExists(menuPath))
            {
                try
                {
                    var tabs = TabCollection.GetTabs(menuPath);
                    foreach (var parent in tabs.Tabs)
                    {
                        parent.IsPlatform = true;
                        tabArrayList.Add(parent);
                    }
                }
                catch { }
            }

            return tabArrayList;
        }

        public static ArrayList GetMenuGroupTabArrayListOfApp(string appID, string menuID)
        {
            var arraylist = new ArrayList();

            var menuPath = PathUtils.GetMenusPath(appID, menuID + ".config");
            if (FileUtils.IsFileExists(menuPath))
            {
                try
                {
                    var tabs = TabCollection.GetTabs(menuPath);
                    foreach (var parent in tabs.Tabs)
                    {
                        arraylist.Add(parent);
                    }
                }
                catch { }
            }

            return arraylist;
        }
	}
}
