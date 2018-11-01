using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
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
            tableColumns.AddRange(DataProvider.ContentDao.TableColumns);
            tableColumns.AddRange(service.ContentTableColumns);

            if (!DataProvider.DatabaseDao.IsTableExists(tableName))
            {
                DataProvider.ContentDao.CreateContentTable(tableName, tableColumns);
            }
            else
            {
                DataProvider.DatabaseDao.AlterSystemTable(tableName, tableColumns, ContentAttribute.DropAttributes.Value);
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

                if (styleInfo.Additional.IsRequired != inputStyle.IsRequired)
                {
                    isEquals = false;
                    styleInfo.Additional.IsRequired = inputStyle.IsRequired;
                }

                if (styleInfo.Additional.ValidateType != inputStyle.ValidateType)
                {
                    isEquals = false;
                    styleInfo.Additional.ValidateType = inputStyle.ValidateType;
                }

                if (styleInfo.Additional.MinNum != inputStyle.MinNum)
                {
                    isEquals = false;
                    styleInfo.Additional.MinNum = inputStyle.MinNum;
                }

                if (styleInfo.Additional.MaxNum != inputStyle.MaxNum)
                {
                    isEquals = false;
                    styleInfo.Additional.MaxNum = inputStyle.MaxNum;
                }

                if (!StringUtils.EqualsIgnoreNull(styleInfo.Additional.RegExp, inputStyle.RegExp))
                {
                    isEquals = false;
                    styleInfo.Additional.RegExp = inputStyle.RegExp;
                }

                if (!StringUtils.EqualsIgnoreNull(styleInfo.Additional.Width, inputStyle.Width))
                {
                    isEquals = false;
                    styleInfo.Additional.Width = inputStyle.Width;
                }

                if (!(styleInfo.Additional.Height == 0 && string.IsNullOrEmpty(inputStyle.Height)) && styleInfo.Additional.Height != TranslateUtils.ToInt(inputStyle.Height))
                {
                    isEquals = false;
                    styleInfo.Additional.Height = TranslateUtils.ToInt(inputStyle.Height);
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
                                IsSelected = listItem.Selected
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

                            if (styleItem.IsSelected != listItem.Selected)
                            {
                                isEquals = false;
                                styleItem.IsSelected = listItem.Selected;
                            }
                        }
                    }
                }

                if (isEquals) continue;

                styleInfo.IsVisibleInList = false;
                styleInfo.Additional.IsValidate = true;
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
