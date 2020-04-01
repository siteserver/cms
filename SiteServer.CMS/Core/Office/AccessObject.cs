using SiteServer.Utils;
using SiteServer.CMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Collections.Specialized;
using SiteServer.CMS.DataCache;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Core.Office
{
	public static class AccessObject
	{
        public static bool CreateAccessFileForContents(string filePath, SiteInfo siteInfo, ChannelInfo nodeInfo, List<int> contentIdList, List<string> displayAttributes, bool isPeriods, string dateFrom, string dateTo, ETriState checkedState)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var sourceFilePath = SiteServerAssets.GetPath(SiteServerAssets.Default.AccessMdb);
            FileUtils.CopyFile(sourceFilePath, filePath);

            var tableName = ChannelManager.GetTableName(siteInfo, nodeInfo);
            var styleInfoList = TableStyleManager.GetContentStyleInfoList(siteInfo, nodeInfo);
            styleInfoList = ContentUtility.GetAllTableStyleInfoList(styleInfoList);

            var accessDao = new AccessDao(filePath);

            var createTableSqlString = accessDao.GetCreateTableSqlString(nodeInfo.ChannelName, styleInfoList, displayAttributes);
            accessDao.ExecuteSqlString(createTableSqlString);

            bool isExport;

            var insertSqlList = accessDao.GetInsertSqlStringList(nodeInfo.ChannelName, siteInfo.Id, nodeInfo.Id, tableName, styleInfoList, displayAttributes, contentIdList, isPeriods, dateFrom, dateTo, checkedState, out isExport);

            foreach (var insertSql in insertSqlList)
            {
                accessDao.ExecuteSqlString(insertSql);
            }

            return isExport;
        }

        public static List<ContentInfo> GetContentsByAccessFile(string filePath, SiteInfo siteInfo, ChannelInfo nodeInfo)
        {
            var contentInfoList = new List<ContentInfo>();

            var accessDao = new AccessDao(filePath);
            var tableNames = accessDao.GetTableNames();
            if (tableNames != null && tableNames.Length > 0)
            {
                foreach (var tableName in tableNames)
                {
                    var sqlString = $"SELECT * FROM [{tableName}]";
                    var dataset = accessDao.ReturnDataSet(sqlString);

                    var oleDt = dataset.Tables[0];

                    if (oleDt.Rows.Count > 0)
                    {
                        var tableStyleInfoList = TableStyleManager.GetContentStyleInfoList(siteInfo, nodeInfo);

                        var nameValueCollection = new NameValueCollection();

                        foreach (var styleInfo in tableStyleInfoList)
                        {
                            nameValueCollection[styleInfo.DisplayName] = styleInfo.AttributeName.ToLower();
                        }

                        //var attributeNames = new ArrayList();
                        //for (var i = 0; i < oleDt.Columns.Count; i++)
                        //{
                        //    var columnName = oleDt.Columns[i].ColumnName;
                        //    attributeNames.Add(!string.IsNullOrEmpty(nameValueCollection[columnName])
                        //        ? nameValueCollection[columnName]
                        //        : columnName);
                        //}

                        foreach (DataRow row in oleDt.Rows)
                        {
                            var contentInfo = new ContentInfo(row);

                            if (!string.IsNullOrEmpty(contentInfo.Title))
                            {
                                contentInfo.SiteId = siteInfo.Id;
                                contentInfo.ChannelId = nodeInfo.Id;
                                contentInfo.LastEditDate = DateTime.Now;

                                contentInfoList.Add(contentInfo);
                            }
                        }
                    }
                }
            }

            return contentInfoList;
        }
	}
}
