using System;
using System.Text;
using SiteServer.Utils;
using System.Collections.Specialized;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Core
{
    public class ChannelLoading
    {
        public static string GetChannelRowHtml(SiteInfo siteInfo, ChannelInfo nodeInfo, bool enabled, ELoadingType loadingType, NameValueCollection additional, string administratorName)
        {
            var nodeTreeItem = NodeTreeItem.CreateInstance(siteInfo, nodeInfo, enabled, administratorName);
            var title = nodeTreeItem.GetItemHtml(loadingType, PageChannel.GetRedirectUrl(siteInfo.Id, nodeInfo.Id), additional);

            var rowHtml = string.Empty;

            if (loadingType == ELoadingType.ContentTree)
            {
                rowHtml = $@"
<tr treeItemLevel=""{nodeInfo.ParentsCount + 1}"">
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
                    if (AdminUtility.HasChannelPermissions(administratorName, nodeInfo.SiteId, nodeInfo.Id, ConfigManager.Permissions.Channel.ChannelEdit))
                    {
                        var urlEdit = PageChannelEdit.GetRedirectUrl(nodeInfo.SiteId, nodeInfo.Id, PageChannel.GetRedirectUrl(nodeInfo.SiteId, nodeInfo.Id));
                        editUrl = $"<a href=\"{urlEdit}\">编辑</a>";
                        var urlSubtract = PageUtils.GetCmsUrl(nodeInfo.SiteId, nameof(PageChannel), new NameValueCollection
                        {
                            {"Subtract", true.ToString()},
                            {"channelId", nodeInfo.Id.ToString()}
                        });
                        upLink =
                            $@"<a href=""{urlSubtract}""><img src=""../Pic/icon/up.gif"" border=""0"" alt=""上升"" /></a>";
                        var urlAdd = PageUtils.GetCmsUrl(nodeInfo.SiteId, nameof(PageChannel), new NameValueCollection
                        {
                            {"Add", true.ToString()},
                            {"channelId", nodeInfo.Id.ToString()}
                        });
                        downLink =
                            $@"<a href=""{urlAdd}""><img src=""../Pic/icon/down.gif"" border=""0"" alt=""下降"" /></a>";
                    }
                    checkBoxHtml = $"<input type='checkbox' name='ChannelIDCollection' value='{nodeInfo.Id}' />";
                }

                rowHtml = $@"
<tr treeItemLevel=""{nodeInfo.ParentsCount + 1}"">
    <td>{title}</td>
    <td class=""text-nowrap"">{nodeInfo.GroupNameCollection}</td>
    <td class=""text-nowrap"">{nodeInfo.IndexName}</td>
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

                var tableName = ChannelManager.GetTableName(siteInfo, nodeInfo);
                var num = DataProvider.ContentDao.GetCountOfContentAdd(tableName, siteInfo.Id, nodeInfo.Id, EScopeType.All, startDate, endDate, string.Empty);
                var contentAddNum = num == 0 ? "0" : $"<strong>{num}</strong>";

                num = DataProvider.ContentDao.GetCountOfContentUpdate(tableName, siteInfo.Id, nodeInfo.Id, EScopeType.All, startDate, endDate, string.Empty);
                var contentUpdateNum = num == 0 ? "0" : $"<strong>{num}</strong>";

                rowHtml = $@"
<tr treeItemLevel=""{nodeInfo.ParentsCount + 1}"">
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
                    var showPopWinString = ModalTemplateFilePathRule.GetOpenWindowString(nodeInfo.SiteId, nodeInfo.Id);
                    editLink = $"<a href=\"javascript:;\" onclick=\"{showPopWinString}\">更改</a>";
                }
                var filePath = PageUtility.GetInputChannelUrl(siteInfo, nodeInfo, false);

                rowHtml = $@"
<tr treeItemLevel=""{nodeInfo.ParentsCount + 1}"">
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
                    var showPopWinString = ModalConfigurationCreateChannel.GetOpenWindowString(nodeInfo.SiteId, nodeInfo.Id);
                    editChannelLink = $"<a href=\"javascript:;\" onclick=\"{showPopWinString}\">触发栏目</a>";
                }

                if (nodeInfo.Additional.ToNameValueCollection().Count > 0)
                {
                    var nodeNameBuilder = new StringBuilder();
                    var channelIdList = TranslateUtils.StringCollectionToIntList(nodeInfo.Additional.CreateChannelIDsIfContentChanged);
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
                }

                rowHtml = $@"
<tr treeItemLevel=""{nodeInfo.ParentsCount + 1}"">
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
                    var showPopWinString = ModalCrossSiteTransEdit.GetOpenWindowString(nodeInfo.SiteId, nodeInfo.Id);
                    editLink = $"<a href=\"javascript:;\" onclick=\"{showPopWinString}\">更改</a>";
                }

                var contribute = CrossSiteTransUtility.GetDescription(nodeInfo.SiteId, nodeInfo);

                rowHtml = $@"
<tr treeItemLevel=""{nodeInfo.ParentsCount + 1}"">
	<td>{title}</td>
	<td>{contribute}</td>
	<td class=""text-center"">{editLink}</td>
</tr>
";
            }
            else if (loadingType == ELoadingType.ChannelSelect)
            {
                rowHtml = $@"
<tr treeItemLevel=""{nodeInfo.ParentsCount + 1}"">
	<td>{title}</td>
</tr>
";
            }

            return rowHtml;
        }

        public static string GetScript(SiteInfo siteInfo, ELoadingType loadingType, NameValueCollection additional)
        {
            return NodeTreeItem.GetScript(siteInfo, loadingType, additional);
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
            return NodeTreeItem.GetScriptOnLoad(path);
        }
    }
}
