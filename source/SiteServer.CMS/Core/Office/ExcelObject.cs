using System.Collections;
using System.Data.OleDb;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.IO;
using SiteServer.CMS.Model;
using System;
using System.Data;
using System.Collections.Specialized;
using System.Collections.Generic;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.CMS.Core.Office
{
    public class ExcelObject
    {
        public static void CreateExcelFileForInputContents(string filePath, PublishmentSystemInfo publishmentSystemInfo,
            InputInfo inputInfo)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var head = new List<string>();
            var rows = new List<List<string>>();

            var relatedidentityes = RelatedIdentities.GetRelatedIdentities(ETableStyle.InputContent,
                publishmentSystemInfo.PublishmentSystemId, inputInfo.InputId);
            var tableStyleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.InputContent,
                DataProvider.InputContentDao.TableName, relatedidentityes);

            if (tableStyleInfoList.Count == 0)
            {
                throw new Exception("表单无字段，无法导出");
            }

            foreach (var tableStyleInfo in tableStyleInfoList)
            {
                head.Add(tableStyleInfo.DisplayName);
            }

            if (inputInfo.IsReply)
            {
                head.Add("回复");
            }
            head.Add("添加时间");

            var contentIdList = DataProvider.InputContentDao.GetContentIdListWithChecked(inputInfo.InputId);
            foreach (var contentId in contentIdList)
            {
                var contentInfo = DataProvider.InputContentDao.GetContentInfo(contentId);
                if (contentInfo != null)
                {
                    var row = new List<string>();

                    foreach (var tableStyleInfo in tableStyleInfoList)
                    {
                        var value = contentInfo.Attributes.Get(tableStyleInfo.AttributeName);

                        if (!string.IsNullOrEmpty(value))
                        {
                            value = InputParserUtility.GetContentByTableStyle(value, publishmentSystemInfo,
                                ETableStyle.InputContent, tableStyleInfo);
                        }

                        row.Add(StringUtils.StripTags(value));
                    }

                    if (inputInfo.IsReply)
                    {
                        row.Add(StringUtils.StripTags(contentInfo.Reply));
                    }
                    row.Add(DateUtils.GetDateAndTimeString(contentInfo.AddDate));

                    rows.Add(row);
                }
            }

            CsvUtils.Export(filePath, head, rows);
        }

        public static void CreateExcelFileForContents(string filePath, PublishmentSystemInfo publishmentSystemInfo,
            NodeInfo nodeInfo, List<int> contentIdList, List<string> displayAttributes, bool isPeriods, string startDate,
            string endDate, ETriState checkedState)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var head = new List<string>();
            var rows = new List<List<string>>();

            var relatedidentityes =
                RelatedIdentities.GetChannelRelatedIdentities(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId);
            var modelInfo = ContentModelManager.GetContentModelInfo(publishmentSystemInfo, nodeInfo.ContentModelId);
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
            var tableStyleInfoList = TableStyleManager.GetTableStyleInfoList(tableStyle, modelInfo.TableName,
                relatedidentityes);
            tableStyleInfoList = ContentUtility.GetAllTableStyleInfoList(publishmentSystemInfo, tableStyle,
                tableStyleInfoList);

            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);

            foreach (var tableStyleInfo in tableStyleInfoList)
            {
                if (displayAttributes.Contains(tableStyleInfo.AttributeName))
                {
                    head.Add(tableStyleInfo.DisplayName);
                }
            }

            if (contentIdList == null || contentIdList.Count == 0)
            {
                contentIdList = BaiRongDataProvider.ContentDao.GetContentIdList(tableName, nodeInfo.NodeId, isPeriods,
                    startDate, endDate, checkedState);
            }

            foreach (var contentId in contentIdList)
            {
                var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);
                if (contentInfo != null)
                {
                    var row = new List<string>();

                    foreach (var tableStyleInfo in tableStyleInfoList)
                    {
                        if (displayAttributes.Contains(tableStyleInfo.AttributeName))
                        {
                            var value = contentInfo.GetExtendedAttribute(tableStyleInfo.AttributeName);
                            row.Add(StringUtils.StripTags(value));
                        }
                    }

                    rows.Add(row);
                }
            }

            CsvUtils.Export(filePath, head, rows);
        }

        public static void CreateExcelFileForComments(string filePath, PublishmentSystemInfo publishmentSystemInfo,
            int nodeId, int contentId)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var head = new List<string>();
            var rows = new List<List<string>>();

            head.Add("用户");
            head.Add("评论");
            head.Add("添加时间");

            var commentInfoList =
                DataProvider.CommentDao.GetCommentInfoListChecked(publishmentSystemInfo.PublishmentSystemId, nodeId,
                    contentId, 10000, 0);
            foreach (var commentInfo in commentInfoList)
            {
                var row = new List<string>
                {
                    commentInfo.UserName,
                    StringUtils.StripTags(commentInfo.Content),
                    DateUtils.GetDateAndTimeString(commentInfo.AddDate)
                };

                rows.Add(row);
            }

            CsvUtils.Export(filePath, head, rows);
        }

        public static void CreateExcelFileForTrackingHours(string filePath, int publishmentSystemId)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var head = new List<string>();
            var rows = new List<List<string>>();

            var trackingHourHashtable = DataProvider.TrackingDao.GetTrackingHourHashtable(publishmentSystemId);
            var uniqueTrackingHourHashtable =
                DataProvider.TrackingDao.GetUniqueTrackingHourHashtable(publishmentSystemId);

            head.Add("时间段");
            head.Add("访问量");
            head.Add("访客数");

            var maxAccessNum = 0;
            var uniqueMaxAccessNum = 0;

            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            for (var i = 0; i < 24; i++)
            {
                var datetime = now.AddHours(-i);
                var accessNum = 0;
                if (trackingHourHashtable[datetime] != null)
                {
                    accessNum = (int) trackingHourHashtable[datetime];
                }
                if (accessNum > maxAccessNum)
                {
                    maxAccessNum = accessNum;
                }

                var uniqueAccessNum = 0;
                if (uniqueTrackingHourHashtable[datetime] != null)
                {
                    uniqueAccessNum = (int) uniqueTrackingHourHashtable[datetime];
                }
                if (uniqueAccessNum > uniqueMaxAccessNum)
                {
                    uniqueMaxAccessNum = uniqueAccessNum;
                }

                var row = new List<string>
                {
                    datetime.Hour.ToString(),
                    accessNum.ToString(),
                    uniqueAccessNum.ToString()
                };

                rows.Add(row);
            }

            CsvUtils.Export(filePath, head, rows);
        }

        public static void CreateExcelFileForTrackingDays(string filePath, int publishmentSystemId)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var head = new List<string>();
            var rows = new List<List<string>>();

            var trackingDayHashtable = DataProvider.TrackingDao.GetTrackingDayHashtable(publishmentSystemId);
            var uniqueTrackingDayHashtable = DataProvider.TrackingDao.GetUniqueTrackingDayHashtable(publishmentSystemId);

            head.Add("时间段");
            head.Add("访问量");
            head.Add("访客数");

            var maxAccessNum = 0;
            var uniqueMaxAccessNum = 0;

            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            for (var i = 0; i < 30; i++)
            {
                var datetime = now.AddDays(-i);
                var accessNum = 0;
                if (trackingDayHashtable[datetime] != null)
                {
                    accessNum = (int) trackingDayHashtable[datetime];
                }
                if (accessNum > maxAccessNum)
                {
                    maxAccessNum = accessNum;
                }

                var uniqueAccessNum = 0;
                if (uniqueTrackingDayHashtable[datetime] != null)
                {
                    uniqueAccessNum = (int) uniqueTrackingDayHashtable[datetime];
                }
                if (uniqueAccessNum > uniqueMaxAccessNum)
                {
                    uniqueMaxAccessNum = uniqueAccessNum;
                }

                rows.Add(new List<string>
                {
                    datetime.Day.ToString(),
                    accessNum.ToString(),
                    uniqueAccessNum.ToString()
                });
            }

            CsvUtils.Export(filePath, head, rows);
        }

        public static void CreateExcelFileForTrackingMonths(string filePath, int publishmentSystemId)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var head = new List<string>();
            var rows = new List<List<string>>();

            var trackingMonthHashtable = DataProvider.TrackingDao.GetTrackingMonthHashtable(publishmentSystemId);
            var uniqueTrackingMonthHashtable =
                DataProvider.TrackingDao.GetUniqueTrackingMonthHashtable(publishmentSystemId);

            head.Add("时间段");
            head.Add("访问量");
            head.Add("访客数");

            var maxAccessNum = 0;
            var uniqueMaxAccessNum = 0;

            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            for (var i = 0; i < 12; i++)
            {
                var datetime = now.AddMonths(-i);
                var accessNum = 0;
                if (trackingMonthHashtable[datetime] != null)
                {
                    accessNum = (int) trackingMonthHashtable[datetime];
                }
                if (accessNum > maxAccessNum)
                {
                    maxAccessNum = accessNum;
                }

                var uniqueAccessNum = 0;
                if (uniqueTrackingMonthHashtable[datetime] != null)
                {
                    uniqueAccessNum = (int) uniqueTrackingMonthHashtable[datetime];
                }
                if (uniqueAccessNum > uniqueMaxAccessNum)
                {
                    uniqueMaxAccessNum = uniqueAccessNum;
                }

                rows.Add(new List<string>
                {
                    datetime.Month.ToString(),
                    accessNum.ToString(),
                    uniqueAccessNum.ToString()
                });
            }

            CsvUtils.Export(filePath, head, rows);
        }

        public static void CreateExcelFileForTrackingYears(string filePath, int publishmentSystemId)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var head = new List<string>();
            var rows = new List<List<string>>();

            var trackingYearHashtable = DataProvider.TrackingDao.GetTrackingYearHashtable(publishmentSystemId);
            var uniqueTrackingYearHashtable =
                DataProvider.TrackingDao.GetUniqueTrackingYearHashtable(publishmentSystemId);

            head.Add("时间段");
            head.Add("访问量");
            head.Add("访客数");

            var maxAccessNum = 0;
            var uniqueMaxAccessNum = 0;

            var now = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);
            for (var i = 0; i < 10; i++)
            {
                var datetime = now.AddYears(-i);
                var accessNum = 0;
                if (trackingYearHashtable[datetime] != null)
                {
                    accessNum = (int) trackingYearHashtable[datetime];
                }
                if (accessNum > maxAccessNum)
                {
                    maxAccessNum = accessNum;
                }

                var uniqueAccessNum = 0;
                if (uniqueTrackingYearHashtable[datetime] != null)
                {
                    uniqueAccessNum = (int) uniqueTrackingYearHashtable[datetime];
                }
                if (uniqueAccessNum > uniqueMaxAccessNum)
                {
                    uniqueMaxAccessNum = uniqueAccessNum;
                }

                rows.Add(new List<string>
                {
                    datetime.Year.ToString(),
                    accessNum.ToString(),
                    uniqueAccessNum.ToString()
                });
            }

            CsvUtils.Export(filePath, head, rows);
        }

        public static void CreateExcelFileForTrackingContents(string filePath, string startDateString,
            string endDateString, PublishmentSystemInfo publishmentSystemInfo, int nodeId, int contentId, int totalNum,
            bool isDelete)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var head = new List<string>
            {
                "目标页面",
                "上级栏目",
                "上上级栏目",
                "IP地址",
                "访问时间",
                "访问来源"
            };
            var rows = new List<List<string>>();

            var target = string.Empty;
            var upChannel = string.Empty;
            var upupChannel = string.Empty;
            if (contentId != 0)
            {
                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeId);
                target = BaiRongDataProvider.ContentDao.GetValue(tableName, contentId, ContentAttribute.Title);
                upChannel = NodeManager.GetNodeName(publishmentSystemInfo.PublishmentSystemId, nodeId);
                if (nodeId != publishmentSystemInfo.PublishmentSystemId)
                {
                    upupChannel = NodeManager.GetNodeName(publishmentSystemInfo.PublishmentSystemId,
                        NodeManager.GetParentId(publishmentSystemInfo.PublishmentSystemId, nodeId));
                }
            }

            var begin = DateUtils.SqlMinValue;
            if (!string.IsNullOrEmpty(startDateString))
            {
                begin = TranslateUtils.ToDateTime(startDateString);
            }
            var end = TranslateUtils.ToDateTime(endDateString);

            var ipAddresses =
                DataProvider.TrackingDao.GetContentIpAddressArrayList(publishmentSystemInfo.PublishmentSystemId, nodeId,
                    contentId, begin, end);
            var trackingInfoArrayList =
                DataProvider.TrackingDao.GetTrackingInfoArrayList(publishmentSystemInfo.PublishmentSystemId, nodeId,
                    contentId, begin, end);

            var ipAddressWithNumSortedList = new SortedList();
            foreach (string ipAddress in ipAddresses)
            {
                if (ipAddressWithNumSortedList[ipAddress] != null)
                {
                    ipAddressWithNumSortedList[ipAddress] = (int) ipAddressWithNumSortedList[ipAddress] + 1;
                }
                else
                {
                    ipAddressWithNumSortedList[ipAddress] = 1;
                }
            }

            foreach (TrackingInfo trackingInfo in trackingInfoArrayList)
            {
                if (contentId == 0)
                {
                    if (trackingInfo.PageContentId != 0)
                    {
                        var tableName = NodeManager.GetTableName(publishmentSystemInfo, trackingInfo.PageNodeId);
                        target = BaiRongDataProvider.ContentDao.GetValue(tableName, trackingInfo.PageContentId,
                            ContentAttribute.Title);
                        upChannel = NodeManager.GetNodeName(publishmentSystemInfo.PublishmentSystemId,
                            trackingInfo.PageNodeId);
                        if (trackingInfo.PageNodeId != publishmentSystemInfo.PublishmentSystemId)
                        {
                            upupChannel = NodeManager.GetNodeName(publishmentSystemInfo.PublishmentSystemId,
                                NodeManager.GetParentId(publishmentSystemInfo.PublishmentSystemId,
                                    trackingInfo.PageNodeId));
                        }
                    }
                    else if (trackingInfo.PageNodeId != 0)
                    {
                        target = NodeManager.GetNodeName(publishmentSystemInfo.PublishmentSystemId,
                            trackingInfo.PageNodeId);
                        if (trackingInfo.PageNodeId != publishmentSystemInfo.PublishmentSystemId)
                        {
                            var upChannelId = NodeManager.GetParentId(publishmentSystemInfo.PublishmentSystemId,
                                trackingInfo.PageNodeId);
                            upChannel = NodeManager.GetNodeName(publishmentSystemInfo.PublishmentSystemId, upChannelId);
                            if (upChannelId != publishmentSystemInfo.PublishmentSystemId)
                            {
                                upupChannel = NodeManager.GetNodeName(publishmentSystemInfo.PublishmentSystemId,
                                    NodeManager.GetParentId(publishmentSystemInfo.PublishmentSystemId, upChannelId));
                            }
                        }
                    }
                }
                var ipAddress = trackingInfo.IpAddress;
                var accessDate = trackingInfo.AccessDateTime.ToString(DateUtils.FormatStringDateTime);
                var referrer = trackingInfo.Referrer;

                rows.Add(new List<string>
                {
                    target,
                    upChannel,
                    upupChannel,
                    ipAddress,
                    accessDate,
                    referrer
                });
            }

            CsvUtils.Export(filePath, head, rows);

            if (isDelete)
            {
                DataProvider.TrackingDao.DeleteAll(publishmentSystemInfo.PublishmentSystemId);
            }
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
                "用户组",
                "注册时间",
                "最后一次活动时间"
            };
            var rows = new List<List<string>>();

            var userIdList = BaiRongDataProvider.UserDao.GetUserIdList(checkedState != ETriState.False);
            if (checkedState == ETriState.All)
            {
                userIdList.AddRange(BaiRongDataProvider.UserDao.GetUserIdList(false));
            }

            foreach (var userId in userIdList)
            {
                var userInfo = BaiRongDataProvider.UserDao.GetUserInfo(userId);

                rows.Add(new List<string>
                {
                    userInfo.UserName,
                    userInfo.DisplayName,
                    userInfo.Email,
                    userInfo.Mobile,
                    UserGroupManager.GetGroupName(userInfo.GroupId),
                    DateUtils.GetDateAndTimeString(userInfo.CreateDate),
                    DateUtils.GetDateAndTimeString(userInfo.LastActivityDate)
                });
            }

            CsvUtils.Export(filePath, head, rows);
        }

        public static List<BackgroundContentInfo> GetContentsByCsvFile(string filePath, PublishmentSystemInfo publishmentSystemInfo,
            NodeInfo nodeInfo)
        {
            var contentInfoList = new List<BackgroundContentInfo>();

            List<string> head;
            List<List<string>> rows;
            CsvUtils.Import(filePath, out head, out rows);

            if (rows.Count > 0)
            {
                var relatedidentityes =
                    RelatedIdentities.GetChannelRelatedIdentities(
                        publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId);
                var modelInfo = ContentModelManager.GetContentModelInfo(publishmentSystemInfo,
                    nodeInfo.ContentModelId);
                var tableStyle = EAuxiliaryTableTypeUtils.GetTableStyle(modelInfo.TableType);
                // ArrayList tableStyleInfoArrayList = TableStyleManager.GetTableStyleInfoArrayList(ETableStyle.BackgroundContent, publishmentSystemInfo.AuxiliaryTableForContent, relatedidentityes);

                var tableStyleInfoList = TableStyleManager.GetTableStyleInfoList(tableStyle,
                    modelInfo.TableName, relatedidentityes);
                tableStyleInfoList = ContentUtility.GetAllTableStyleInfoList(publishmentSystemInfo,
                    tableStyle, tableStyleInfoList);
                var nameValueCollection = new NameValueCollection();

                foreach (var styleInfo in tableStyleInfoList)
                {
                    nameValueCollection[styleInfo.DisplayName] = styleInfo.AttributeName.ToLower();
                }

                var attributeNames = new List<string>();
                foreach (var columnName in head)
                {
                    if (!string.IsNullOrEmpty(nameValueCollection[columnName]))
                    {
                        attributeNames.Add(nameValueCollection[columnName]);
                    }
                    else
                    {
                        attributeNames.Add(columnName);
                    }
                }

                foreach (var row in rows)
                {
                    var contentInfo = new BackgroundContentInfo();
                    if (row.Count != attributeNames.Count) continue;

                    for (var i = 0; i < attributeNames.Count; i++)
                    {
                        var attributeName = attributeNames[i];
                        if (!string.IsNullOrEmpty(attributeName))
                        {
                            var value = row[i];
                            contentInfo.SetExtendedAttribute(attributeName, value);
                        }
                    }

                    if (!string.IsNullOrEmpty(contentInfo.Title))
                    {
                        contentInfo.PublishmentSystemId = publishmentSystemInfo.PublishmentSystemId;
                        contentInfo.NodeId = nodeInfo.NodeId;
                        contentInfo.LastEditDate = DateTime.Now;

                        contentInfoList.Add(contentInfo);
                    }
                }
            }

            return contentInfoList;
        }

        public static List<InputContentInfo> GetInputContentsByCsvFile(string filePath, PublishmentSystemInfo publishmentSystemInfo,
            InputInfo inputInfo)
        {
            var contentInfoList = new List<InputContentInfo>();

            List<string> head;
            List<List<string>> rows;
            CsvUtils.Import(filePath, out head, out rows);

            if (rows.Count > 0)
            {
                var relatedidentityes = RelatedIdentities.GetRelatedIdentities(ETableStyle.InputContent,
                    publishmentSystemInfo.PublishmentSystemId, inputInfo.InputId);
                var tableStyleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.InputContent,
                    DataProvider.InputContentDao.TableName, relatedidentityes);

                var nameValueCollection = new NameValueCollection();

                foreach (var styleInfo in tableStyleInfoList)
                {
                    nameValueCollection[styleInfo.DisplayName] = styleInfo.AttributeName.ToLower();
                }

                nameValueCollection["回复"] = InputContentAttribute.Reply.ToLower();
                nameValueCollection["添加时间"] = InputContentAttribute.AddDate.ToLower();

                var attributeNames = new List<string>();
                foreach (var columnName in head)
                {
                    if (!string.IsNullOrEmpty(nameValueCollection[columnName]))
                    {
                        attributeNames.Add(nameValueCollection[columnName]);
                    }
                    else
                    {
                        attributeNames.Add(columnName);
                    }
                }

                foreach (var row in rows)
                {
                    if (row.Count != attributeNames.Count) continue;

                    var contentInfo = new InputContentInfo(0, inputInfo.InputId, 0, true, string.Empty, string.Empty,
                        DateTime.Now, string.Empty);

                    for (var i = 0; i < attributeNames.Count; i++)
                    {
                        var attributeName = attributeNames[i];
                        if (!string.IsNullOrEmpty(attributeName))
                        {
                            var value = row[i];
                            contentInfo.SetExtendedAttribute(attributeName, value);
                        }
                    }

                    contentInfoList.Add(contentInfo);
                }
            }

            return contentInfoList;
        }
    }
}
