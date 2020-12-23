using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Configuration;
using SSCMS.Core.Utils.Serialization.Atom.Atom.Core;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils.Serialization.Components
{
	internal class TableStyleIe
    {
        private const string DirectoryNameContentTable = "contentTable";

        private readonly IDatabaseManager _databaseManager;
        private readonly CacheUtils _caching;
        private readonly string _directoryPath;

        public TableStyleIe(IDatabaseManager databaseManager, CacheUtils caching, string directoryPath)
        {
            _databaseManager = databaseManager;
            _caching = caching;
            _directoryPath = directoryPath;
        }

        public async Task ExportTableStylesAsync(int siteId, bool isContentTable, string tableName)
		{
            var relatedIdentities = await _databaseManager.ChannelRepository.GetChannelIdsAsync(siteId);
            relatedIdentities.Insert(0, 0);

            if (StringUtils.EqualsIgnoreCase(tableName, _databaseManager.SiteRepository.TableName))
            {
                relatedIdentities = _databaseManager.TableStyleRepository.GetRelatedIdentities(siteId);
            }

            var tableStyleWithItemsDict = await _databaseManager.TableStyleRepository.GetTableStyleWithItemsDictionaryAsync(tableName, relatedIdentities);
		    if (tableStyleWithItemsDict == null || tableStyleWithItemsDict.Count <= 0) return;

		    var styleDirectoryPath = PathUtils.Combine(_directoryPath, tableName);
            if (isContentTable)
            {
                styleDirectoryPath = PathUtils.Combine(_directoryPath, DirectoryNameContentTable);
            }
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
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.ItemValues), tableStyle.ItemValues);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(TableStyle.RuleValues), tableStyle.RuleValues);

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

            var styleList = await databaseManager.TableStyleRepository.GetTableStylesAsync(tableName, relatedIdentities);
            foreach (var tableStyle in styleList)
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

            var styleList = await databaseManager.TableStyleRepository.GetTableStylesAsync(tableName, relatedIdentities);
            foreach (var tableStyle in styleList)
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
                if (string.IsNullOrEmpty(attributeName))
                {
                    attributeName = AtomUtility.GetDcElementContent(feed.AdditionalElements, "Title");
                }

                if (string.IsNullOrEmpty(attributeName)) continue;
                var taxis = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.Taxis)));
                var displayName = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.DisplayName));
                if (string.IsNullOrEmpty(displayName))
                {
                    displayName = attributeName;
                }
                var helpText = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.HelpText));
                var list = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, new List<string>
                {
                    nameof(TableStyle.List),
                    "VisibleInList"
                }));
                var inputTypeString = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.InputType));
                if (string.IsNullOrEmpty(inputTypeString))
                {
                    inputTypeString = AtomUtility.GetDcElementContent(feed.AdditionalElements, "FieldType");
                }
                var inputType = TranslateUtils.ToEnum(inputTypeString, InputType.Text);
                var defaultValue = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.DefaultValue));
                var isHorizontal = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.Horizontal)));

                var style = new TableStyle
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
                        style.Set(o.Key, o.Value);
                    }
                }

                if (await databaseManager.TableStyleRepository.IsExistsAsync(relatedIdentity, tableName, attributeName))
                {
                    await databaseManager.TableStyleRepository.DeleteAsync(tableName, relatedIdentity, attributeName);
                }
                await databaseManager.TableStyleRepository.InsertAsync(relatedIdentities, style);
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
                if (StringUtils.EqualsIgnoreCase(tableName, DirectoryNameContentTable))
                {
                    tableName = site.TableName;
                }
                else if (tableName == "siteserver_PublishmentSystem")
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
                        var itemValues =
                            AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.ItemValues));
                        var ruleValues =
                            AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.RuleValues));

                        var orderString = AtomUtility.GetDcElementContent(feed.AdditionalElements, "OrderString");

                        var relatedIdentity = !string.IsNullOrEmpty(orderString) ? await _databaseManager.ChannelRepository.ImportGetIdAsync(site.Id, orderString) : site.Id;

                        if (relatedIdentity <= 0 || await _databaseManager.TableStyleRepository.IsExistsAsync(relatedIdentity, tableName, attributeName)) continue;

                        var style = new TableStyle
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
                            Horizontal = isHorizontal,
                            ItemValues = itemValues,
                            RuleValues = ruleValues
                        };

                        style.Items = TranslateUtils.JsonDeserialize<List<InputStyleItem>>(style.ItemValues);
                        style.Rules = TranslateUtils.JsonDeserialize<List<InputStyleRule>>(style.RuleValues);

                        var json = AtomUtility.GetDcElementContent(feed.AdditionalElements,
                            "ExtendValues");
                        if (!string.IsNullOrEmpty(json))
                        {
                            var dict = ListUtils.ToDictionary(json);
                            foreach (var o in dict)
                            {
                                style.Set(o.Key, o.Value);
                            }
                        }

                        styles.Add(style);
                    }
                }
            }

            foreach (var style in styles)
            {
                _caching.SetProcess(guid, $"导入表字段: {style.AttributeName}");
                await _databaseManager.TableStyleRepository.InsertAsync(_databaseManager.TableStyleRepository.GetRelatedIdentities(style.RelatedIdentity), style);
            }
        }

	}
}
