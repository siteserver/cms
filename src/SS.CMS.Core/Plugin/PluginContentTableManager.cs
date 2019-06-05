using System.Collections.Generic;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Core.Plugin.Impl;
using SS.CMS.Core.Repositories;
using SS.CMS.Data;
using SS.CMS.Plugin;
using SS.CMS.Utils;

namespace SS.CMS.Core.Plugin
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
            tableColumns.AddRange(ContentDao.TableColumnsDefault);
            tableColumns.AddRange(service.ContentTableColumns);

            if (!DatabaseUtils.IsTableExists(tableName))
            {
                TableColumnManager.CreateContentTable(tableName, tableColumns);
            }
            else
            {
                TableColumnManager.AlterSystemTable(tableName, tableColumns, ContentAttribute.DropAttributes.Value);
            }

            ContentTableCreateOrUpdateStyles(tableName, service.ContentInputStyles);
        }

        private static void ContentTableCreateOrUpdateStyles(string tableName, List<InputStyle> inputStyles)
        {
            var styleInfoList = new List<TableStyleInfo>();
            var columnTaxis = 0;
            foreach (var inputStyle in inputStyles)
            {
                columnTaxis++;
                var styleInfo = TableStyleManager.GetTableStyleInfo(tableName, inputStyle.AttributeName, new List<int> { 0 });

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

                if (ValidateTypeUtils.Equals(styleInfo.ValidateType, inputStyle.ValidateType))
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
                    DataProvider.TableStyleDao.Insert(styleInfo);
                }
                else
                {
                    DataProvider.TableStyleDao.Update(styleInfo);
                }
            }
        }
    }
}
