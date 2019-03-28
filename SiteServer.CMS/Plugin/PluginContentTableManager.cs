using System.Collections.Generic;
using SiteServer.CMS.Apis;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Attributes;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;
using TableColumn = SiteServer.Plugin.TableColumn;

namespace SiteServer.CMS.Plugin
{
    public static class PluginContentTableManager
    {
        public static bool IsContentTable(ServiceImpl service)
        {
            return !string.IsNullOrEmpty(service.ContentTableName) &&
                                     service.ContentTableColumns != null && service.ContentTableColumns.Count > 0;
        }

        public static string GetTableName(string pluginId)
        {
            foreach (var service in PluginManager.Services)
            {
                if (service.PluginId == pluginId && IsContentTable(service))
                {
                    return service.ContentTableName;
                }
            }

            return string.Empty;
        }

        public static void SyncContentTable(ServiceImpl service)
        {
            if (!IsContentTable(service)) return;

            var tableName = service.ContentTableName;

            var tableColumns = new List<TableColumn>();
            tableColumns.AddRange(DataProvider.ContentRepository.TableColumns);
            tableColumns.AddRange(service.ContentTableColumns);

            if (!DatabaseApi.Instance.IsTableExists(tableName))
            {
                DatabaseApi.Instance.CreateTable(tableName, tableColumns, service.PluginId, true, out _, out _);
            }
            else
            {
                DatabaseApi.Instance.AlterTable(tableName, tableColumns, service.PluginId, ContentAttribute.DropAttributes.Value);
            }

            ContentTableCreateOrUpdateStyles(tableName, service.ContentTableColumns);
        }

        private static void ContentTableCreateOrUpdateStyles(string tableName, List<TableColumn> tableColumns)
        {
            var styleInfoList = new List<TableStyleInfo>();
            var columnTaxis = 0;
            foreach (var tableColumn in tableColumns)
            {
                var inputStyle = tableColumn.InputStyle ?? new InputStyle
                {
                    InputType = InputType.Hidden
                };

                columnTaxis++;
                var styleInfo = TableStyleManager.GetTableStyleInfo(tableName, tableColumn.AttributeName, new List<int> { 0 });

                var isEquals = true;

                if (styleInfo.Type != inputStyle.InputType)
                {
                    isEquals = false;
                    styleInfo.Type = inputStyle.InputType;
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

                if (styleInfo.Required != inputStyle.IsRequired)
                {
                    isEquals = false;
                    styleInfo.Required = inputStyle.IsRequired;
                }

                if (!ValidateTypeUtils.Equals(inputStyle.ValidateType, styleInfo.ValidateType))
                {
                    isEquals = false;
                    styleInfo.ValidateType = inputStyle.ValidateType.Value;
                }

                if (styleInfo.MinNum != inputStyle.MinNum)
                {
                    isEquals = false;
                    styleInfo.MinNum = inputStyle.MinNum;
                }

                if (styleInfo.MaxNum != inputStyle.MaxNum)
                {
                    isEquals = false;
                    styleInfo.MaxNum = inputStyle.MaxNum;
                }

                if (!StringUtils.EqualsIgnoreNull(styleInfo.RegExp, inputStyle.RegExp))
                {
                    isEquals = false;
                    styleInfo.RegExp = inputStyle.RegExp;
                }

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

                if (!(styleInfo.StyleItems == null && inputStyle.ListItems == null))
                {
                    var styleItems = styleInfo.StyleItems ?? new List<TableStyleItemInfo>();
                    var listItems = inputStyle.ListItems ?? new List<InputListItem>();

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
                            styleItems.Add(new TableStyleItemInfo
                            {
                                TableStyleId = styleInfo.Id,
                                ItemTitle = listItem.Text,
                                ItemValue = listItem.Value,
                                Selected = listItem.Selected
                            });
                        }
                        else
                        {
                            var styleItem = styleItems[i];

                            if (!StringUtils.EqualsIgnoreNull(styleItem.ItemTitle, listItem.Text))
                            {
                                isEquals = false;
                                styleItem.ItemTitle = listItem.Text;
                            }

                            if (!StringUtils.EqualsIgnoreNull(styleItem.ItemValue, listItem.Value))
                            {
                                isEquals = false;
                                styleItem.ItemValue = listItem.Value;
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

                styleInfo.VisibleInList = false;
                styleInfo.Validate = true;
                styleInfoList.Add(styleInfo);
            }

            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.Id == 0)
                {
                    DataProvider.TableStyle.Insert(styleInfo);
                }
                else
                {
                    DataProvider.TableStyle.Update(styleInfo);
                }
            }
        }
    }
}
