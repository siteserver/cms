using System;
using System.Text;
using SiteServer.Utils;
using System.Collections.Specialized;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Enumerations;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Core
{
    public static class ChannelLoading
    {
        public static string GetChannelRowHtml(SiteInfo siteInfo, ChannelInfo channelInfo, bool enabled, ELoadingType loadingType, NameValueCollection additional, PermissionsImpl permissionsImpl)
        {
            var nodeTreeItem = ChannelTreeItem.CreateInstance(siteInfo, channelInfo, enabled, permissionsImpl);
            var onlyAdminId = permissionsImpl.GetOnlyAdminId(siteInfo.Id, channelInfo.Id);
            var title = nodeTreeItem.GetItemHtml(loadingType, PageChannel.GetRedirectUrl(siteInfo.Id, channelInfo.Id), onlyAdminId, additional);

            var rowHtml = string.Empty;

            if (loadingType == ELoadingType.ContentTree)
            {
                rowHtml = $@"
<tr treeItemLevel=""{channelInfo.ParentsCount + 1}"">
	<td nowrap>{title}</td>
</tr>
";
            }
            else if (loadingType == ELoadingType.Channel)
            {
                var upLink = string.Empty;
                var downLink = string.Empty;
                var editUrl = string.Empty;
                var checkBoxHtml = string.Empty;

                if (enabled)
                {
                    if (permissionsImpl.HasChannelPermissions(channelInfo.SiteId, channelInfo.Id, ConfigManager.ChannelPermissions.ChannelEdit))
                    {
                        editUrl = $@"<a href=""{PageChannelEdit.GetRedirectUrl(channelInfo.SiteId, channelInfo.Id, PageChannel.GetRedirectUrl(channelInfo.SiteId, channelInfo.Id))}"" onclick=""event.stopPropagation()"">编辑</a>";
                        upLink =
                            $@"<a href=""{PageUtils.GetCmsUrl(channelInfo.SiteId, nameof(PageChannel), new NameValueCollection
                            {
                                {"Subtract", true.ToString()},
                                {"channelId", channelInfo.Id.ToString()}
                            })}"" onclick=""event.stopPropagation()""><img src=""../Pic/icon/up.gif"" border=""0"" alt=""上升"" /></a>";
                        downLink =
                            $@"<a href=""{PageUtils.GetCmsUrl(channelInfo.SiteId, nameof(PageChannel), new NameValueCollection
                            {
                                {"Add", true.ToString()},
                                {"channelId", channelInfo.Id.ToString()}
                            })}"" onclick=""event.stopPropagation()""><img src=""../Pic/icon/down.gif"" border=""0"" alt=""下降"" /></a>";
                    }
                    checkBoxHtml = $@"<input type=""checkbox"" name=""ChannelIDCollection"" value=""{channelInfo.Id}"" onclick=""checkboxClick(this)"" />";
                }

                rowHtml = $@"
<tr treeItemLevel=""{channelInfo.ParentsCount + 1}"" onclick=""activeRow(this);return false;"">
    <td>{title}</td>
    <td class=""text-nowrap"">{channelInfo.GroupNameCollection}</td>
    <td class=""text-nowrap"">{channelInfo.IndexName}</td>
    <td class=""text-center"">{upLink}</td>
    <td class=""text-center"">{downLink}</td>
    <td class=""text-center"">{editUrl}</td>
    <td class=""text-center"">{checkBoxHtml}</td>
</tr>
";
            }
            else if (loadingType == ELoadingType.SiteAnalysis)
            {
                var startDate = TranslateUtils.ToDateTime(additional["StartDate"]);
                var endDate = TranslateUtils.ToDateTime(additional["EndDate"]);

                var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
                var num = DataProvider.ContentRepository.GetCountOfContentAdd(tableName, siteInfo.Id, channelInfo.Id, EScopeType.All, startDate, endDate, string.Empty, ETriState.All);
                var contentAddNum = num == 0 ? "0" : $"<strong>{num}</strong>";

                num = DataProvider.ContentRepository.GetCountOfContentUpdate(tableName, siteInfo.Id, channelInfo.Id, EScopeType.All, startDate, endDate, string.Empty);
                var contentUpdateNum = num == 0 ? "0" : $"<strong>{num}</strong>";

                rowHtml = $@"
<tr treeItemLevel=""{channelInfo.ParentsCount + 1}"">
	<td>{title}</td>
	<td class=""text-center"">{contentAddNum}</td>
	<td class=""text-center"">{contentUpdateNum}</td>
</tr>
";
            }
            else if (loadingType == ELoadingType.TemplateFilePathRule)
            {
                var editLink = string.Empty;

                if (enabled)
                {
                    var showPopWinString = ModalTemplateFilePathRule.GetOpenWindowString(channelInfo.SiteId, channelInfo.Id);
                    editLink = $"<a href=\"javascript:;\" onclick=\"{showPopWinString}\">更改</a>";
                }
                var filePath = PageUtility.GetInputChannelUrl(siteInfo, channelInfo, false);

                rowHtml = $@"
<tr treeItemLevel=""{channelInfo.ParentsCount + 1}"">
	<td>{title}</td>
	<td>{filePath}</td>
	<td class=""text-center"">{editLink}</td>
</tr>
";
            }
            else if (loadingType == ELoadingType.ConfigurationCreateDetails)
            {
                var editChannelLink = string.Empty;

                var nodeNames = string.Empty;

                if (enabled)
                {
                    var showPopWinString = ModalConfigurationCreateChannel.GetOpenWindowString(channelInfo.SiteId, channelInfo.Id);
                    editChannelLink = $"<a href=\"javascript:;\" onclick=\"{showPopWinString}\">触发栏目</a>";
                }

                var nodeNameBuilder = new StringBuilder();
                var channelIdList = TranslateUtils.StringCollectionToIntList(channelInfo.CreateChannelIdsIfContentChanged);
                foreach (var theChannelId in channelIdList)
                {
                    var theNodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, theChannelId);
                    if (theNodeInfo != null)
                    {
                        nodeNameBuilder.Append(theNodeInfo.ChannelName).Append(",");
                    }
                }
                if (nodeNameBuilder.Length > 0)
                {
                    nodeNameBuilder.Length--;
                    nodeNames = nodeNameBuilder.ToString();
                }

                rowHtml = $@"
<tr treeItemLevel=""{channelInfo.ParentsCount + 1}"">
	<td>{title}</td>
	<td>{nodeNames}</td>
	<td class=""text-center"">{editChannelLink}</td>
</tr>
";
            }
            else if (loadingType == ELoadingType.ConfigurationCrossSiteTrans)
            {
                var editLink = string.Empty;

                if (enabled)
                {
                    var showPopWinString = ModalCrossSiteTransEdit.GetOpenWindowString(channelInfo.SiteId, channelInfo.Id);
                    editLink = $"<a href=\"javascript:;\" onclick=\"{showPopWinString}\">更改</a>";
                }

                var contribute = CrossSiteTransUtility.GetDescription(channelInfo.SiteId, channelInfo);

                rowHtml = $@"
<tr treeItemLevel=""{channelInfo.ParentsCount + 1}"">
	<td>{title}</td>
	<td>{contribute}</td>
	<td class=""text-center"">{editLink}</td>
</tr>
";
            }
            else if (loadingType == ELoadingType.ChannelClickSelect)
            {
                rowHtml = $@"
<tr treeItemLevel=""{channelInfo.ParentsCount + 1}"">
	<td>{title}</td>
</tr>
";
            }

            return rowHtml;
        }

        public static string GetScript(SiteInfo siteInfo, string contentModelPluginId, ELoadingType loadingType, NameValueCollection additional)
        {
            return ChannelTreeItem.GetScript(siteInfo, loadingType, contentModelPluginId, additional);
        }

        public static string GetScriptOnLoad(int siteId, int currentChannelId)
        {
            if (currentChannelId == 0 || currentChannelId == siteId) return string.Empty;

            var nodeInfo = ChannelManager.GetChannelInfo(siteId, currentChannelId);
            if (nodeInfo == null) return string.Empty;

            string path;
            if (nodeInfo.ParentId == siteId)
            {
                path = currentChannelId.ToString();
            }
            else
            {
                path = nodeInfo.ParentsPath.Substring(nodeInfo.ParentsPath.IndexOf(",", StringComparison.Ordinal) + 1) + "," + currentChannelId;
            }
            return ChannelTreeItem.GetScriptOnLoad(path);
        }
    }
}
