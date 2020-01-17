using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Context.Atom.Atom.Core;
using SiteServer.Abstractions;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;

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
            var tableStyleWithItemsDict = await DataProvider.TableStyleRepository.GetTableStyleWithItemsDictionaryAsync(tableName, allRelatedIdentities);
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
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.InputType), tableStyle.InputType.GetValue());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.DefaultValue), tableStyle.DefaultValue);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.Horizontal), tableStyle.Horizontal.ToString());
            //SettingsXML
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.ExtendValues), tableStyle.ToString());

            //保存此栏目样式在系统中的排序号
            var orderString = string.Empty;
            if (tableStyle.RelatedIdentity != 0)
            {
                orderString = await DataProvider.ChannelRepository.GetOrderStringInSiteAsync(tableStyle.RelatedIdentity);
            }

            AtomUtility.AddDcElement(feed.AdditionalElements, "OrderString", orderString);

			return feed;
		}

        public static async Task SingleExportTableStylesAsync(string tableName, int siteId, int relatedIdentity, string styleDirectoryPath)
        {
            var channelInfo = await ChannelManager.GetChannelAsync(siteId, relatedIdentity);
            var relatedIdentities = DataProvider.TableStyleRepository.GetRelatedIdentities(channelInfo);

            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(styleDirectoryPath);

            var styleInfoList = await DataProvider.TableStyleRepository.GetStyleListAsync(tableName, relatedIdentities);
            foreach (var tableStyle in styleInfoList)
            {
                var filePath = PathUtils.Combine(styleDirectoryPath, tableStyle.AttributeName + ".xml");
                var feed = await ExportTableStyleAsync(tableStyle);
                feed.Save(filePath);
            }
        }

        public static async Task SingleExportTableStylesAsync(string tableName, List<int> relatedIdentities, string styleDirectoryPath)
        {
            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(styleDirectoryPath);

            var styleInfoList = await DataProvider.TableStyleRepository.GetStyleListAsync(tableName, relatedIdentities);
            foreach (var tableStyle in styleInfoList)
            {
                var filePath = PathUtils.Combine(styleDirectoryPath, tableStyle.AttributeName + ".xml");
                var feed = await ExportTableStyleAsync(tableStyle);
                feed.Save(filePath);
            }
        }

        public static async Task SingleImportTableStyleAsync(string tableName, string styleDirectoryPath, List<int> relatedIdentities)
        {
            if (!DirectoryUtils.IsDirectoryExists(styleDirectoryPath)) return;

            var relatedIdentity = relatedIdentities[0];

            var filePaths = DirectoryUtils.GetFilePaths(styleDirectoryPath);
            foreach (var filePath in filePaths)
            {
                var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

                var attributeName = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.AttributeName));
                var taxis = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.Taxis)));
                var displayName = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.DisplayName));
                var helpText = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.HelpText));
                var isVisibleInList = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.VisibleInList)));
                var inputType = TranslateUtils.ToEnum(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.InputType)), InputType.Text);
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
                    InputType = inputType,
                    DefaultValue = defaultValue,
                    Horizontal = isHorizontal,
                    ExtendValues = extendValues
                };

                if (await DataProvider.TableStyleRepository.IsExistsAsync(relatedIdentity, tableName, attributeName))
                {
                    await DataProvider.TableStyleRepository.DeleteAsync(relatedIdentity, tableName, attributeName);
                }
                await DataProvider.TableStyleRepository.InsertAsync(relatedIdentities, styleInfo);
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
                    tableName = DataProvider.SiteRepository.TableName;
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
                        var inputType = TranslateUtils.ToEnum(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.InputType)), InputType.Text);
                        var defaultValue = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.DefaultValue));
                        var isHorizontal = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.Horizontal)));
                        var extendValues = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.ExtendValues));

                        var orderString = AtomUtility.GetDcElementContent(feed.AdditionalElements, "OrderString");

                        var relatedIdentity = !string.IsNullOrEmpty(orderString) ? await DataProvider.ChannelRepository.GetIdAsync(siteId, orderString) : siteId;

                        if (relatedIdentity <= 0 || await DataProvider.TableStyleRepository.IsExistsAsync(relatedIdentity, tableName, attributeName)) continue;

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
                            InputType = inputType,
                            DefaultValue = defaultValue,
                            Horizontal = isHorizontal,
                            ExtendValues = extendValues
                        };

                        await DataProvider.TableStyleRepository.InsertAsync(DataProvider.TableStyleRepository.GetRelatedIdentities(relatedIdentity), styleInfo);
                    }
                }
            }
		}

	}
}
