using System.Collections.Generic;
using Atom.Core;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.ImportExport.Components
{
	internal class TableStyleIe
	{
		private readonly string _directoryPath;

        public TableStyleIe(string directoryPath)
		{
			_directoryPath = directoryPath;
		}

		public void ExportTableStyles(int publishmentSystemId, string tableName)
		{
            var allRelatedIdentities = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(publishmentSystemId);
            allRelatedIdentities.Insert(0, 0);
            var tableStyleInfoWithItemsDict = TableStyleManager.GetTableStyleInfoWithItemsDictinary(tableName, allRelatedIdentities);
		    if (tableStyleInfoWithItemsDict == null || tableStyleInfoWithItemsDict.Count <= 0) return;

		    var styleDirectoryPath = PathUtils.Combine(_directoryPath, tableName);
		    DirectoryUtils.CreateDirectoryIfNotExists(styleDirectoryPath);

		    foreach (var attributeName in tableStyleInfoWithItemsDict.Keys)
		    {
		        var tableStyleInfoWithItemList = tableStyleInfoWithItemsDict[attributeName];
		        if (tableStyleInfoWithItemList == null || tableStyleInfoWithItemList.Count <= 0) continue;

		        var attributeNameDirectoryPath = PathUtils.Combine(styleDirectoryPath, attributeName);
		        DirectoryUtils.CreateDirectoryIfNotExists(attributeNameDirectoryPath);

		        foreach (var tableStyleInfo in tableStyleInfoWithItemList)
		        {
		            //仅导出当前系统内的表样式
		            if (tableStyleInfo.RelatedIdentity != 0)
		            {
		                if (!NodeManager.IsAncestorOrSelf(publishmentSystemId, publishmentSystemId, tableStyleInfo.RelatedIdentity))
		                {
		                    continue;
		                }
		            }
		            var filePath = attributeNameDirectoryPath + PathUtils.SeparatorChar + tableStyleInfo.TableStyleId + ".xml";
		            var feed = ExportTableStyleInfo(tableStyleInfo);
		            if (tableStyleInfo.StyleItems != null && tableStyleInfo.StyleItems.Count > 0)
		            {
		                foreach (var styleItemInfo in tableStyleInfo.StyleItems)
		                {
		                    var entry = ExportTableStyleItemInfo(styleItemInfo);
		                    feed.Entries.Add(entry);
		                }
		            }
		            feed.Save(filePath);
		        }
		    }
		}

        private static AtomFeed ExportTableStyleInfo(TableStyleInfo tableStyleInfo)
		{
			var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, "TableStyleID", tableStyleInfo.TableStyleId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, "RelatedIdentity", tableStyleInfo.RelatedIdentity.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, "TableName", tableStyleInfo.TableName);
            AtomUtility.AddDcElement(feed.AdditionalElements, "AttributeName", tableStyleInfo.AttributeName);
            AtomUtility.AddDcElement(feed.AdditionalElements, "Taxis", tableStyleInfo.Taxis.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, "DisplayName", tableStyleInfo.DisplayName);
            AtomUtility.AddDcElement(feed.AdditionalElements, "HelpText", tableStyleInfo.HelpText);
            AtomUtility.AddDcElement(feed.AdditionalElements, "IsVisible", tableStyleInfo.IsVisible.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, "IsVisibleInList", tableStyleInfo.IsVisibleInList.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, "IsSingleLine", tableStyleInfo.IsSingleLine.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, "InputType", tableStyleInfo.InputType);
            AtomUtility.AddDcElement(feed.AdditionalElements, "DefaultValue", tableStyleInfo.DefaultValue);
            AtomUtility.AddDcElement(feed.AdditionalElements, "IsHorizontal", tableStyleInfo.IsHorizontal.ToString());
            //SettingsXML
            AtomUtility.AddDcElement(feed.AdditionalElements, "ExtendValues", tableStyleInfo.ExtendValues);

            //保存此栏目样式在系统中的排序号
            var orderString = string.Empty;
            if (tableStyleInfo.RelatedIdentity != 0)
            {
                orderString = DataProvider.NodeDao.GetOrderStringInPublishmentSystem(tableStyleInfo.RelatedIdentity);
            }

            AtomUtility.AddDcElement(feed.AdditionalElements, "OrderString", orderString);

			return feed;
		}

        private static AtomEntry ExportTableStyleItemInfo(TableStyleItemInfo styleItemInfo)
		{
			var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, "TableStyleItemID", styleItemInfo.TableStyleItemId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "TableStyleID", styleItemInfo.TableStyleId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "ItemTitle", styleItemInfo.ItemTitle);
            AtomUtility.AddDcElement(entry.AdditionalElements, "ItemValue", styleItemInfo.ItemValue);
            AtomUtility.AddDcElement(entry.AdditionalElements, "IsSelected", styleItemInfo.IsSelected.ToString());

			return entry;
		}

        public static void SingleExportTableStyles(ETableStyle tableStyle, string tableName, int publishmentSystemId, int relatedIdentity, string styleDirectoryPath)
        {
            var relatedIdentities = RelatedIdentities.GetRelatedIdentities(tableStyle, publishmentSystemId, relatedIdentity);

            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(styleDirectoryPath);

            var styleInfoList = TableStyleManager.GetTableStyleInfoList(tableStyle, tableName, relatedIdentities);
            foreach (var tableStyleInfo in styleInfoList)
            {
                var filePath = PathUtils.Combine(styleDirectoryPath, tableStyleInfo.AttributeName + ".xml");
                var feed = ExportTableStyleInfo(tableStyleInfo);
                var styleItems = BaiRongDataProvider.TableStyleDao.GetStyleItemInfoList(tableStyleInfo.TableStyleId);
                if (styleItems != null && styleItems.Count > 0)
                {
                    foreach (var styleItemInfo in styleItems)
                    {
                        var entry = ExportTableStyleItemInfo(styleItemInfo);
                        feed.Entries.Add(entry);
                    }
                }
                feed.Save(filePath);
            }
        }

        public static void SingleExportTableStyles(ETableStyle tableStyle, string tableName, string styleDirectoryPath)
        {
            var relatedIdentities = new List<int>{0};

            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(styleDirectoryPath);

            var styleInfoList = TableStyleManager.GetTableStyleInfoList(tableStyle, tableName, relatedIdentities);
            foreach (var tableStyleInfo in styleInfoList)
            {
                var filePath = PathUtils.Combine(styleDirectoryPath, tableStyleInfo.AttributeName + ".xml");
                var feed = ExportTableStyleInfo(tableStyleInfo);
                var styleItems = BaiRongDataProvider.TableStyleDao.GetStyleItemInfoList(tableStyleInfo.TableStyleId);
                if (styleItems != null && styleItems.Count > 0)
                {
                    foreach (var styleItemInfo in styleItems)
                    {
                        var entry = ExportTableStyleItemInfo(styleItemInfo);
                        feed.Entries.Add(entry);
                    }
                }
                feed.Save(filePath);
            }
        }

        public static void SingleImportTableStyle(ETableStyle tableStyle, string tableName, string styleDirectoryPath, int relatedIdentity)
        {
            if (!DirectoryUtils.IsDirectoryExists(styleDirectoryPath)) return;

            var filePaths = DirectoryUtils.GetFilePaths(styleDirectoryPath);
            foreach (var filePath in filePaths)
            {
                var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

                var attributeName = AtomUtility.GetDcElementContent(feed.AdditionalElements, "AttributeName");
                var taxis = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(feed.AdditionalElements, "Taxis"), 0);
                var displayName = AtomUtility.GetDcElementContent(feed.AdditionalElements, "DisplayName");
                var helpText = AtomUtility.GetDcElementContent(feed.AdditionalElements, "HelpText");
                var isVisible = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, "IsVisible"));
                var isVisibleInList = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, "IsVisibleInList"));
                var isSingleLine = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, "IsSingleLine"));
                var inputType = EInputTypeUtils.GetEnumType(AtomUtility.GetDcElementContent(feed.AdditionalElements, "InputType"));
                var defaultValue = AtomUtility.GetDcElementContent(feed.AdditionalElements, "DefaultValue");
                var isHorizontal = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, "IsHorizontal"));
                //SettingsXML
                var extendValues = AtomUtility.GetDcElementContent(feed.AdditionalElements, "ExtendValues");

                var styleInfo = new TableStyleInfo(0, relatedIdentity, tableName, attributeName, taxis, displayName, helpText, isVisible, isVisibleInList, isSingleLine, EInputTypeUtils.GetValue(inputType), defaultValue, isHorizontal, extendValues);

                var styleItems = new List<TableStyleItemInfo>();
                foreach (AtomEntry entry in feed.Entries)
                {
                    var itemTitle = AtomUtility.GetDcElementContent(entry.AdditionalElements, "ItemTitle");
                    var itemValue = AtomUtility.GetDcElementContent(entry.AdditionalElements, "ItemValue");
                    var isSelected = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsSelected"));

                    styleItems.Add(new TableStyleItemInfo(0, 0, itemTitle, itemValue, isSelected));
                }

                if (styleItems.Count > 0)
                {
                    styleInfo.StyleItems = styleItems;
                }

                if (TableStyleManager.IsExists(relatedIdentity, tableName, attributeName))
                {
                    TableStyleManager.Delete(relatedIdentity, tableName, attributeName);
                }
                TableStyleManager.InsertWithTaxis(styleInfo, tableStyle);
            }
        }

        public void ImportTableStyles(int publishmentSystemId)
		{
			if (!DirectoryUtils.IsDirectoryExists(_directoryPath)) return;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

            var importObject = new ImportObject(publishmentSystemId);
            var tableNameCollection = importObject.GetTableNameCache();

			var styleDirectoryPaths = DirectoryUtils.GetDirectoryPaths(_directoryPath);

            foreach (var styleDirectoryPath in styleDirectoryPaths)
            {
                var tableName = PathUtils.GetDirectoryName(styleDirectoryPath);
                if (!string.IsNullOrEmpty(tableNameCollection?[tableName]))
                {
                    tableName = tableNameCollection[tableName];
                }

                ETableStyle tableStyle;

                if (BaiRongDataProvider.TableCollectionDao.IsTableExists(tableName))
                {
                    var tableType = BaiRongDataProvider.TableCollectionDao.GetTableType(tableName);
                    tableStyle = EAuxiliaryTableTypeUtils.GetTableStyle(tableType);
                }
                else
                {
                    tableStyle = PublishmentSystemManager.GetTableStyle(publishmentSystemInfo, tableName);
                }

                var attributeNamePaths = DirectoryUtils.GetDirectoryPaths(styleDirectoryPath);
                foreach (var attributeNamePath in attributeNamePaths)
                {
                    var attributeName = PathUtils.GetDirectoryName(attributeNamePath);
                    var filePaths = DirectoryUtils.GetFilePaths(attributeNamePath);
                    foreach (var filePath in filePaths)
                    {
                        var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

                        var taxis = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(feed.AdditionalElements, "Taxis"), 0);
                        var displayName = AtomUtility.GetDcElementContent(feed.AdditionalElements, "DisplayName");
                        var helpText = AtomUtility.GetDcElementContent(feed.AdditionalElements, "HelpText");
                        var isVisible = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, "IsVisible"));
                        var isVisibleInList = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, "IsVisibleInList"));
                        var isSingleLine = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, "IsSingleLine"));
                        var inputType = EInputTypeUtils.GetEnumType(AtomUtility.GetDcElementContent(feed.AdditionalElements, "InputType"));
                        var defaultValue = AtomUtility.GetDcElementContent(feed.AdditionalElements, "DefaultValue");
                        var isHorizontal = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, "IsHorizontal"));
                        var extendValues = AtomUtility.GetDcElementContent(feed.AdditionalElements, "ExtendValues");

                        int relatedIdentity;
                        if (tableStyle == ETableStyle.Site)
                        {
                            relatedIdentity = publishmentSystemId;
                        }
                        else
                        {
                            var orderString = AtomUtility.GetDcElementContent(feed.AdditionalElements, "OrderString");

                            if (!string.IsNullOrEmpty(orderString))
                            {
                                relatedIdentity = DataProvider.NodeDao.GetNodeId(publishmentSystemId, orderString);
                            }
                            else
                            {
                                relatedIdentity = publishmentSystemId;
                            }
                        }

                        if (relatedIdentity <= 0 ||
                            TableStyleManager.IsExists(relatedIdentity, tableName, attributeName)) continue;

                        var styleInfo = new TableStyleInfo(0, relatedIdentity, tableName, attributeName, taxis, displayName, helpText, isVisible, isVisibleInList, isSingleLine, EInputTypeUtils.GetValue(inputType), defaultValue, isHorizontal, extendValues);

                        var styleItems = new List<TableStyleItemInfo>();
                        foreach (AtomEntry entry in feed.Entries)
                        {
                            var itemTitle = AtomUtility.GetDcElementContent(entry.AdditionalElements, "ItemTitle");
                            var itemValue = AtomUtility.GetDcElementContent(entry.AdditionalElements, "ItemValue");
                            var isSelected = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsSelected"));

                            styleItems.Add(new TableStyleItemInfo(0, 0, itemTitle, itemValue, isSelected));
                        }

                        if (styleItems.Count > 0)
                        {
                            styleInfo.StyleItems = styleItems;
                        }

                        TableStyleManager.InsertWithTaxis(styleInfo, tableStyle);
                    }
                }
            }
		}

	}
}
