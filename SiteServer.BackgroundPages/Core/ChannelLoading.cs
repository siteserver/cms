using System;
using System.Text;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.Repositories;
using SiteServer.Abstractions;

namespace SiteServer.BackgroundPages.Core
{
    public static class ChannelLoading
    {
        public static async Task<string> GetChannelRowHtmlAsync(Site site, Channel node, bool enabled, ELoadingType loadingType, NameValueCollection additional, PermissionsImpl permissionsImpl)
        {
            var nodeTreeItem = ChannelTreeItem.CreateInstance(site, node, enabled, permissionsImpl);
            var adminId = await permissionsImpl.GetAdminIdAsync(site.Id, node.Id);
            var title = await nodeTreeItem.GetItemHtmlAsync(loadingType, PageChannel.GetRedirectUrl(site.Id, node.Id), adminId, additional);

            var rowHtml = string.Empty;

            if (loadingType == ELoadingType.ContentTree)
            {
                rowHtml = $@"
<tr treeItemLevel=""{node.ParentsCount + 1}"">
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
                    if (await permissionsImpl.HasChannelPermissionsAsync(node.SiteId, node.Id, Constants.ChannelPermissions.ChannelEdit))
                    {
                        editUrl = $@"<a href=""{PageChannelEdit.GetRedirectUrl(node.SiteId, node.Id, PageChannel.GetRedirectUrl(node.SiteId, node.Id))}"" onclick=""event.stopPropagation()"">编辑</a>";
                        upLink =
                            $@"<a href=""{PageUtils.GetCmsUrl(node.SiteId, nameof(PageChannel), new NameValueCollection
                            {
                                {"Subtract", true.ToString()},
                                {"channelId", node.Id.ToString()}
                            })}"" onclick=""event.stopPropagation()""><img src=""../Pic/icon/up.gif"" border=""0"" alt=""上升"" /></a>";
                        downLink =
                            $@"<a href=""{PageUtils.GetCmsUrl(node.SiteId, nameof(PageChannel), new NameValueCollection
                            {
                                {"Add", true.ToString()},
                                {"channelId", node.Id.ToString()}
                            })}"" onclick=""event.stopPropagation()""><img src=""../Pic/icon/down.gif"" border=""0"" alt=""下降"" /></a>";
                    }
                    checkBoxHtml = $@"<input type=""checkbox"" name=""ChannelIDCollection"" value=""{node.Id}"" onclick=""checkboxClick(this)"" />";
                }

                rowHtml = $@"
<tr treeItemLevel=""{node.ParentsCount + 1}"" onclick=""activeRow(this);return false;"">
    <td>{title}</td>
    <td class=""text-nowrap"">{StringUtils.Join(node.GroupNames)}</td>
    <td class=""text-nowrap"">{node.IndexName}</td>
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

                var tableName = ChannelManager.GetTableNameAsync(site, node).GetAwaiter().GetResult();
                var num = DataProvider.ContentRepository.GetCountOfContentAddAsync(tableName, site.Id, node.Id, EScopeType.All, startDate, endDate, string.Empty, ETriState.All).GetAwaiter().GetResult();
                var contentAddNum = num == 0 ? "0" : $"<strong>{num}</strong>";

                num = DataProvider.ContentRepository.GetCountOfContentUpdateAsync(tableName, site.Id, node.Id, EScopeType.All, startDate, endDate, string.Empty).GetAwaiter().GetResult();
                var contentUpdateNum = num == 0 ? "0" : $"<strong>{num}</strong>";

                rowHtml = $@"
<tr treeItemLevel=""{node.ParentsCount + 1}"">
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
                    var showPopWinString = ModalTemplateFilePathRule.GetOpenWindowString(node.SiteId, node.Id);
                    editLink = $"<a href=\"javascript:;\" onclick=\"{showPopWinString}\">更改</a>";
                }
                var filePath = PageUtility.GetInputChannelUrlAsync(site, node, false).GetAwaiter().GetResult();

                rowHtml = $@"
<tr treeItemLevel=""{node.ParentsCount + 1}"">
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
                    var showPopWinString = ModalConfigurationCreateChannel.GetOpenWindowString(node.SiteId, node.Id);
                    editChannelLink = $"<a href=\"javascript:;\" onclick=\"{showPopWinString}\">触发栏目</a>";
                }

                var nodeNameBuilder = new StringBuilder();
                var channelIdList = StringUtils.GetIntList(node.CreateChannelIdsIfContentChanged);
                foreach (var theChannelId in channelIdList)
                {
                    var theNodeInfo = await ChannelManager.GetChannelAsync(site.Id, theChannelId);
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
<tr treeItemLevel=""{node.ParentsCount + 1}"">
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
                    var showPopWinString = ModalCrossSiteTransEdit.GetOpenWindowString(node.SiteId, node.Id);
                    editLink = $"<a href=\"javascript:;\" onclick=\"{showPopWinString}\">更改</a>";
                }

                var contribute = CrossSiteTransUtility.GetDescriptionAsync(node.SiteId, node).GetAwaiter().GetResult();

                rowHtml = $@"
<tr treeItemLevel=""{node.ParentsCount + 1}"">
	<td>{title}</td>
	<td>{contribute}</td>
	<td class=""text-center"">{editLink}</td>
</tr>
";
            }
            else if (loadingType == ELoadingType.ChannelClickSelect)
            {
                rowHtml = $@"
<tr treeItemLevel=""{node.ParentsCount + 1}"">
	<td>{title}</td>
</tr>
";
            }

            return rowHtml;
        }

        public static string GetScript(Site site, string contentModelPluginId, ELoadingType loadingType, NameValueCollection additional)
        {
            return ChannelTreeItem.GetScript(site, loadingType, contentModelPluginId, additional);
        }

        public static string GetScriptOnLoad(int siteId, int currentChannelId)
        {
            if (currentChannelId == 0 || currentChannelId == siteId) return string.Empty;

            var nodeInfo = ChannelManager.GetChannelAsync(siteId, currentChannelId).GetAwaiter().GetResult();
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
