using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.CMS.Context.Atom.Atom.Core;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.ImportExport.Components
{
	internal class TableStyleIe
	{
		private readonly string _directoryPath;
	    private readonly string _adminName;

        public TableStyleIe(string directoryPath, string adminName)
		{
			_directoryPath = directoryPath;
		    _adminName = adminName;
		}

		public async Task ExportTableStylesAsync(int siteId, string tableName)
		{
            var allRelatedIdentities = await ChannelManager.GetChannelIdListAsync(siteId);
            allRelatedIdentities.Insert(0, 0);
            var tableStyleWithItemsDict = await TableStyleManager.GetTableStyleWithItemsDictionaryAsync(tableName, allRelatedIdentities);
		    if (tableStyleWithItemsDict == null || tableStyleWithItemsDict.Count <= 0) return;

		    var styleDirectoryPath = PathUtils.Combine(_directoryPath, tableName);
		    DirectoryUtils.CreateDirectoryIfNotExists(styleDirectoryPath);

		    foreach (var attributeName in tableStyleWithItemsDict.Keys)
		    {
		        var tableStyleWithItemList = tableStyleWithItemsDict[attributeName];
		        if (tableStyleWithItemList == null || tableStyleWithItemList.Count <= 0) continue;

		        var attributeNameDirectoryPath = PathUtils.Combine(styleDirectoryPath, attributeName);
		        DirectoryUtils.CreateDirectoryIfNotExists(attributeNameDirectoryPath);

		        foreach (var tableStyle in tableStyleWithItemList)
		        {
		            //仅导出当前系统内的表样式
		            if (tableStyle.RelatedIdentity != 0)
		            {
		                if (!await ChannelManager.IsAncestorOrSelfAsync(siteId, siteId, tableStyle.RelatedIdentity))
		                {
		                    continue;
		                }
		            }
		            var filePath = attributeNameDirectoryPath + PathUtils.SeparatorChar + tableStyle.Id + ".xml";
		            var feed = await ExportTableStyleAsync(tableStyle);
		            if (tableStyle.StyleItems != null && tableStyle.StyleItems.Count > 0)
		            {
		                foreach (var styleItemInfo in tableStyle.StyleItems)
		                {
		                    var entry = ExportTableStyleItemInfo(styleItemInfo);
		                    feed.Entries.Add(entry);
		                }
		            }
		            feed.Save(filePath);
		        }
		    }
		}

        private static async Task<AtomFeed> ExportTableStyleAsync(TableStyle tableStyle)
		{
			var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string>{ nameof(TableStyle.Id), "TableStyleID" }, tableStyle.Id.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.RelatedIdentity), tableStyle.RelatedIdentity.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.TableName), tableStyle.TableName);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.AttributeName), tableStyle.AttributeName);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.Taxis), tableStyle.Taxis.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.DisplayName), tableStyle.DisplayName);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.HelpText), tableStyle.HelpText);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.VisibleInList), tableStyle.VisibleInList.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.InputType), tableStyle.Type.Value);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.DefaultValue), tableStyle.DefaultValue);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.Horizontal), tableStyle.Horizontal.ToString());
            //SettingsXML
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.ExtendValues), tableStyle.ToString());

            //保存此栏目样式在系统中的排序号
            var orderString = string.Empty;
            if (tableStyle.RelatedIdentity != 0)
            {
                orderString = await DataProvider.ChannelDao.GetOrderStringInSiteAsync(tableStyle.RelatedIdentity);
            }

            AtomUtility.AddDcElement(feed.AdditionalElements, "OrderString", orderString);

			return feed;
		}

        private static AtomEntry ExportTableStyleItemInfo(TableStyleItem styleItem)
		{
			var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string>{ nameof(TableStyleItem.Id), "TableStyleItemID" }, styleItem.Id.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(TableStyleItem.TableStyleId), "TableStyleID" }, styleItem.TableStyleId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TableStyleItem.ItemTitle), styleItem.ItemTitle);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TableStyleItem.ItemValue), styleItem.ItemValue);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TableStyleItem.Selected), styleItem.Selected.ToString());

			return entry;
		}

        public static async Task SingleExportTableStylesAsync(string tableName, int siteId, int relatedIdentity, string styleDirectoryPath)
        {
            var channelInfo = await ChannelManager.GetChannelAsync(siteId, relatedIdentity);
            var relatedIdentities = TableStyleManager.GetRelatedIdentities(channelInfo);

            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(styleDirectoryPath);

            var styleInfoList = await TableStyleManager.GetStyleListAsync(tableName, relatedIdentities);
            foreach (var tableStyle in styleInfoList)
            {
                var filePath = PathUtils.Combine(styleDirectoryPath, tableStyle.AttributeName + ".xml");
                var feed = await ExportTableStyleAsync(tableStyle);
                var styleItems = tableStyle.StyleItems;
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

        public static async Task SingleExportTableStylesAsync(string tableName, string styleDirectoryPath)
        {
            var relatedIdentities = new List<int>{0};

            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(styleDirectoryPath);

            var styleInfoList = await TableStyleManager.GetStyleListAsync(tableName, relatedIdentities);
            foreach (var tableStyle in styleInfoList)
            {
                var filePath = PathUtils.Combine(styleDirectoryPath, tableStyle.AttributeName + ".xml");
                var feed = await ExportTableStyleAsync(tableStyle);
                var styleItems = tableStyle.StyleItems;
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

        public static async Task SingleImportTableStyleAsync(string tableName, string styleDirectoryPath, int relatedIdentity)
        {
            if (!DirectoryUtils.IsDirectoryExists(styleDirectoryPath)) return;

            var filePaths = DirectoryUtils.GetFilePaths(styleDirectoryPath);
            foreach (var filePath in filePaths)
            {
                var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

                var attributeName = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.AttributeName));
                var taxis = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.Taxis)));
                var displayName = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.DisplayName));
                var helpText = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.HelpText));
                var isVisibleInList = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.VisibleInList)));
                var inputType = InputTypeUtils.GetEnumType(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.InputType)));
                var defaultValue = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.DefaultValue));
                var isHorizontal = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.Horizontal)));
                //SettingsXML
                var extendValues = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.ExtendValues));

                var styleInfo = new TableStyle
                {
                    Id = 0,
                    RelatedIdentity = relatedIdentity,
                    TableName = tableName,
                    AttributeName = attributeName,
                    Taxis = taxis,
                    DisplayName = displayName,
                    HelpText = helpText,
                    VisibleInList = isVisibleInList,
                    Type = inputType,
                    DefaultValue = defaultValue,
                    Horizontal = isHorizontal,
                    ExtendValues = extendValues
                };

                var styleItems = new List<TableStyleItem>();
                foreach (AtomEntry entry in feed.Entries)
                {
                    var itemTitle = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TableStyleItem.ItemTitle));
                    var itemValue = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TableStyleItem.ItemValue));
                    var isSelected = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TableStyleItem.Selected)));

                    styleItems.Add(new TableStyleItem
                    {
                        Id = 0,
                        TableStyleId = 0,
                        ItemTitle = itemTitle,
                        ItemValue = itemValue,
                        Selected = isSelected
                    });
                }

                if (styleItems.Count > 0)
                {
                    styleInfo.StyleItems = styleItems;
                }

                if (await TableStyleManager.IsExistsAsync(relatedIdentity, tableName, attributeName))
                {
                    await DataProvider.TableStyleDao.DeleteAsync(relatedIdentity, tableName, attributeName);
                }
                await DataProvider.TableStyleDao.InsertAsync(styleInfo);
            }
        }

        public async Task ImportTableStylesAsync(int siteId)
		{
			if (!DirectoryUtils.IsDirectoryExists(_directoryPath)) return;

            var importObject = new ImportObject(siteId, _adminName);
            var tableNameCollection = await importObject.GetTableNameCacheAsync();

			var styleDirectoryPaths = DirectoryUtils.GetDirectoryPaths(_directoryPath);

            foreach (var styleDirectoryPath in styleDirectoryPaths)
            {
                var tableName = PathUtils.GetDirectoryName(styleDirectoryPath, false);
                if (tableName == "siteserver_PublishmentSystem")
                {
                    tableName = DataProvider.SiteDao.TableName;
                }
                if (!string.IsNullOrEmpty(tableNameCollection?[tableName]))
                {
                    tableName = tableNameCollection[tableName];
                }

                var attributeNamePaths = DirectoryUtils.GetDirectoryPaths(styleDirectoryPath);
                foreach (var attributeNamePath in attributeNamePaths)
                {
                    var attributeName = PathUtils.GetDirectoryName(attributeNamePath, false);
                    var filePaths = DirectoryUtils.GetFilePaths(attributeNamePath);
                    foreach (var filePath in filePaths)
                    {
                        var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

                        var taxis = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.Taxis)));
                        var displayName = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.DisplayName));
                        var helpText = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.HelpText));
                        var isVisibleInList = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.VisibleInList)));
                        var inputType = InputTypeUtils.GetEnumType(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.InputType)));
                        var defaultValue = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.DefaultValue));
                        var isHorizontal = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.Horizontal)));
                        var extendValues = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.ExtendValues));

                        var orderString = AtomUtility.GetDcElementContent(feed.AdditionalElements, "OrderString");

                        var relatedIdentity = !string.IsNullOrEmpty(orderString) ? await DataProvider.ChannelDao.GetIdAsync(siteId, orderString) : siteId;

                        if (relatedIdentity <= 0 || await TableStyleManager.IsExistsAsync(relatedIdentity, tableName, attributeName)) continue;

                        var styleInfo = new TableStyle
                        {
                            Id = 0,
                            RelatedIdentity = relatedIdentity,
                            TableName = tableName,
                            AttributeName = attributeName,
                            Taxis = taxis,
                            DisplayName = displayName,
                            HelpText = helpText,
                            VisibleInList = isVisibleInList,
                            Type = inputType,
                            DefaultValue = defaultValue,
                            Horizontal = isHorizontal,
                            ExtendValues = extendValues
                        };

                        var styleItems = new List<TableStyleItem>();
                        foreach (AtomEntry entry in feed.Entries)
                        {
                            var itemTitle = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TableStyleItem.ItemTitle));
                            var itemValue = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TableStyleItem.ItemValue));
                            var isSelected = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TableStyleItem.Selected)));

                            styleItems.Add(new TableStyleItem
                            {
                                Id = 0,
                                TableStyleId = 0,
                                ItemTitle = itemTitle,
                                ItemValue = itemValue,
                                Selected = isSelected
                            });
                        }

                        if (styleItems.Count > 0)
                        {
                            styleInfo.StyleItems = styleItems;
                        }

                        await DataProvider.TableStyleDao.InsertAsync(styleInfo);
                    }
                }
            }
		}

	}
}
