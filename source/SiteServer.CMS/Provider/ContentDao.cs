using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
    public class ContentDao : DataProviderBase
    {
        public int GetTaxisToInsert(string tableName, int nodeId, bool isTop)
        {
            int taxis;

            if (isTop)
            {
                taxis = BaiRongDataProvider.ContentDao.GetMaxTaxis(tableName, nodeId, true) + 1;
            }
            else
            {
                taxis = BaiRongDataProvider.ContentDao.GetMaxTaxis(tableName, nodeId, false) + 1;
            }

            return taxis;
        }

        public int Insert(string tableName, PublishmentSystemInfo publishmentSystemInfo, ContentInfo contentInfo)
        {
            var taxis = GetTaxisToInsert(tableName, contentInfo.NodeId, contentInfo.IsTop);
            return Insert(tableName, publishmentSystemInfo, contentInfo, true, taxis);
        }

        public int InsertPreview(string tableName, PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, ContentInfo contentInfo)
        {
            nodeInfo.Additional.IsPreviewContents = true;
            DataProvider.NodeDao.UpdateAdditional(nodeInfo);

            contentInfo.SourceId = SourceManager.Preview;
            return Insert(tableName, publishmentSystemInfo, contentInfo, false, 0);
        }

        public int Insert(string tableName, PublishmentSystemInfo publishmentSystemInfo, ContentInfo contentInfo, bool isUpdateContentNum, int taxis)
        {
            var contentId = 0;

            if (!string.IsNullOrEmpty(tableName))
            {
                if (publishmentSystemInfo.Additional.IsAutoPageInTextEditor && contentInfo.ContainsKey(BackgroundContentAttribute.Content))
                {
                    contentInfo.SetExtendedAttribute(BackgroundContentAttribute.Content, ContentUtility.GetAutoPageContent(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.Content), publishmentSystemInfo.Additional.AutoPageWordNum));
                }

                contentInfo.BeforeExecuteNonQuery();

                contentInfo.Taxis = taxis;

                contentId = BaiRongDataProvider.ContentDao.Insert(tableName, contentInfo);

                if (isUpdateContentNum)
                {
                    new Action(() =>
                    {
                        DataProvider.NodeDao.UpdateContentNum(PublishmentSystemManager.GetPublishmentSystemInfo(contentInfo.PublishmentSystemId), contentInfo.NodeId, true);
                    }).BeginInvoke(null, null);
                }
            }

            return contentId;
        }

        public void Update(string tableName, PublishmentSystemInfo publishmentSystemInfo, ContentInfo contentInfo)
        {
            if (publishmentSystemInfo.Additional.IsAutoPageInTextEditor && contentInfo.ContainsKey(BackgroundContentAttribute.Content))
            {
                contentInfo.SetExtendedAttribute(BackgroundContentAttribute.Content, ContentUtility.GetAutoPageContent(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.Content), publishmentSystemInfo.Additional.AutoPageWordNum));
            }

            BaiRongDataProvider.ContentDao.Update(tableName, contentInfo);
        }

        public void UpdateAutoPageContent(string tableName, PublishmentSystemInfo publishmentSystemInfo)
        {
            if (publishmentSystemInfo.Additional.IsAutoPageInTextEditor)
            {
                string sqlString =
                    $"SELECT ID, [{BackgroundContentAttribute.Content}] FROM [{tableName}] WHERE ([PublishmentSystemID] = {publishmentSystemInfo.PublishmentSystemId})";

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var contentId = GetInt(rdr, 0);
                        var content = GetString(rdr, 1);
                        if (!string.IsNullOrEmpty(content))
                        {
                            content = ContentUtility.GetAutoPageContent(content, publishmentSystemInfo.Additional.AutoPageWordNum);
                            string updateString =
                                $"UPDATE [{tableName}] SET [{BackgroundContentAttribute.Content}] = '{content}' WHERE ID = {contentId}";
                            try
                            {
                                ExecuteNonQuery(updateString);
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                    }

                    rdr.Close();
                }
            }
        }

        public ContentInfo GetContentInfoNotTrash(ETableStyle tableStyle, string tableName, int contentId)
        {
            ContentInfo info = null;
            if (contentId > 0)
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    string sqlWhere = $"WHERE NodeID > 0 AND ID = {contentId}";
                    var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, sqlWhere);

                    using (var rdr = ExecuteReader(sqlSelect))
                    {
                        if (rdr.Read())
                        {
                            info = ContentUtility.GetContentInfo(tableStyle);
                            BaiRongDataProvider.DatabaseDao.ReadResultsToExtendedAttributes(rdr, info);
                        }
                        rdr.Close();
                    }
                }
            }

            info?.AfterExecuteReader();
            return info;
        }

        public ContentInfo GetContentInfo(ETableStyle tableStyle, string tableName, int contentId)
        {
            ContentInfo info = null;
            if (contentId > 0)
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    string sqlWhere = $"WHERE ID = {contentId}";
                    var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, sqlWhere);

                    using (var rdr = ExecuteReader(sqlSelect))
                    {
                        if (rdr.Read())
                        {
                            info = ContentUtility.GetContentInfo(tableStyle);
                            BaiRongDataProvider.DatabaseDao.ReadResultsToExtendedAttributes(rdr, info);
                        }
                        rdr.Close();
                    }
                }
            }

            info?.AfterExecuteReader();
            return info;
        }

        public ContentInfo GetContentInfo(ETableStyle tableStyle, string sqlString)
        {
            ContentInfo info = null;
            if (!string.IsNullOrEmpty(sqlString))
            {
                using (var rdr = ExecuteReader(sqlString))
                {
                    if (rdr.Read())
                    {
                        info = ContentUtility.GetContentInfo(tableStyle);
                        BaiRongDataProvider.DatabaseDao.ReadResultsToExtendedAttributes(rdr, info);
                    }
                    rdr.Close();
                }
            }

            info?.AfterExecuteReader();
            return info;
        }

        public int GetCountOfContentAdd(string tableName, int publishmentSystemId, int nodeId, DateTime begin, DateTime end, string userName)
        {
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeId, EScopeType.All, string.Empty, string.Empty);
            return BaiRongDataProvider.ContentDao.GetCountOfContentAdd(tableName, publishmentSystemId, nodeIdList, begin, end, userName);
        }

        public int GetCountOfContentUpdate(string tableName, int publishmentSystemId, int nodeId, DateTime begin, DateTime end, string userName)
        {
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeId, EScopeType.All, string.Empty, string.Empty);
            return BaiRongDataProvider.ContentDao.GetCountOfContentUpdate(tableName, publishmentSystemId, nodeIdList, begin, end, userName);
        }

        public List<int> GetContentIdArrayListChecked(string tableName, int nodeId, bool isSystemAdministrator, List<int> owningNodeIdList, int totalNum, string orderByString, string whereString, EScopeType scopeType)
        {
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeId, scopeType, string.Empty, string.Empty);

            var theList = new List<int>();
            foreach (int theNodeId in nodeIdList)
            {
                if (isSystemAdministrator || owningNodeIdList.Contains(theNodeId))
                {
                    theList.Add(theNodeId);
                }
            }

            return BaiRongDataProvider.ContentDao.GetContentIdListChecked(tableName, theList, totalNum, orderByString, whereString);
        }

        public List<int> GetContentIdListChecked(string tableName, int nodeId, int totalNum, string orderByString, string whereString, EScopeType scopeType)
        {
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeId, scopeType, string.Empty, string.Empty);

            return BaiRongDataProvider.ContentDao.GetContentIdListChecked(tableName, nodeIdList, totalNum, orderByString, whereString);
        }

        public List<int> GetContentIdListChecked(string tableName, int nodeId, int totalNum, string orderByFormatString, string whereString)
        {
            return GetContentIdListChecked(tableName, nodeId, totalNum, orderByFormatString, whereString, EScopeType.Self);
        }

        public List<int> GetContentIdListChecked(string tableName, int nodeId, string orderByFormatString, string whereString)
        {
            return GetContentIdListChecked(tableName, nodeId, 0, orderByFormatString, whereString);
        }

        public List<int> GetContentIdListChecked(string tableName, int nodeId, int totalNum, string orderByFormatString)
        {
            return GetContentIdListChecked(tableName, nodeId, totalNum, orderByFormatString, string.Empty);
        }

        public List<int> GetContentIdListChecked(string tableName, int nodeId, string orderByFormatString)
        {
            return GetContentIdListChecked(tableName, nodeId, orderByFormatString, string.Empty);
        }

        public void TrashContents(int publishmentSystemId, string tableName, List<int> contentIdList, int nodeId)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                var referenceIdList = BaiRongDataProvider.ContentDao.GetReferenceIdList(tableName, contentIdList);
                if (referenceIdList.Count > 0)
                {
                    DeleteContents(publishmentSystemId, tableName, referenceIdList);
                }
                var updateNum = BaiRongDataProvider.ContentDao.TrashContents(publishmentSystemId, tableName, contentIdList);
                if (updateNum > 0)
                {
                    new Action(() =>
                    {
                        DataProvider.NodeDao.UpdateContentNum(PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId), nodeId, true);
                    }).BeginInvoke(null, null);
                }
            }
        }

        public void TrashContents(int publishmentSystemId, string tableName, List<int> contentIdArrayList)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                var referenceIdList = BaiRongDataProvider.ContentDao.GetReferenceIdList(tableName, contentIdArrayList);
                if (referenceIdList.Count > 0)
                {
                    DeleteContents(publishmentSystemId, tableName, referenceIdList);
                }
                var updateNum = BaiRongDataProvider.ContentDao.TrashContents(publishmentSystemId, tableName, contentIdArrayList);
                if (updateNum > 0)
                {
                    new Action(() =>
                    {
                        DataProvider.NodeDao.UpdateContentNum(PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId));
                    }).BeginInvoke(null, null);
                }
            }
        }

        public void TrashContentsByNodeId(int publishmentSystemId, string tableName, int nodeId)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                var contentIdList = BaiRongDataProvider.ContentDao.GetContentIdList(tableName, nodeId);
                var referenceIdList = BaiRongDataProvider.ContentDao.GetReferenceIdList(tableName, contentIdList);
                if (referenceIdList.Count > 0)
                {
                    DeleteContents(publishmentSystemId, tableName, referenceIdList);
                }
                var updateNum = BaiRongDataProvider.ContentDao.TrashContentsByNodeId(publishmentSystemId, tableName, nodeId);
                if (updateNum > 0)
                {
                    new Action(() =>
                    {
                        DataProvider.NodeDao.UpdateContentNum(PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId), nodeId, true);
                    }).BeginInvoke(null, null);
                }
            }
        }

        public void DeleteContents(int publishmentSystemId, string tableName, List<int> contentIdArrayList, int nodeId)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                var deleteNum = BaiRongDataProvider.ContentDao.DeleteContents(publishmentSystemId, tableName, contentIdArrayList);

                if (nodeId > 0 && deleteNum > 0)
                {
                    new Action(() =>
                    {
                        DataProvider.NodeDao.UpdateContentNum(PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId), nodeId, true);
                    }).BeginInvoke(null, null);
                }
            }
        }

        private void DeleteContents(int publishmentSystemId, string tableName, List<int> contentIdArrayList)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                var deleteNum = BaiRongDataProvider.ContentDao.DeleteContents(publishmentSystemId, tableName, contentIdArrayList);
                if (deleteNum > 0)
                {
                    new Action(() =>
                    {
                        DataProvider.NodeDao.UpdateContentNum(PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId));
                    }).BeginInvoke(null, null);
                }
            }
        }

        public void DeleteContentsByNodeId(int publishmentSystemId, string tableName, int nodeId)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                var contentIdArrayList = GetContentIdListChecked(tableName, nodeId, string.Empty);
                var deleteNum = BaiRongDataProvider.ContentDao.DeleteContentsByNodeId(publishmentSystemId, tableName, nodeId, contentIdArrayList);

                if (nodeId > 0 && deleteNum > 0)
                {
                    new Action(() =>
                    {
                        DataProvider.NodeDao.UpdateContentNum(PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId), nodeId, true);
                    }).BeginInvoke(null, null);
                }
            }
        }

        public void DeletePreviewContents(int publishmentSystemId, string tableName, NodeInfo nodeInfo)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                nodeInfo.Additional.IsPreviewContents = false;
                DataProvider.NodeDao.UpdateAdditional(nodeInfo);

                string sqlString =
                    $"DELETE FROM {tableName} WHERE PublishmentSystemID = {publishmentSystemId} AND NodeID = {nodeInfo.NodeId} AND SourceID = {SourceManager.Preview}";
                BaiRongDataProvider.DatabaseDao.ExecuteSql(sqlString);
            }
        }

        public void RestoreContentsByTrash(int publishmentSystemId, string tableName)
        {
            var updateNum = BaiRongDataProvider.ContentDao.RestoreContentsByTrash(publishmentSystemId, tableName);
            if (updateNum > 0)
            {
                new Action(() =>
                {
                    DataProvider.NodeDao.UpdateContentNum(PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId));
                }).BeginInvoke(null, null);
            }
        }

        public string GetWhereStringBySearchOutput(PublishmentSystemInfo publishmentSystemInfo, int nodeId, ETableStyle tableStyle, string word, StringCollection typeCollection, string channelId, string dateFrom, string dateTo, string date, string dateAttribute, string excludeAttributes, NameValueCollection form)
        {
            var whereBuilder = new StringBuilder();

            if (typeCollection.Count == 0)
            {
                typeCollection.Add(ContentAttribute.Title);
                if (tableStyle == ETableStyle.BackgroundContent)
                {
                    typeCollection.Add(BackgroundContentAttribute.Content);
                }
                else if (tableStyle == ETableStyle.JobContent)
                {
                    typeCollection.Add(JobContentAttribute.Location);
                    typeCollection.Add(JobContentAttribute.Department);
                    typeCollection.Add(JobContentAttribute.Requirement);
                    typeCollection.Add(JobContentAttribute.Responsibility);
                }
            }

            whereBuilder.Append($"(PublishmentSystemID = {publishmentSystemInfo.PublishmentSystemId}) ");
            if (!string.IsNullOrEmpty(word))
            {
                whereBuilder.Append(" AND (");
                foreach (var attributeName in typeCollection)
                {
                    whereBuilder.Append($"[{attributeName}] like '%{PageUtils.FilterSql(word)}%' OR ");
                }
                whereBuilder.Length = whereBuilder.Length - 3;
                whereBuilder.Append(")");
            }
            if (!string.IsNullOrEmpty(channelId))
            {
                var theChannelId = TranslateUtils.ToInt(channelId);
                if (theChannelId > 0 && theChannelId != publishmentSystemInfo.PublishmentSystemId)
                {
                    whereBuilder.Append(" AND ");
                    var nodeIdList = DataProvider.NodeDao.GetNodeIdListForDescendant(theChannelId);
                    nodeIdList.Add(theChannelId);
                    whereBuilder.Append(nodeIdList.Count == 1
                        ? $"(NodeID = {nodeIdList[0]}) "
                        : $"(NodeID IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)})) ");
                }
            }
            if (!string.IsNullOrEmpty(dateFrom))
            {
                whereBuilder.Append(" AND ");
                whereBuilder.Append($" {dateAttribute} >= '{dateFrom}' ");
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                whereBuilder.Append(" AND ");
                whereBuilder.Append($" {dateAttribute} <= '{dateTo}' ");
            }
            if (!string.IsNullOrEmpty(date))
            {
                var days = TranslateUtils.ToInt(date);
                if (days > 0)
                {
                    whereBuilder.Append(" AND ");
                    whereBuilder.Append(SqlUtils.GetDateDiffLessThanDays(dateAttribute, days.ToString()));
                }
            }

            var styleInfoList = RelatedIdentities.GetTableStyleInfoList(publishmentSystemInfo, tableStyle, nodeId);
            var tableName = RelatedIdentities.GetTableName(publishmentSystemInfo, tableStyle, nodeId);

            var arraylist = TranslateUtils.StringCollectionToStringList(excludeAttributes);
            foreach (string key in form.Keys)
            {
                if (arraylist.Contains(key.ToLower())) continue;
                if (!string.IsNullOrEmpty(form[key]))
                {
                    var value = StringUtils.Trim(form[key]);
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (TableManager.IsAttributeNameExists(tableStyle, tableName, key))
                        {
                            whereBuilder.Append(" AND ");
                            whereBuilder.Append($"([{key}] LIKE '%{value}%')");
                        }
                        else
                        {
                            foreach (var tableStyleInfo in styleInfoList)
                            {
                                if (StringUtils.EqualsIgnoreCase(tableStyleInfo.AttributeName, key))
                                {
                                    whereBuilder.Append(" AND ");
                                    whereBuilder.Append($"({ContentAttribute.SettingsXml} LIKE '%{key}={value}%')");
                                    break;
                                }
                            }
                        }

                        if (tableStyle == ETableStyle.GovPublicContent)
                        {
                            if (StringUtils.EqualsIgnoreCase(key, GovPublicContentAttribute.DepartmentId))
                            {
                                whereBuilder.Append(" AND ");
                                whereBuilder.Append($"([{key}] = {TranslateUtils.ToInt(value)})");
                            }
                            else if (StringUtils.EqualsIgnoreCase(key, GovPublicContentAttribute.Category1Id))
                            {
                                whereBuilder.Append(" AND ");
                                whereBuilder.Append($"([{key}] = {TranslateUtils.ToInt(value)})");
                            }
                            else if (StringUtils.EqualsIgnoreCase(key, GovPublicContentAttribute.Category2Id))
                            {
                                whereBuilder.Append(" AND ");
                                whereBuilder.Append($"([{key}] = {TranslateUtils.ToInt(value)})");
                            }
                            else if (StringUtils.EqualsIgnoreCase(key, GovPublicContentAttribute.Category3Id))
                            {
                                whereBuilder.Append(" AND ");
                                whereBuilder.Append($"([{key}] = {TranslateUtils.ToInt(value)})");
                            }
                            else if (StringUtils.EqualsIgnoreCase(key, GovPublicContentAttribute.Category4Id))
                            {
                                whereBuilder.Append(" AND ");
                                whereBuilder.Append($"([{key}] = {TranslateUtils.ToInt(value)})");
                            }
                            else if (StringUtils.EqualsIgnoreCase(key, GovPublicContentAttribute.Category5Id))
                            {
                                whereBuilder.Append(" AND ");
                                whereBuilder.Append($"([{key}] = {TranslateUtils.ToInt(value)})");
                            }
                            else if (StringUtils.EqualsIgnoreCase(key, GovPublicContentAttribute.Category6Id))
                            {
                                whereBuilder.Append(" AND ");
                                whereBuilder.Append($"([{key}] = {TranslateUtils.ToInt(value)})");
                            }
                        }
                    }
                }
            }
            return whereBuilder.ToString();
        }

        public string GetSelectCommend(ETableStyle tableStyle, string tableName, int publishmentSystemId, int nodeId, bool isSystemAdministrator, List<int> owningNodeIdList, string searchType, string keyword, string dateFrom, string dateTo, bool isSearchChildren, ETriState checkedState)
        {
            return GetSelectCommend(tableStyle, tableName, publishmentSystemId, nodeId, isSystemAdministrator, owningNodeIdList, searchType, keyword, dateFrom, dateTo, isSearchChildren, checkedState, false, false);
        }

        public string GetSelectCommend(ETableStyle tableStyle, string tableName, int publishmentSystemId, int nodeId, bool isSystemAdministrator, List<int> owningNodeIdList, string searchType, string keyword, string dateFrom, string dateTo, bool isSearchChildren, ETriState checkedState, bool isNoDup, bool isTrashContent)
        {
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeInfo,
                isSearchChildren ? EScopeType.All : EScopeType.Self, string.Empty, string.Empty, nodeInfo.ContentModelId);

            var list = new List<int>();
            if (isSystemAdministrator)
            {
                list = nodeIdList;
            }
            else
            {
                foreach (int theNodeId in nodeIdList)
                {
                    if (owningNodeIdList.Contains(theNodeId))
                    {
                        list.Add(theNodeId);
                    }
                }
            }

            return BaiRongDataProvider.ContentDao.GetSelectCommendByCondition(tableStyle, tableName, publishmentSystemId, list, searchType, keyword, dateFrom, dateTo, checkedState, isNoDup, isTrashContent);
        }

        public string GetSelectCommend(ETableStyle tableStyle, string tableName, int publishmentSystemId, int nodeId, bool isSystemAdministrator, List<int> owningNodeIdList, string searchType, string keyword, string dateFrom, string dateTo, bool isSearchChildren, ETriState checkedState, bool isNoDup, bool isTrashContent, bool isWritingOnly, string userNameOnly)
        {
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeInfo, isSearchChildren ? EScopeType.All : EScopeType.Self, string.Empty, string.Empty, nodeInfo.ContentModelId);

            var list = new List<int>();
            if (isSystemAdministrator)
            {
                list = nodeIdList;
            }
            else
            {
                foreach (int theNodeId in nodeIdList)
                {
                    if (owningNodeIdList.Contains(theNodeId))
                    {
                        list.Add(theNodeId);
                    }
                }
            }

            return BaiRongDataProvider.ContentDao.GetSelectCommendByCondition(tableStyle, tableName, publishmentSystemId, list, searchType, keyword, dateFrom, dateTo, checkedState, isNoDup, isTrashContent, isWritingOnly, userNameOnly);
        }

        public string GetWritingSelectCommend(string writingUserName, string tableName, int publishmentSystemId, List<int> nodeIdList, string searchType, string keyword, string dateFrom, string dateTo)
        {
            if (nodeIdList == null || nodeIdList.Count == 0)
            {
                return null;
            }

            var whereString = new StringBuilder($"WHERE WritingUserName = '{writingUserName}' ");

            if (nodeIdList.Count == 1)
            {
                whereString.AppendFormat("AND PublishmentSystemID = {0} AND NodeID = {1} ", publishmentSystemId, nodeIdList[0]);
            }
            else
            {
                whereString.AppendFormat("AND PublishmentSystemID = {0} AND NodeID IN ({1}) ", publishmentSystemId, TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList));
            }

            var dateString = string.Empty;
            if (!string.IsNullOrEmpty(dateFrom))
            {
                dateString = $" AND AddDate >= '{dateFrom}' ";
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                dateTo = DateUtils.GetDateString(TranslateUtils.ToDateTime(dateTo).AddDays(1));
                dateString += $" AND AddDate <= '{dateTo}' ";
            }

            if (string.IsNullOrEmpty(keyword))
            {
                whereString.Append(dateString);
            }
            else
            {
                var columnNameList = BaiRongDataProvider.TableStructureDao.GetColumnNameList(tableName);
                foreach (var columnName in columnNameList)
                {
                    if (StringUtils.EqualsIgnoreCase(columnName, searchType))
                    {
                        whereString.AppendFormat("AND ([{0}] LIKE '%{1}%') {2} ", searchType, keyword, dateString);
                        break;
                    }
                }
            }

            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString.ToString());
        }

        public string GetSelectCommendByContentGroup(string tableName, string contentGroupName, int publishmentSystemId)
        {
            contentGroupName = PageUtils.FilterSql(contentGroupName);
            string sqlString =
                $"SELECT * FROM {tableName} WHERE PublishmentSystemID={publishmentSystemId} AND NodeID>0 AND (ContentGroupNameCollection LIKE '{contentGroupName},%' OR ContentGroupNameCollection LIKE '%,{contentGroupName}' OR ContentGroupNameCollection  LIKE '%,{contentGroupName},%'  OR ContentGroupNameCollection='{contentGroupName}')";
            return sqlString;
        }

        public IEnumerable GetStlDataSourceChecked(string tableName, int nodeId, int startNum, int totalNum, string orderByString, string whereString, EScopeType scopeType, string groupChannel, string groupChannelNot, bool isNoDup, NameValueCollection otherAttributes)
        {
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeId, scopeType, groupChannel, groupChannelNot);
            return BaiRongDataProvider.ContentDao.GetStlDataSourceChecked(tableName, nodeIdList, startNum, totalNum, orderByString, whereString, isNoDup, otherAttributes);
        }

        public string GetStlSqlStringChecked(string tableName, int publishmentSystemId, int nodeId, int startNum, int totalNum, string orderByString, string whereString, EScopeType scopeType, string groupChannel, string groupChannelNot, bool isNoDup)
        {
            string sqlWhereString;

            if (publishmentSystemId == nodeId && scopeType == EScopeType.All && string.IsNullOrEmpty(groupChannel) && string.IsNullOrEmpty(groupChannelNot))
            {
                sqlWhereString =
                    $"WHERE (PublishmentSystemId = {publishmentSystemId} AND NodeID > 0 AND IsChecked = '{true}' {whereString})";
            }
            else
            {
                var nodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeId, scopeType, groupChannel, groupChannelNot);
                if (nodeIdList == null || nodeIdList.Count == 0)
                {
                    return string.Empty;
                }
                sqlWhereString = nodeIdList.Count == 1 ? $"WHERE (NodeID = {nodeIdList[0]} AND IsChecked = '{true}' {whereString})" : $"WHERE (NodeID IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) AND IsChecked = '{true}' {whereString})";
            }

            if (isNoDup)
            {
                var sqlString = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(tableName, "MIN(ID)", sqlWhereString + " GROUP BY Title");
                sqlWhereString += $" AND ID IN ({sqlString})";
            }

            if (!string.IsNullOrEmpty(tableName))
            {
                return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(tableName, startNum, totalNum, SqlUtils.Asterisk, sqlWhereString, orderByString);
            }
            return string.Empty;
        }

        public void TidyUp(string tableName, int nodeId, string attributeName, bool isDesc)
        {
            var taxisDirection = isDesc ? "ASC" : "DESC";//升序,但由于页面排序是按Taxis的Desc排序的，所以这里sql里面的ASC/DESC取反

            string sqlString =
                $"SELECT ID, IsTop FROM {tableName} WHERE NodeID = {nodeId} OR NodeID = -{nodeId} ORDER BY {attributeName} {taxisDirection}";
            var sqlList = new List<string>();

            using (var rdr = ExecuteReader(sqlString))
            {
                var taxis = 1;
                while (rdr.Read())
                {
                    var id = GetInt(rdr, 0);
                    var isTop = GetBool(rdr, 1);

                    sqlList.Add(
                        $"UPDATE {tableName} SET Taxis = {taxis++}, IsTop = '{isTop}' WHERE ID = {id}");
                }
                rdr.Close();
            }

            BaiRongDataProvider.DatabaseDao.ExecuteSql(sqlList);
        }

        public ContentInfo GetContentInfoByTitle(ETableStyle tableStyle, string tableName, string title)
        {
            ContentInfo info = null;

            if (!string.IsNullOrEmpty(tableName))
            {
                string sqlWhere = $"WHERE Title = {title}";
                var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, sqlWhere);

                using (var rdr = ExecuteReader(sqlSelect))
                {
                    if (rdr.Read())
                    {
                        info = ContentUtility.GetContentInfo(tableStyle);
                        BaiRongDataProvider.DatabaseDao.ReadResultsToExtendedAttributes(rdr, info);
                    }
                    rdr.Close();
                }
            }

            info?.AfterExecuteReader();
            return info;
        }

        public List<int> GetIdListBySameTitleInOneNode(string tableName, int nodeId, string title)
        {
            var list = new List<int>();
            string sql = $"SELECT ID FROM {tableName} WHERE NodeID = {nodeId} AND Title = '{title}'";
            using (var rdr = ExecuteReader(sql))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }
            return list;
        }
    }
}
