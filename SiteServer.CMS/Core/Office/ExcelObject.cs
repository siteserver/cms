using SiteServer.Utils;
using SiteServer.CMS.Model;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model.Db;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Core.Office
{
    public static class ExcelObject
    {
        public static void CreateExcelFileForContents(string filePath, Site site,
            ChannelInfo channelInfo, List<int> contentIdList, List<string> displayAttributes, bool isPeriods, string startDate,
            string endDate, ETriState checkedState)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var head = new List<string>();
            var rows = new List<List<string>>();

            var tableName = ChannelManager.GetTableName(site, channelInfo);
            var styleInfoList = ContentUtility.GetAllTableStyleInfoList(TableStyleManager.GetContentStyleInfoList(site, channelInfo));

            foreach (var styleInfo in styleInfoList)
            {
                if (displayAttributes.Contains(styleInfo.AttributeName))
                {
                    head.Add(styleInfo.DisplayName);
                }
            }

            if (contentIdList == null || contentIdList.Count == 0)
            {
                contentIdList = DataProvider.ContentDao.GetContentIdList(tableName, channelInfo.Id, isPeriods,
                    startDate, endDate, checkedState);
            }

            foreach (var contentId in contentIdList)
            {
                var contentInfo = ContentManager.GetContentInfo(site, channelInfo, contentId);
                if (contentInfo != null)
                {
                    var row = new List<string>();

                    foreach (var styleInfo in styleInfoList)
                    {
                        if (displayAttributes.Contains(styleInfo.AttributeName))
                        {
                            var value = contentInfo.GetString(styleInfo.AttributeName);
                            row.Add(StringUtils.StripTags(value));
                        }
                    }

                    rows.Add(row);
                }
            }

            CsvUtils.Export(filePath, head, rows);
        }

        public static void CreateExcelFileForContents(string filePath, Site site,
            ChannelInfo channelInfo, List<ContentInfo> contentInfoList, List<string> columnNames)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var head = new List<string>();
            var rows = new List<List<string>>();

            var columns = ContentManager.GetContentColumns(site, channelInfo, true);

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
                        var value = contentInfo.GetString(column.AttributeName);
                        row.Add(StringUtils.StripTags(value));
                    }
                }

                rows.Add(row);
            }

            CsvUtils.Export(filePath, head, rows);
        }

        public static async Task CreateExcelFileForUsersAsync(string filePath, ETriState checkedState)
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

            var userIdList = (await DataProvider.UserDao.GetIdListAsync(checkedState != ETriState.False)).ToList();
            if (checkedState == ETriState.All)
            {
                userIdList.AddRange(await DataProvider.UserDao.GetIdListAsync(false));
            }

            foreach (var userId in userIdList)
            {
                var userInfo = await UserManager.GetUserByUserIdAsync(userId);

                rows.Add(new List<string>
                {
                    userInfo.UserName,
                    userInfo.DisplayName,
                    userInfo.Email,
                    userInfo.Mobile,
                    DateUtils.GetDateAndTimeString(userInfo.CreateDate),
                    DateUtils.GetDateAndTimeString(userInfo.LastActivityDate)
                });
            }

            CsvUtils.Export(filePath, head, rows);
        }

        public static List<ContentInfo> GetContentsByCsvFile(string filePath, Site site,
            ChannelInfo nodeInfo)
        {
            var contentInfoList = new List<ContentInfo>();

            CsvUtils.Import(filePath, out var head, out var rows);

            if (rows.Count <= 0) return contentInfoList;

            var styleInfoList = ContentUtility.GetAllTableStyleInfoList(TableStyleManager.GetContentStyleInfoList(site, nodeInfo));
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

                var contentInfo = new ContentInfo(dict);

                if (!string.IsNullOrEmpty(contentInfo.Title))
                {
                    contentInfo.SiteId = site.Id;
                    contentInfo.ChannelId = nodeInfo.Id;
                    contentInfo.LastEditDate = DateTime.Now;

                    contentInfoList.Add(contentInfo);
                }
            }

            return contentInfoList;
        }
    }
}
