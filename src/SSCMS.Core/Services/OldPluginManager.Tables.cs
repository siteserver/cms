using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory.Utils;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Utils;
using TableColumn = Datory.TableColumn;

namespace SSCMS.Core.Services
{
    public partial class OldPluginManager
    {
        public async void SyncTable(IOldPlugin pluginService)
        {
            if (pluginService.DatabaseTables == null || pluginService.DatabaseTables.Count <= 0) return;

            foreach (var tableName in pluginService.DatabaseTables.Keys)
            {
                var tableColumns = pluginService.DatabaseTables[tableName];
                if (tableColumns == null || tableColumns.Count == 0) continue;

                if (!await _settingsManager.Database.IsTableExistsAsync(tableName))
                {
                    await _settingsManager.Database.CreateTableAsync(tableName, tableColumns);
                }
                else
                {
                    await _settingsManager.Database.AlterTableAsync(tableName, tableColumns);
                }
            }
        }

        public bool IsContentTable(IOldPlugin pluginService)
        {
            return !string.IsNullOrEmpty(pluginService.ContentTableName) &&
                                     pluginService.ContentTableColumns != null && pluginService.ContentTableColumns.Count > 0;
        }

        public string GetTableName(string pluginId)
        {
            foreach (var plugin in GetPlugins())
            {
                if (plugin.PluginId == pluginId && IsContentTable(plugin))
                {
                    return plugin.ContentTableName;
                }
            }

            return string.Empty;
        }

        public async Task SyncContentTableAsync(IOldPlugin pluginService)
        {
            if (!IsContentTable(pluginService)) return;

            var tableName = pluginService.ContentTableName;

            var tableColumns = new List<TableColumn>();
            tableColumns.AddRange(_databaseManager.ContentRepository.GetTableColumns(tableName));
            tableColumns.AddRange(pluginService.ContentTableColumns);

            var database = _settingsManager.Database;

            if (!await database.IsTableExistsAsync(tableName))
            {
                await _databaseManager.ContentRepository.CreateContentTableAsync(tableName, tableColumns);
            }
            else
            {
                await database.AlterTableAsync(tableName, tableColumns, ColumnsManager.DropAttributes.Value);
            }

            await ContentTableCreateOrUpdateStylesAsync(tableName, pluginService.ContentInputStyles);
        }

        private async Task ContentTableCreateOrUpdateStylesAsync(string tableName, List<InputStyle> inputStyles)
        {
            var styleInfoList = new List<TableStyle>();
            var columnTaxis = 0;
            if (inputStyles != null)
            {
                foreach (var inputStyle in inputStyles)
                {
                    columnTaxis++;
                    var styleInfo = await _databaseManager.TableStyleRepository.GetTableStyleAsync(tableName, inputStyle.AttributeName, _databaseManager.TableStyleRepository.EmptyRelatedIdentities);

                    var isEquals = true;

                    if (styleInfo.InputType != inputStyle.InputType)
                    {
                        isEquals = false;
                        styleInfo.InputType = inputStyle.InputType;
                    }

                    if (!StringUtils.EqualsIgnoreNull(styleInfo.DisplayName, inputStyle.DisplayName))
                    {
                        isEquals = false;
                        styleInfo.DisplayName = inputStyle.DisplayName;
                    }

                    if (!StringUtils.EqualsIgnoreNull(styleInfo.HelpText, inputStyle.HelpText))
                    {
                        isEquals = false;
                        styleInfo.HelpText = inputStyle.HelpText;
                    }

                    if (!StringUtils.EqualsIgnoreNull(styleInfo.DefaultValue, inputStyle.DefaultValue))
                    {
                        isEquals = false;
                        styleInfo.DefaultValue = inputStyle.DefaultValue;
                    }

                    if (styleInfo.Taxis != columnTaxis)
                    {
                        isEquals = false;
                        styleInfo.Taxis = columnTaxis;
                    }

                    //if (styleInfo.IsRequired != inputStyle.IsRequired)
                    //{
                    //    isEquals = false;
                    //    styleInfo.IsRequired = inputStyle.IsRequired;
                    //}

                    //if (styleInfo.ValidateType != inputStyle.ValidateType)
                    //{
                    //    isEquals = false;
                    //    styleInfo.ValidateType = inputStyle.ValidateType;
                    //}

                    //if (styleInfo.MinNum != inputStyle.MinNum)
                    //{
                    //    isEquals = false;
                    //    styleInfo.MinNum = inputStyle.MinNum;
                    //}

                    //if (styleInfo.MaxNum != inputStyle.MaxNum)
                    //{
                    //    isEquals = false;
                    //    styleInfo.MaxNum = inputStyle.MaxNum;
                    //}

                    //if (!StringUtils.EqualsIgnoreNull(styleInfo.RegExp, inputStyle.RegExp))
                    //{
                    //    isEquals = false;
                    //    styleInfo.RegExp = inputStyle.RegExp;
                    //}

                    if (!StringUtils.EqualsIgnoreNull(styleInfo.Width, inputStyle.Width))
                    {
                        isEquals = false;
                        styleInfo.Width = inputStyle.Width;
                    }

                    if (!(styleInfo.Height == 0 && string.IsNullOrEmpty(inputStyle.Height)) && styleInfo.Height != TranslateUtils.ToInt(inputStyle.Height))
                    {
                        isEquals = false;
                        styleInfo.Height = TranslateUtils.ToInt(inputStyle.Height);
                    }

                    if (!(styleInfo.Items == null && inputStyle.Items == null))
                    {
                        var styleItems = styleInfo.Items ?? new List<InputStyleItem>();
                        var listItems = inputStyle.Items ?? new List<InputStyleItem>();

                        if (styleItems.Count > listItems.Count)
                        {
                            isEquals = false;
                            styleItems.RemoveRange(listItems.Count, styleItems.Count - listItems.Count);
                        }

                        for (var i = 0; i < listItems.Count; i++)
                        {
                            var listItem = listItems[i];
                            if (styleItems.Count < i + 1)
                            {
                                isEquals = false;
                                styleItems.Add(new InputStyleItem
                                {
                                    Label = listItem.Label,
                                    Value = listItem.Value,
                                    Selected = listItem.Selected
                                });
                            }
                            else
                            {
                                var styleItem = styleItems[i];

                                if (!StringUtils.EqualsIgnoreNull(styleItem.Label, listItem.Label))
                                {
                                    isEquals = false;
                                    styleItem.Label = listItem.Label;
                                }

                                if (!StringUtils.EqualsIgnoreNull(styleItem.Value, listItem.Value))
                                {
                                    isEquals = false;
                                    styleItem.Value = listItem.Value;
                                }

                                if (styleItem.Selected != listItem.Selected)
                                {
                                    isEquals = false;
                                    styleItem.Selected = listItem.Selected;
                                }
                            }
                        }
                    }

                    if (isEquals) continue;

                    styleInfo.List = false;
                    styleInfoList.Add(styleInfo);
                }
            }

            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.Id == 0)
                {
                    await _databaseManager.TableStyleRepository.InsertAsync(_databaseManager.TableStyleRepository.EmptyRelatedIdentities,  styleInfo);
                }
                else
                {
                    await _databaseManager.TableStyleRepository.UpdateAsync(styleInfo);
                }
            }
        }

        public List<IPackageMetadata> GetContentModelPlugins()
        {
            var list = new List<IPackageMetadata>();

            foreach (var plugin in GetPlugins())
            {
                if (IsContentTable(plugin))
                {
                    list.Add(plugin);
                }
            }

            return list;
        }

        public List<string> GetContentTableNameList()
        {
            var list = new List<string>();

            foreach (var plugin in GetPlugins())
            {
                if (IsContentTable(plugin))
                {
                    if (!StringUtils.ContainsIgnoreCase(list, plugin.ContentTableName))
                    {
                        list.Add(plugin.ContentTableName);
                    }
                }
            }

            return list;
        }

        public List<IPackageMetadata> GetAllContentRelatedPlugins(bool includeContentTable)
        {
            var list = new List<IPackageMetadata>();

            foreach (var plugin in GetPlugins())
            {
                var isContentModel = IsContentTable(plugin);

                if (!includeContentTable && isContentModel) continue;

                if (isContentModel)
                {
                    list.Add(plugin);
                }
                else if (plugin.ContentColumns != null && plugin.ContentColumns.Count > 0)
                {
                    list.Add(plugin);
                }
                //else
                //{
                //    var contentMenus = plugin.GetContentMenus(content) ?? await plugin.GetContentMenusAsync(content);
                //    if (contentMenus == null) continue;
                //}
            }

            return list;
        }

        public List<IOldPlugin> GetContentPlugins(Channel channel, bool includeContentTable)
        {
            var list = new List<IOldPlugin>();
            var pluginIds = Utilities.GetStringList(channel.ContentRelatedPluginIds);
            if (!string.IsNullOrEmpty(channel.ContentModelPluginId))
            {
                pluginIds.Add(channel.ContentModelPluginId);
            }

            foreach (var plugin in GetPlugins())
            {
                if (!pluginIds.Contains(plugin.PluginId)) continue;

                if (!includeContentTable && IsContentTable(plugin)) continue;

                list.Add(plugin);
            }

            return list;
        }

        public List<string> GetContentPluginIds(Channel channel)
        {
            if (channel.ContentRelatedPluginIds != null && channel.ContentRelatedPluginIds.Any() &&
                string.IsNullOrEmpty(channel.ContentModelPluginId))
            {
                return null;
            }

            var pluginIds = Utilities.GetStringList(channel.ContentRelatedPluginIds);
            if (!string.IsNullOrEmpty(channel.ContentModelPluginId))
            {
                pluginIds.Add(channel.ContentModelPluginId);
            }

            return pluginIds;
        }

        public Dictionary<string, Dictionary<string, Func<IContentContext, string>>> GetContentColumns(List<string> pluginIds)
        {
            var dict = new Dictionary<string, Dictionary<string, Func<IContentContext, string>>>();
            if (pluginIds == null || pluginIds.Count == 0) return dict;

            foreach (var plugin in GetPlugins())
            {
                if (!pluginIds.Contains(plugin.PluginId) || plugin.ContentColumns == null || plugin.ContentColumns.Count == 0) continue;

                dict[plugin.PluginId] = plugin.ContentColumns;
            }

            return dict;
        }
    }
}
