using System.Collections.Generic;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesAddController
    {
        private static void AddSite(List<KeyValuePair<int, string>> siteList, Site site, Dictionary<int, List<Site>> parentWithChildren, int level)
        {
            if (level > 1) return;
            var padding = string.Empty;
            for (var i = 0; i < level; i++)
            {
                padding += "　";
            }
            if (level > 0)
            {
                padding += "└ ";
            }

            if (parentWithChildren.ContainsKey(site.Id))
            {
                var children = parentWithChildren[site.Id];
                siteList.Add(new KeyValuePair<int, string>(site.Id, padding + site.SiteName + $"({children.Count})"));
                level++;
                foreach (var subSite in children)
                {
                    AddSite(siteList, subSite, parentWithChildren, level);
                }
            }
            else
            {
                siteList.Add(new KeyValuePair<int, string>(site.Id, padding + site.SiteName));
            }
        }
    }
}