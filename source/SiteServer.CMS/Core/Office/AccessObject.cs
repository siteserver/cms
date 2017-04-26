using System.Collections;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using SiteServer.CMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Collections.Specialized;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.CMS.Core.Office
{
	public class AccessObject
	{
        public static bool CreateAccessFileForContents(string filePath, PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, List<int> contentIDArrayList, List<string> displayAttributes, bool isPeriods, string dateFrom, string dateTo, ETriState checkedState)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var sourceFilePath = SiteServerAssets.GetPath(SiteServerAssets.Default.AccessMdb);
            FileUtils.CopyFile(sourceFilePath, filePath);

            var relatedidentityes = RelatedIdentities.GetChannelRelatedIdentities(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId);

            var modelInfo = ContentModelManager.GetContentModelInfo(publishmentSystemInfo, nodeInfo.ContentModelId);
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
            var styleInfoList = TableStyleManager.GetTableStyleInfoList(tableStyle, modelInfo.TableName, relatedidentityes);
            styleInfoList = ContentUtility.GetAllTableStyleInfoList(publishmentSystemInfo, tableStyle, styleInfoList);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);

            var accessDAO = new AccessDao(filePath);

            var createTableSqlString = accessDAO.GetCreateTableSqlString(nodeInfo.NodeName, styleInfoList, displayAttributes);
            accessDAO.ExecuteSqlString(createTableSqlString);

            bool isExport;

            var insertSqlArrayList = accessDAO.GetInsertSqlStringArrayList(nodeInfo.NodeName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, tableStyle, tableName, styleInfoList, displayAttributes, contentIDArrayList, isPeriods, dateFrom, dateTo, checkedState, out isExport);

            foreach (string insertSql in insertSqlArrayList)
            {
                accessDAO.ExecuteSqlString(insertSql);
            }

            return isExport;
        }

        public static ArrayList GetContentsByAccessFile(string filePath, PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo)
        {
            var contentInfoArrayList = new ArrayList();

            var accessDao = new AccessDao(filePath);
            var tableNames = accessDao.GetTableNames();
            if (tableNames != null && tableNames.Length > 0)
            {
                foreach (var tableName in tableNames)
                {
                    string sqlString = $"SELECT * FROM [{tableName}]";
                    var dataset = accessDao.ReturnDataSet(sqlString);

                    var oleDt = dataset.Tables[0];

                    if (oleDt.Rows.Count > 0)
                    {
                        var relatedidentityes = RelatedIdentities.GetChannelRelatedIdentities(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId);

                        var modelInfo = ContentModelManager.GetContentModelInfo(publishmentSystemInfo, nodeInfo.ContentModelId);
                        var tableStyle = EAuxiliaryTableTypeUtils.GetTableStyle(modelInfo.TableType);

                        var tableStyleInfoList = TableStyleManager.GetTableStyleInfoList(tableStyle, modelInfo.TableName, relatedidentityes);

                        var nameValueCollection = new NameValueCollection();

                        foreach (var styleInfo in tableStyleInfoList)
                        {
                            nameValueCollection[styleInfo.DisplayName] = styleInfo.AttributeName.ToLower();
                        }

                        var attributeNames = new ArrayList();
                        for (var i = 0; i < oleDt.Columns.Count; i++)
                        {
                            var columnName = oleDt.Columns[i].ColumnName;
                            if (!string.IsNullOrEmpty(nameValueCollection[columnName]))
                            {
                                attributeNames.Add(nameValueCollection[columnName]);
                            }
                            else
                            {
                                attributeNames.Add(columnName);
                            }
                        }

                        foreach (DataRow row in oleDt.Rows)
                        {
                            var contentInfo = new BackgroundContentInfo();

                            for (var i = 0; i < oleDt.Columns.Count; i++)
                            {
                                var attributeName = attributeNames[i] as string;
                                if (!string.IsNullOrEmpty(attributeName))
                                {
                                    var value = row[i].ToString();
                                    contentInfo.SetExtendedAttribute(attributeName, value);
                                }
                            }

                            if (!string.IsNullOrEmpty(contentInfo.Title))
                            {
                                contentInfo.PublishmentSystemId = publishmentSystemInfo.PublishmentSystemId;
                                contentInfo.NodeId = nodeInfo.NodeId;
                                contentInfo.LastEditDate = DateTime.Now;

                                contentInfoArrayList.Add(contentInfo);
                            }
                        }
                    }
                }
            }

            return contentInfoArrayList;
        }
	}
}
