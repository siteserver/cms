using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils.Serialization.Atom.Atom.Core;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils.Serialization.Components
{
	internal class TableStyleIe
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly CacheUtils _caching;
        private readonly string _directoryPath;

        public TableStyleIe(IDatabaseManager databaseManager, CacheUtils caching, string directoryPath)
        {
            _databaseManager = databaseManager;
            _caching = caching;
            _directoryPath = directoryPath;
        }

		public async Task ExportTableStylesAsync(int siteId, string tableName)
		{
            var allRelatedIdentities = await _databaseManager.ChannelRepository.GetChannelIdsAsync(siteId);
            allRelatedIdentities.Insert(0, 0);
            var tableStyleWithItemsDict = await _databaseManager.TableStyleRepository.GetTableStyleWithItemsDictionaryAsync(tableName, allRelatedIdentities);
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
		                if (!await _databaseManager.ChannelRepository.IsAncestorOrSelfAsync(siteId, siteId, tableStyle.RelatedIdentity))
		                {
		                    continue;
		                }
		            }
		            var filePath = attributeNameDirectoryPath + PathUtils.SeparatorChar + tableStyle.Id + ".xml";
		            var feed = await ExportTableStyleAsync(_databaseManager, siteId, tableStyle);
                    feed.Save(filePath);
		        }
		    }
		}

        private static async Task<AtomFeed> ExportTableStyleAsync(IDatabaseManager databaseManager, int siteId, TableStyle tableStyle)
		{
			var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string>{ nameof(TableStyle.Id), "TableStyleID" }, tableStyle.Id.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.RelatedIdentity), tableStyle.RelatedIdentity.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.TableName), tableStyle.TableName);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.AttributeName), tableStyle.AttributeName);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.Taxis), tableStyle.Taxis.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.DisplayName), tableStyle.DisplayName);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.HelpText), tableStyle.HelpText);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.List), tableStyle.List.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.InputType), tableStyle.InputType.GetValue());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.DefaultValue), tableStyle.DefaultValue);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.Horizontal), tableStyle.Horizontal.ToString());

            var json = AtomUtility.GetDcElementContent(feed.AdditionalElements,
                "ExtendValues");
            if (!string.IsNullOrEmpty(json))
            {
                var dict = ListUtils.ToDictionary(json);
                foreach (var o in dict)
                {
                    tableStyle.Set(o.Key, o.Value);
                }
            }

            //保存此栏目样式在系统中的排序号
            var orderString = string.Empty;
            if (siteId > 0 && tableStyle.RelatedIdentity != 0)
            {
                orderString = await databaseManager.ChannelRepository.ImportGetOrderStringInSiteAsync(siteId, tableStyle.RelatedIdentity);
            }

            AtomUtility.AddDcElement(feed.AdditionalElements, "OrderString", orderString);

			return feed;
		}

        public static async Task SingleExportTableStylesAsync(IDatabaseManager databaseManager, string tableName, int siteId, int relatedIdentity, string styleDirectoryPath)
        {
            var channelInfo = await databaseManager.ChannelRepository.GetAsync(relatedIdentity);
            var relatedIdentities = databaseManager.TableStyleRepository.GetRelatedIdentities(channelInfo);

            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(styleDirectoryPath);

            var styleInfoList = await databaseManager.TableStyleRepository.GetStylesAsync(tableName, relatedIdentities);
            foreach (var tableStyle in styleInfoList)
            {
                var filePath = PathUtils.Combine(styleDirectoryPath, tableStyle.AttributeName + ".xml");
                var feed = await ExportTableStyleAsync(databaseManager, siteId, tableStyle);
                feed.Save(filePath);
            }
        }

        public static async Task SingleExportTableStylesAsync(IDatabaseManager databaseManager, int siteId, string tableName, List<int> relatedIdentities, string styleDirectoryPath)
        {
            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(styleDirectoryPath);

            var styleInfoList = await databaseManager.TableStyleRepository.GetStylesAsync(tableName, relatedIdentities);
            foreach (var tableStyle in styleInfoList)
            {
                var filePath = PathUtils.Combine(styleDirectoryPath, tableStyle.AttributeName + ".xml");
                var feed = await ExportTableStyleAsync(databaseManager, siteId, tableStyle);
                feed.Save(filePath);
            }
        }

        public static async Task SingleImportTableStyleAsync(IDatabaseManager databaseManager, string tableName, string styleDirectoryPath, List<int> relatedIdentities)
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
                var list = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, new List<string>
                {
                    nameof(TableStyle.List),
                    "VisibleInList"
                }));
                var inputType = TranslateUtils.ToEnum(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.InputType)), InputType.Text);
                var defaultValue = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.DefaultValue));
                var isHorizontal = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.Horizontal)));

                var styleInfo = new TableStyle
                {
                    Id = 0,
                    RelatedIdentity = relatedIdentity,
                    TableName = tableName,
                    AttributeName = attributeName,
                    Taxis = taxis,
                    DisplayName = displayName,
                    HelpText = helpText,
                    List = list,
                    InputType = inputType,
                    DefaultValue = defaultValue,
                    Horizontal = isHorizontal
                };

                var json = AtomUtility.GetDcElementContent(feed.AdditionalElements,
                    "ExtendValues");
                if (!string.IsNullOrEmpty(json))
                {
                    var dict = ListUtils.ToDictionary(json);
                    foreach (var o in dict)
                    {
                        styleInfo.Set(o.Key, o.Value);
                    }
                }

                if (await databaseManager.TableStyleRepository.IsExistsAsync(relatedIdentity, tableName, attributeName))
                {
                    await databaseManager.TableStyleRepository.DeleteAsync(relatedIdentity, tableName, attributeName);
                }
                await databaseManager.TableStyleRepository.InsertAsync(relatedIdentities, styleInfo);
            }
        }

        public async Task ImportTableStylesAsync(Site site, string guid)
		{
			if (!DirectoryUtils.IsDirectoryExists(_directoryPath)) return;

            var styleDirectoryPaths = DirectoryUtils.GetDirectoryPaths(_directoryPath);

            var styles = new List<TableStyle>();
            foreach (var styleDirectoryPath in styleDirectoryPaths)
            {
                var tableName = PathUtils.GetDirectoryName(styleDirectoryPath, false);
                if (tableName == "siteserver_PublishmentSystem")
                {
                    tableName = _databaseManager.SiteRepository.TableName;
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
                        var list = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, new List<string>
                        {
                            nameof(TableStyle.List),
                            "VisibleInList"
                        }));
                        var inputType = TranslateUtils.ToEnum(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.InputType)), InputType.Text);
                        var defaultValue = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.DefaultValue));
                        var isHorizontal = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.Horizontal)));

                        var orderString = AtomUtility.GetDcElementContent(feed.AdditionalElements, "OrderString");

                        var relatedIdentity = !string.IsNullOrEmpty(orderString) ? await _databaseManager.ChannelRepository.ImportGetIdAsync(site.Id, orderString) : site.Id;

                        if (relatedIdentity <= 0 || await _databaseManager.TableStyleRepository.IsExistsAsync(relatedIdentity, tableName, attributeName)) continue;

                        var styleInfo = new TableStyle
                        {
                            Id = 0,
                            RelatedIdentity = relatedIdentity,
                            TableName = tableName,
                            AttributeName = attributeName,
                            Taxis = taxis,
                            DisplayName = displayName,
                            HelpText = helpText,
                            List = list,
                            InputType = inputType,
                            DefaultValue = defaultValue,
                            Horizontal = isHorizontal
                        };

                        var json = AtomUtility.GetDcElementContent(feed.AdditionalElements,
                            "ExtendValues");
                        if (!string.IsNullOrEmpty(json))
                        {
                            var dict = ListUtils.ToDictionary(json);
                            foreach (var o in dict)
                            {
                                styleInfo.Set(o.Key, o.Value);
                            }
                        }

                        styles.Add(styleInfo);
                    }
                }
            }

            foreach (var styleInfo in styles)
            {
                _caching.SetProcess(guid, $"导入表字段: {styleInfo.AttributeName}");
                await _databaseManager.TableStyleRepository.InsertAsync(_databaseManager.TableStyleRepository.GetRelatedIdentities(styleInfo.RelatedIdentity), styleInfo);
            }
        }

	}
}
