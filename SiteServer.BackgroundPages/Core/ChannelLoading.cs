using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
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
            var title = await nodeTreeItem.GetItemHtmlAsync(loadingType, string.Empty, additional);

            var rowHtml = string.Empty;

            if (loadingType == ELoadingType.ContentTree)
            {
                rowHtml = $@"
<tr treeItemLevel=""{node.ParentsCount + 1}"">
	<td nowrap>{title}</td>
</tr>
";
            }
            else if (loadingType == ELoadingType.SiteAnalysis)
            {
                var startDate = TranslateUtils.ToDateTime(additional["StartDate"]);
                var endDate = TranslateUtils.ToDateTime(additional["EndDate"]);

                var tableName = DataProvider.ChannelRepository.GetTableNameAsync(site, node).GetAwaiter().GetResult();
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
    }
}
