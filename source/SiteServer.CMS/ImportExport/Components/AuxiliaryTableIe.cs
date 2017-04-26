using System.Collections.Specialized;
using Atom.Core;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.ImportExport.Components
{
	internal class AuxiliaryTableIe
	{
		private readonly string _directoryPath;

		public AuxiliaryTableIe(string directoryPath)
		{
			_directoryPath = directoryPath;
		}

		public void ExportAuxiliaryTable(string tableName)
		{
            var tableInfo = BaiRongDataProvider.TableCollectionDao.GetAuxiliaryTableInfo(tableName);
			if (tableInfo != null)
			{
                var metaInfoList = TableManager.GetTableMetadataInfoList(tableInfo.TableEnName);
				var filePath = _directoryPath + PathUtils.SeparatorChar + tableInfo.TableEnName + ".xml";

				var feed = GetAtomFeed(tableInfo);

				foreach (var metaInfo in metaInfoList)
				{
					var entry = GetAtomEntry(metaInfo);
					feed.Entries.Add(entry);
				}
				feed.Save(filePath);
			}
		}

		private static AtomFeed GetAtomFeed(AuxiliaryTableInfo tableInfo)
		{
			var feed = AtomUtility.GetEmptyFeed();

			AtomUtility.AddDcElement(feed.AdditionalElements, "TableENName", tableInfo.TableEnName);
			AtomUtility.AddDcElement(feed.AdditionalElements, "TableCNName", tableInfo.TableCnName);
			AtomUtility.AddDcElement(feed.AdditionalElements, "AttributeNum", tableInfo.AttributeNum.ToString());
			AtomUtility.AddDcElement(feed.AdditionalElements, "AuxiliaryTableType", EAuxiliaryTableTypeUtils.GetValue(tableInfo.AuxiliaryTableType));
            AtomUtility.AddDcElement(feed.AdditionalElements, "IsCreatedInDB", tableInfo.IsCreatedInDb.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, "IsChangedAfterCreatedInDB", tableInfo.IsChangedAfterCreatedInDb.ToString());
			AtomUtility.AddDcElement(feed.AdditionalElements, "Description", tableInfo.Description);
            //表唯一序列号
            AtomUtility.AddDcElement(feed.AdditionalElements, "SerializedString", TableManager.GetSerializedString(tableInfo.TableEnName));

			return feed;
		}

		private static AtomEntry GetAtomEntry(TableMetadataInfo metaInfo)
		{
			var entry = AtomUtility.GetEmptyEntry();

			AtomUtility.AddDcElement(entry.AdditionalElements, "TableMetadataID", metaInfo.TableMetadataId.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "AuxiliaryTableENName", metaInfo.AuxiliaryTableEnName);
			AtomUtility.AddDcElement(entry.AdditionalElements, "AttributeName", metaInfo.AttributeName);
			AtomUtility.AddDcElement(entry.AdditionalElements, "DataType", EDataTypeUtils.GetValue(metaInfo.DataType));
			AtomUtility.AddDcElement(entry.AdditionalElements, "DataLength", metaInfo.DataLength.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "Taxis", metaInfo.Taxis.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "IsSystem", metaInfo.IsSystem.ToString());

			return entry;
		}


        /// <summary>
        /// 将频道模板中的辅助表导入发布系统中，返回修改了的表名对照
        /// 在导入辅助表的同时检查发布系统辅助表并替换对应表
        /// </summary>
        public NameValueCollection ImportAuxiliaryTables(int publishmentSystemId, bool isUserTables)
		{
			if (!DirectoryUtils.IsDirectoryExists(_directoryPath)) return null;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

            var nameValueCollection = new NameValueCollection();
            var tableNamePrefix = publishmentSystemInfo.PublishmentSystemDir + "_";

			var filePaths = DirectoryUtils.GetFilePaths(_directoryPath);

            foreach (var filePath in filePaths)
            {
                var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

                var tableName = AtomUtility.GetDcElementContent(feed.AdditionalElements, "TableENName");
                var tableType = EAuxiliaryTableTypeUtils.GetEnumType(AtomUtility.GetDcElementContent(feed.AdditionalElements, "AuxiliaryTableType"));

                if (!isUserTables)
                {
                    nameValueCollection[tableName] = NodeManager.GetTableName(publishmentSystemInfo, tableType);
                    continue;
                }

                var tableCnName = AtomUtility.GetDcElementContent(feed.AdditionalElements, "TableCNName");
                var serializedString = AtomUtility.GetDcElementContent(feed.AdditionalElements, "SerializedString");

                var tableNameToInsert = string.Empty;//需要增加的表名，空代表不需要添加辅助表

                var tableInfo = BaiRongDataProvider.TableCollectionDao.GetAuxiliaryTableInfo(tableName);
                if (tableInfo == null)//如果当前系统无此表名
                {
                    tableNameToInsert = tableName;
                }
                else
                {
                    var serializedStringForExistTable = TableManager.GetSerializedString(tableName);

                    if (!string.IsNullOrEmpty(serializedString))
                    {
                        if (serializedString != serializedStringForExistTable)//仅有此时，导入表需要修改表名
                        {
                            tableNameToInsert = tableNamePrefix + tableName;
                            tableCnName = tableNamePrefix + tableCnName;
                            nameValueCollection[tableName] = tableNameToInsert;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(tableNameToInsert))//需要添加
                {
                    if (!BaiRongDataProvider.TableStructureDao.IsTableExists(tableNameToInsert))
                    {
                        tableInfo = new AuxiliaryTableInfo
                        {
                            TableEnName = tableNameToInsert,
                            TableCnName = tableCnName,
                            AttributeNum = 0,
                            AuxiliaryTableType = tableType,
                            IsCreatedInDb = false,
                            IsChangedAfterCreatedInDb = false,
                            Description = AtomUtility.GetDcElementContent(feed.AdditionalElements, "Description")
                        };

                        BaiRongDataProvider.TableCollectionDao.Insert(tableInfo);

                        var tableStyle = EAuxiliaryTableTypeUtils.GetTableStyle(tableInfo.AuxiliaryTableType);

                        var attributeNameList =
                            TableManager.GetAttributeNameList(tableStyle, tableInfo.TableEnName, true);
                        attributeNameList.AddRange(
                            TableManager.GetHiddenAttributeNameList(tableStyle));

                        foreach (AtomEntry entry in feed.Entries)
                        {
                            var metaInfo = new TableMetadataInfo
                            {
                                AuxiliaryTableEnName = tableNameToInsert,
                                AttributeName =
                                    AtomUtility.GetDcElementContent(entry.AdditionalElements, "AttributeName"),
                                DataType = EDataTypeUtils.GetEnumType(
                                    AtomUtility.GetDcElementContent(entry.AdditionalElements, "DataType")),
                                DataLength = TranslateUtils.ToInt(
                                    AtomUtility.GetDcElementContent(entry.AdditionalElements, "DataLength")),
                                Taxis =
                                    TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements,
                                        "Taxis")),
                                IsSystem = TranslateUtils.ToBool(
                                    AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsSystem"))
                            };

                            if (attributeNameList.IndexOf(metaInfo.AttributeName.Trim().ToLower()) != -1) continue;

                            if (metaInfo.IsSystem) continue;

                            BaiRongDataProvider.TableMetadataDao.Insert(metaInfo);
                        }

                        BaiRongDataProvider.TableMetadataDao.CreateAuxiliaryTable(tableNameToInsert);
                    }
                }

                var tableNameToChange = (!string.IsNullOrEmpty(tableNameToInsert)) ? tableNameToInsert : tableName;
                //更新发布系统后台内容表及栏目表
                if (tableType == EAuxiliaryTableType.BackgroundContent)
                {
                    publishmentSystemInfo.AuxiliaryTableForContent = tableNameToChange;
                    DataProvider.PublishmentSystemDao.Update(publishmentSystemInfo);
                }
                else if (tableType == EAuxiliaryTableType.GovPublicContent)
                {
                    publishmentSystemInfo.AuxiliaryTableForGovPublic = tableNameToChange;
                    DataProvider.PublishmentSystemDao.Update(publishmentSystemInfo);
                }
                else if (tableType == EAuxiliaryTableType.GovInteractContent)
                {
                    publishmentSystemInfo.AuxiliaryTableForGovInteract = tableNameToChange;
                    DataProvider.PublishmentSystemDao.Update(publishmentSystemInfo);
                }
                else if (tableType == EAuxiliaryTableType.JobContent)
                {
                    publishmentSystemInfo.AuxiliaryTableForJob = tableNameToChange;
                    DataProvider.PublishmentSystemDao.Update(publishmentSystemInfo);
                }
                else if (tableType == EAuxiliaryTableType.VoteContent)
                {
                    publishmentSystemInfo.AuxiliaryTableForVote = tableNameToChange;
                    DataProvider.PublishmentSystemDao.Update(publishmentSystemInfo);
                }
            }

            return nameValueCollection;
		}

	}
}
