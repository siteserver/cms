using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.Common.Office
{
    public static class ExcelObject
    {
        public static async Task CreateExcelFileForContents(IPluginManager pluginManager, ITableManager tableManager, ITableStyleRepository tableStyleRepository, IChannelRepository channelRepository, string filePath, Site siteInfo,
            Channel channelInfo, IEnumerable<int> contentIdList, List<string> displayAttributes, bool isPeriods, string startDate,
            string endDate, bool? checkedState)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var head = new List<string>();
            var rows = new List<List<string>>();

            var tableName = await channelRepository.GetTableNameAsync(pluginManager, siteInfo, channelInfo);
            var styleInfoList = ContentUtility.GetAllTableStyleInfoList(await tableManager.GetContentStyleInfoListAsync(pluginManager, siteInfo, channelInfo));

            foreach (var styleInfo in styleInfoList)
            {
                if (displayAttributes.Contains(styleInfo.AttributeName))
                {
                    head.Add(styleInfo.DisplayName);
                }
            }

            if (contentIdList == null || contentIdList.Count() == 0)
            {
                contentIdList = await channelInfo.ContentRepository.GetContentIdListAsync(channelInfo.Id, isPeriods, startDate, endDate, checkedState);
            }

            foreach (var contentId in contentIdList)
            {
                var contentInfo = await channelInfo.ContentRepository.GetContentInfoAsync(contentId);
                if (contentInfo != null)
                {
                    var row = new List<string>();

                    foreach (var styleInfo in styleInfoList)
                    {
                        if (displayAttributes.Contains(styleInfo.AttributeName))
                        {
                            var value = contentInfo.Get<string>(styleInfo.AttributeName);
                            row.Add(StringUtils.StripTags(value));
                        }
                    }

                    rows.Add(row);
                }
            }

            CsvUtils.Export(filePath, head, rows);
        }

        public static async Task CreateExcelFileForContentsAsync(string filePath, Site siteInfo,
            Channel channelInfo, List<Content> contentInfoList, List<string> columnNames)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var head = new List<string>();
            var rows = new List<List<string>>();

            var columns = await channelInfo.ContentRepository.GetContentColumnsAsync(siteInfo, channelInfo, true);

            foreach (var column in columns)
            {
                if (StringUtils.ContainsIgnoreCase(columnNames, column.AttributeName))
                {
                    head.Add(column.DisplayName);
                }
            }

            foreach (var contentInfo in contentInfoList)
            {
                var row = new List<string>();

                foreach (var column in columns)
                {
                    if (StringUtils.ContainsIgnoreCase(columnNames, column.AttributeName))
                    {
                        var value = contentInfo.Get<string>(column.AttributeName);
                        row.Add(StringUtils.StripTags(value));
                    }
                }

                rows.Add(row);
            }

            CsvUtils.Export(filePath, head, rows);
        }

        public static async Task CreateExcelFileForUsersAsync(IUserRepository userRepository, string filePath, ETriState checkedState)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var head = new List<string>
            {
                "用户名",
                "姓名",
                "邮箱",
                "手机",
                "注册时间",
                "最后一次活动时间"
            };
            var rows = new List<List<string>>();

            foreach (var userInfo in await userRepository.GetAllAsync(null))
            {
                rows.Add(new List<string>
                {
                    userInfo.UserName,
                    userInfo.DisplayName,
                    userInfo.Email,
                    userInfo.Mobile,
                    DateUtils.GetDateAndTimeString(userInfo.CreatedDate),
                    DateUtils.GetDateAndTimeString(userInfo.LastActivityDate)
                });
            }

            CsvUtils.Export(filePath, head, rows);
        }

        public static async Task<List<Content>> GetContentsByCsvFileAsync(IPluginManager pluginManager, ITableManager tableManager, ITableStyleRepository tableStyleRepository, string filePath, Site siteInfo,
            Channel nodeInfo)
        {
            var contentInfoList = new List<Content>();

            CsvUtils.Import(filePath, out var head, out var rows);

            if (rows.Count <= 0) return contentInfoList;

            var styleInfoList = ContentUtility.GetAllTableStyleInfoList(await tableManager.GetContentStyleInfoListAsync(pluginManager, siteInfo, nodeInfo));
            var nameValueCollection = new NameValueCollection();
            foreach (var styleInfo in styleInfoList)
            {
                nameValueCollection[styleInfo.DisplayName] = styleInfo.AttributeName.ToLower();
            }

            var attributeNames = new List<string>();
            foreach (var columnName in head)
            {
                attributeNames.Add(!string.IsNullOrEmpty(nameValueCollection[columnName])
                    ? nameValueCollection[columnName]
                    : columnName);
            }

            foreach (var row in rows)
            {
                if (row.Count != attributeNames.Count) continue;

                var dict = new Dictionary<string, object>();

                for (var i = 0; i < attributeNames.Count; i++)
                {
                    var attributeName = attributeNames[i];
                    if (!string.IsNullOrEmpty(attributeName))
                    {
                        dict[attributeName] = row[i];
                    }
                }

                var contentInfo = new Content(dict);

                if (!string.IsNullOrEmpty(contentInfo.Title))
                {
                    contentInfo.SiteId = siteInfo.Id;
                    contentInfo.ChannelId = nodeInfo.Id;

                    contentInfoList.Add(contentInfo);
                }
            }

            return contentInfoList;
        }
    }
}
