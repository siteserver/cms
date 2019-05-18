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
        public static class ChannelUI
        {
            public static void AddListItems(ListItemCollection listItemCollection, SiteInfo siteInfo, bool isSeeOwning, bool isShowContentNum, PermissionsImpl permissionsImpl)
            {
                var list = ChannelManager.GetChannelIdList(siteInfo.Id);
                var nodeCount = list.Count;
                var isLastNodeArray = new bool[nodeCount];
                foreach (var channelId in list)
                {
                    var enabled = true;
                    if (isSeeOwning)
                    {
                        enabled = permissionsImpl.IsOwningChannelId(channelId);
                        if (!enabled)
                        {
                            if (!permissionsImpl.IsDescendantOwningChannelId(siteInfo.Id, channelId)) continue;
                        }
                    }
                    var nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);

                    var listitem = new ListItem(ChannelManager.GetSelectText(siteInfo, nodeInfo, permissionsImpl, isLastNodeArray, isShowContentNum), nodeInfo.Id.ToString());
                    if (!enabled)
                    {
                        listitem.Attributes.Add("style", "color:gray;");
                    }
                    listItemCollection.Add(listitem);
                }
            }

            public static void AddListItems(ListItemCollection listItemCollection, SiteInfo siteInfo, bool isSeeOwning, bool isShowContentNum, string contentModelId, PermissionsImpl permissionsImpl)
            {
                var list = ChannelManager.GetChannelIdList(siteInfo.Id);
                var nodeCount = list.Count;
                var isLastNodeArray = new bool[nodeCount];
                foreach (var channelId in list)
                {
                    var enabled = true;
                    if (isSeeOwning)
                    {
                        enabled = permissionsImpl.IsOwningChannelId(channelId);
                        if (!enabled)
                        {
                            if (!permissionsImpl.IsDescendantOwningChannelId(siteInfo.Id, channelId)) continue;
                        }
                    }
                    var nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);

                    var listitem = new ListItem(ChannelManager.GetSelectText(siteInfo, nodeInfo, permissionsImpl, isLastNodeArray, isShowContentNum), nodeInfo.Id.ToString());
                    if (!enabled)
                    {
                        listitem.Attributes.Add("style", "color:gray;");
                    }
                    if (!StringUtils.EqualsIgnoreCase(nodeInfo.ContentModelPluginId, contentModelId))
                    {
                        listitem.Attributes.Add("disabled", "disabled");
                    }
                    listItemCollection.Add(listitem);
                }
            }

            public static void AddListItemsForAddContent(ListItemCollection listItemCollection, SiteInfo siteInfo, bool isSeeOwning, PermissionsImpl permissionsImpl)
            {
                var list = ChannelManager.GetChannelIdList(siteInfo.Id);
                var nodeCount = list.Count;
                var isLastNodeArray = new bool[nodeCount];
                foreach (var channelId in list)
                {
                    var enabled = true;
                    if (isSeeOwning)
                    {
                        enabled = permissionsImpl.IsOwningChannelId(channelId);
                    }

                    var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                    if (enabled)
                    {
                        if (channelInfo.IsContentAddable == false) enabled = false;
                    }

                    if (!enabled)
                    {
                        continue;
                    }

                    var listitem = new ListItem(ChannelManager.GetSelectText(siteInfo, channelInfo, permissionsImpl, isLastNodeArray, true), channelInfo.Id.ToString());
                    listItemCollection.Add(listitem);
                }
            }

            /// <summary>
            /// 得到栏目，并且不对（栏目是否可添加内容）进行判断
            /// 提供给触发器页面使用
            /// 使用场景：其他栏目的内容变动之后，设置某个栏目（此栏目不能添加内容）触发生成
            /// </summary>
            public static void AddListItemsForCreateChannel(ListItemCollection listItemCollection, SiteInfo siteInfo, bool isSeeOwning, PermissionsImpl permissionsImpl)
            {
                var list = ChannelManager.GetChannelIdList(siteInfo.Id);
                var nodeCount = list.Count;
                var isLastNodeArray = new bool[nodeCount];
                foreach (var channelId in list)
                {
                    var enabled = true;
                    if (isSeeOwning)
                    {
                        enabled = permissionsImpl.IsOwningChannelId(channelId);
                    }

                    var nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);

                    if (!enabled)
                    {
                        continue;
                    }

                    var listitem = new ListItem(ChannelManager.GetSelectText(siteInfo, nodeInfo, permissionsImpl, isLastNodeArray, true), nodeInfo.Id.ToString());
                    listItemCollection.Add(listitem);
                }
            }
        }
    }
}