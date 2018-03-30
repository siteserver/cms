using SiteServer.Utils;
using SiteServer.CMS.Model;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Core.Office
{
    public class ExcelObject
    {
        public static void CreateExcelFileForContents(string filePath, SiteInfo siteInfo,
            ChannelInfo nodeInfo, List<int> contentIdList, List<string> displayAttributes, bool isPeriods, string startDate,
            string endDate, ETriState checkedState)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var head = new List<string>();
            var rows = new List<List<string>>();

            var relatedidentityes =
                RelatedIdentities.GetChannelRelatedIdentities(siteInfo.Id, nodeInfo.Id);
            var tableName = ChannelManager.GetTableName(siteInfo, nodeInfo);
            var styleInfoList = ContentUtility.GetAllTableStyleInfoList(TableStyleManager.GetTableStyleInfoList(tableName, relatedidentityes));

            foreach (var styleInfo in styleInfoList)
            {
                if (displayAttributes.Contains(styleInfo.AttributeName))
                {
                    head.Add(styleInfo.DisplayName);
                }
            }

            if (contentIdList == null || contentIdList.Count == 0)
            {
                contentIdList = DataProvider.ContentDao.GetContentIdList(tableName, nodeInfo.Id, isPeriods,
                    startDate, endDate, checkedState);
            }

            foreach (var contentId in contentIdList)
            {
                var contentInfo = DataProvider.ContentDao.GetContentInfo(tableName, contentId);
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

        public static void CreateExcelFileForUsers(string filePath, ETriState checkedState)
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

            List<int> userIdList = DataProvider.UserDao.GetIdList(checkedState != ETriState.False);
            if (checkedState == ETriState.All)
            {
                userIdList.AddRange(DataProvider.UserDao.GetIdList(false));
            }

            foreach (var userId in userIdList)
            {
                var userInfo = DataProvider.UserDao.GetUserInfo(userId);

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

        public static List<ContentInfo> GetContentsByCsvFile(string filePath, SiteInfo siteInfo,
            ChannelInfo nodeInfo)
        {
            var contentInfoList = new List<ContentInfo>();

            List<string> head;
            List<List<string>> rows;
            CsvUtils.Import(filePath, out head, out rows);

            if (rows.Count <= 0) return contentInfoList;

            var relatedidentityes =
                RelatedIdentities.GetChannelRelatedIdentities(
                    siteInfo.Id, nodeInfo.Id);
            var tableName = ChannelManager.GetTableName(siteInfo, nodeInfo);
            // ArrayList tableStyleInfoArrayList = TableStyleManager.GetTableStyleInfoArrayList(ETableStyle.BackgroundContent, siteInfo.AuxiliaryTableForContent, relatedidentityes);

            var styleInfoList = ContentUtility.GetAllTableStyleInfoList(TableStyleManager.GetTableStyleInfoList(tableName, relatedidentityes));
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
                var contentInfo = new ContentInfo();
                if (row.Count != attributeNames.Count) continue;

                for (var i = 0; i < attributeNames.Count; i++)
                {
                    var attributeName = attributeNames[i];
                    if (!string.IsNullOrEmpty(attributeName))
                    {
                        var value = row[i];
                        contentInfo.Set(attributeName, value);
                    }
                }

                if (!string.IsNullOrEmpty(contentInfo.Title))
                {
                    contentInfo.SiteId = siteInfo.Id;
                    contentInfo.ChannelId = nodeInfo.Id;
                    contentInfo.LastEditDate = DateTime.Now;

                    contentInfoList.Add(contentInfo);
                }
            }

            return contentInfoList;
        }
    }
}
