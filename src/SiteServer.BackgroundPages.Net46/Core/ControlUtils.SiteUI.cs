using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Core
{
    public static partial class ControlUtils
    {
        public static class SiteUI
        {
            public static void AddListItems(ListControl listControl)
            {
                var siteIdList = SiteManager.GetSiteIdList();
                var mySystemInfoList = new List<SiteInfo>();
                var parentWithChildren = new Hashtable();
                SiteInfo hqSiteInfo = null;
                foreach (var siteId in siteIdList)
                {
                    var siteInfo = SiteManager.GetSiteInfo(siteId);
                    if (siteInfo.Root)
                    {
                        hqSiteInfo = siteInfo;
                    }
                    else
                    {
                        if (siteInfo.ParentId == 0)
                        {
                            mySystemInfoList.Add(siteInfo);
                        }
                        else
                        {
                            var children = new List<SiteInfo>();
                            if (parentWithChildren.Contains(siteInfo.ParentId))
                            {
                                children = (List<SiteInfo>)parentWithChildren[siteInfo.ParentId];
                            }
                            children.Add(siteInfo);
                            parentWithChildren[siteInfo.ParentId] = children;
                        }
                    }
                }
                if (hqSiteInfo != null)
                {
                    AddListItem(listControl, hqSiteInfo, parentWithChildren, 0);
                }
                foreach (var siteInfo in mySystemInfoList)
                {
                    AddListItem(listControl, siteInfo, parentWithChildren, 0);
                }
            }

            private static void AddListItem(ListControl listControl, SiteInfo siteInfo, Hashtable parentWithChildren, int level)
            {
                var padding = string.Empty;
                for (var i = 0; i < level; i++)
                {
                    padding += "　";
                }
                if (level > 0)
                {
                    padding += "└ ";
                }

                if (parentWithChildren[siteInfo.Id] != null)
                {
                    var children = (List<SiteInfo>)parentWithChildren[siteInfo.Id];
                    listControl.Items.Add(new ListItem(padding + siteInfo.SiteName + $"({children.Count})", siteInfo.Id.ToString()));
                    level++;
                    foreach (SiteInfo subSiteInfo in children)
                    {
                        AddListItem(listControl, subSiteInfo, parentWithChildren, level);
                    }
                }
                else
                {
                    listControl.Items.Add(new ListItem(padding + siteInfo.SiteName, siteInfo.Id.ToString()));
                }
            }
        }
    }
}
