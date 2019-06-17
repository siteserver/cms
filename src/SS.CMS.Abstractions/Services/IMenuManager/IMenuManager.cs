using System.Collections;
using System.Collections.Generic;

namespace SS.CMS.Abstractions.Services
{
    public interface IMenuManager
    {
        // IList<Menu> GetTabs(string filePath);

        IList<Menu> GetTopMenuTabs();

        bool IsValid(Menu menu, IList<string> permissionList);

        IList<Menu> GetTabList(string topId, int siteId);
    }
}