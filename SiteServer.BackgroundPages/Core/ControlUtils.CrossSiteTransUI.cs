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
        public static class CrossSiteTransUI
        {
            public static void LoadSiteIdDropDownList(DropDownList siteIdDropDownList, SiteInfo siteInfo, int channelId)
            {
                siteIdDropDownList.Items.Clear();

                var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                var transType = ECrossSiteTransTypeUtils.GetEnumType(channelInfo.TransType);

                if (transType == ECrossSiteTransType.SelfSite || transType == ECrossSiteTransType.SpecifiedSite || transType == ECrossSiteTransType.ParentSite)
                {
                    int theSiteId;
                    if (transType == ECrossSiteTransType.SelfSite)
                    {
                        theSiteId = siteInfo.Id;
                    }
                    else if (transType == ECrossSiteTransType.SpecifiedSite)
                    {
                        theSiteId = channelInfo.TransSiteId;
                    }
                    else
                    {
                        theSiteId = SiteManager.GetParentSiteId(siteInfo.Id);
                    }
                    if (theSiteId > 0)
                    {
                        var theSiteInfo = SiteManager.GetSiteInfo(theSiteId);
                        if (theSiteInfo != null)
                        {
                            var listitem = new ListItem(theSiteInfo.SiteName, theSiteInfo.Id.ToString());
                            siteIdDropDownList.Items.Add(listitem);
                        }
                    }
                }
                else if (transType == ECrossSiteTransType.AllParentSite)
                {
                    var siteIdList = SiteManager.GetSiteIdList();

                    var allParentSiteIdList = new List<int>();
                    SiteManager.GetAllParentSiteIdList(allParentSiteIdList, siteIdList, siteInfo.Id);

                    foreach (var psId in siteIdList)
                    {
                        if (psId == siteInfo.Id) continue;
                        var psInfo = SiteManager.GetSiteInfo(psId);
                        var show = psInfo.Root || allParentSiteIdList.Contains(psInfo.Id);
                        if (show)
                        {
                            var listitem = new ListItem(psInfo.SiteName, psId.ToString());
                            if (psInfo.Root) listitem.Selected = true;
                            siteIdDropDownList.Items.Add(listitem);
                        }
                    }
                }
                else if (transType == ECrossSiteTransType.AllSite)
                {
                    var siteIdList = SiteManager.GetSiteIdList();

                    foreach (var psId in siteIdList)
                    {
                        var psInfo = SiteManager.GetSiteInfo(psId);
                        var listitem = new ListItem(psInfo.SiteName, psId.ToString());
                        if (psInfo.Root) listitem.Selected = true;
                        siteIdDropDownList.Items.Add(listitem);
                    }
                }
            }

            public static void LoadChannelIdListBox(ListBox channelIdListBox, SiteInfo siteInfo, int psId, ChannelInfo channelInfo, PermissionsImpl permissionsImpl)
            {
                channelIdListBox.Items.Clear();

                var transType = ECrossSiteTransTypeUtils.GetEnumType(channelInfo.TransType);
                var isUseNodeNames = transType == ECrossSiteTransType.AllParentSite || transType == ECrossSiteTransType.AllSite;

                if (!isUseNodeNames)
                {
                    var channelIdList = TranslateUtils.StringCollectionToIntList(channelInfo.TransChannelIds);
                    foreach (var theChannelId in channelIdList)
                    {
                        var theNodeInfo = ChannelManager.GetChannelInfo(psId, theChannelId);
                        if (theNodeInfo != null)
                        {
                            var listitem = new ListItem(theNodeInfo.ChannelName, theNodeInfo.Id.ToString());
                            channelIdListBox.Items.Add(listitem);
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(channelInfo.TransChannelNames))
                    {
                        var nodeNameArrayList = TranslateUtils.StringCollectionToStringList(channelInfo.TransChannelNames);
                        var channelIdList = ChannelManager.GetChannelIdList(psId);
                        foreach (var nodeName in nodeNameArrayList)
                        {
                            foreach (var theChannelId in channelIdList)
                            {
                                var theNodeInfo = ChannelManager.GetChannelInfo(psId, theChannelId);
                                if (theNodeInfo.ChannelName == nodeName)
                                {
                                    var listitem = new ListItem(theNodeInfo.ChannelName, theNodeInfo.Id.ToString());
                                    channelIdListBox.Items.Add(listitem);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        ControlUtils.ChannelUI.AddListItemsForAddContent(channelIdListBox.Items, SiteManager.GetSiteInfo(psId), false, permissionsImpl);
                    }
                }
            }

            public static ListItem GetListItem(ECrossSiteTransType type, bool selected)
            {
                var item = new ListItem(ECrossSiteTransTypeUtils.GetText(type), ECrossSiteTransTypeUtils.GetValue(type));
                if (selected)
                {
                    item.Selected = true;
                }
                return item;
            }

            public static void AddListItems(ListControl listControl)
            {
                if (listControl == null) return;

                listControl.Items.Add(GetListItem(ECrossSiteTransType.None, false));
                listControl.Items.Add(GetListItem(ECrossSiteTransType.SelfSite, false));
                listControl.Items.Add(GetListItem(ECrossSiteTransType.ParentSite, false));
                listControl.Items.Add(GetListItem(ECrossSiteTransType.AllParentSite, false));
                listControl.Items.Add(GetListItem(ECrossSiteTransType.AllSite, false));
            }

            public static void AddAllListItems(ListControl listControl, bool isParentSite)
            {
                if (listControl == null) return;

                listControl.Items.Add(GetListItem(ECrossSiteTransType.None, false));
                listControl.Items.Add(GetListItem(ECrossSiteTransType.SelfSite, false));
                listControl.Items.Add(GetListItem(ECrossSiteTransType.SpecifiedSite, false));
                if (isParentSite)
                {
                    listControl.Items.Add(GetListItem(ECrossSiteTransType.ParentSite, false));
                    listControl.Items.Add(GetListItem(ECrossSiteTransType.AllParentSite, false));
                }
                listControl.Items.Add(GetListItem(ECrossSiteTransType.AllSite, false));
            }
        }
    }
}
