using System.Collections;
using BaiRong.Core;
using SiteServer.CMS.Core.Security;
using System.Collections.Generic;
using BaiRong.Core.Tabs;
using SiteServer.BackgroundPages.Wcm;
using SiteServer.CMS.Core;
using SiteServer.CMS.Wcm.GovInteract;

namespace SiteServer.BackgroundPages.Core
{
    public class NodeNaviTabManager
    {
        public static bool IsValid(Tab tab, ArrayList permissionArrayList)
        {
            if (StringUtils.StartsWithIgnoreCase(tab.Id, AppManager.Cms.LeftMenu.Function.IdInput))
            {
                return ProductPermissionsManager.Current.IsSystemAdministrator || IsValid(tab, permissionArrayList);
            }
            return IsValid(tab, permissionArrayList);
        }

        public static TabCollection GetTabCollection(Tab parent, int publishmentSystemId)
        {
            TabCollection tabCollection;
            if (StringUtils.EqualsIgnoreCase(parent.Id, AppManager.Cms.LeftMenu.Function.IdResume))
            {
                tabCollection = new TabCollection(parent.Children);
            }
            else if (StringUtils.EqualsIgnoreCase(parent.Id, AppManager.Wcm.LeftMenu.IdGovInteract))
            {
                Tab[] tabs;
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var nodeInfoList = GovInteractManager.GetNodeInfoList(publishmentSystemInfo);
                if (nodeInfoList.Count > 0 && ProductPermissionsManager.Current.GovInteractPermissionDict.ContainsKey(publishmentSystemId))
                {
                    var govInteractPermissionListOfPublishmentSystemId = ProductPermissionsManager.Current.GovInteractPermissionDict[publishmentSystemId];
                    var govInteractPermissionList = govInteractPermissionListOfPublishmentSystemId;
                    var tabList = new List<Tab>();
                    foreach (var nodeInfo in nodeInfoList)
                    {
                        if (govInteractPermissionListOfPublishmentSystemId == null || govInteractPermissionListOfPublishmentSystemId.Count == 0)
                        {
                            if (ProductPermissionsManager.Current.GovInteractPermissionDict.ContainsKey(nodeInfo.NodeId))
                            {
                                govInteractPermissionList = ProductPermissionsManager.Current.GovInteractPermissionDict[nodeInfo.NodeId];
                            }
                        }

                        if (govInteractPermissionList != null && govInteractPermissionList.Count > 0)
                        {
                            var tab = new Tab
                            {
                                Text = nodeInfo.NodeName,
                                Id = AppManager.Wcm.LeftMenu.IdGovInteract + "_" + nodeInfo.NodeId
                            };

                            var childList = new List<Tab>();

                            if (govInteractPermissionList.Contains(AppManager.Wcm.Permission.GovInteract.GovInteractAccept))
                            {
                                childList.Add(new Tab
                                {
                                    Text = "待受理办件",
                                    Href = PageGovInteractListAccept.GetRedirectUrl(nodeInfo.PublishmentSystemId, nodeInfo.NodeId),
                                    KeepQueryString = false,
                                    Target = "right"
                                });
                            }
                            if (govInteractPermissionList.Contains(AppManager.Wcm.Permission.GovInteract.GovInteractReply))
                            {
                                childList.Add(new Tab
                                {
                                    Text = "待办理办件",
                                    Href = PageGovInteractListReply.GetRedirectUrl(nodeInfo.PublishmentSystemId, nodeInfo.NodeId),
                                    KeepQueryString = false,
                                    Target = "right"
                                });
                            }
                            if (govInteractPermissionList.Contains(AppManager.Wcm.Permission.GovInteract.GovInteractCheck))
                            {
                                childList.Add(new Tab
                                {
                                    Text = "待审核办件",
                                    Href = PageGovInteractListCheck.GetRedirectUrl(nodeInfo.PublishmentSystemId, nodeInfo.NodeId),
                                    KeepQueryString = false,
                                    Target = "right"
                                });
                            }
                            if (govInteractPermissionList.Contains(AppManager.Wcm.Permission.GovInteract.GovInteractView) || govInteractPermissionList.Contains(AppManager.Wcm.Permission.GovInteract.GovInteractDelete))
                            {
                                childList.Add(new Tab
                                {
                                    Text = "所有办件",
                                    Href = PageGovInteractListAll.GetRedirectUrl(nodeInfo.PublishmentSystemId, nodeInfo.NodeId),
                                    KeepQueryString = false,
                                    Target = "right"
                                });
                            }
                            if (govInteractPermissionList.Contains(AppManager.Wcm.Permission.GovInteract.GovInteractAdd))
                            {
                                var redirectUrl = PageGovInteractListAccept.GetRedirectUrl(nodeInfo.PublishmentSystemId, nodeInfo.NodeId);
                                var child = new Tab
                                {
                                    Text = "新增办件",
                                    Href = WebUtils.GetContentAddAddUrl(publishmentSystemId, nodeInfo, redirectUrl),
                                    KeepQueryString = false,
                                    Target = "right"
                                };
                                childList.Add(child);
                            }

                            tab.Children = new Tab[childList.Count];
                            for (var i = 0; i < childList.Count; i++)
                            {
                                tab.Children[i] = childList[i];
                            }

                            tabList.Add(tab);
                        }
                    }

                    var k = 0;
                    tabs = new Tab[parent.Children.Length + tabList.Count];
                    for (; k < tabList.Count; k++)
                    {
                        tabs[k] = tabList[k];
                    }

                    for (var j = 0; j < parent.Children.Length; j++)
                    {
                        tabs[j + k] = parent.Children[j];
                    }
                }
                else
                {
                    tabs = parent.Children;
                }

                tabCollection = new TabCollection(tabs);
            }
            else
            {
                tabCollection = new TabCollection(parent.Children);
            }
            return tabCollection;
        }
    }
}

