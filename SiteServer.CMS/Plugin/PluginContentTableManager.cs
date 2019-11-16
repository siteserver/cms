using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Plugin
{
    public static class PluginContentTableManager
    {
        public static bool IsContentTable(ServiceImpl service)
        {
            return !string.IsNullOrEmpty(service.ContentTableName) &&
                                     service.ContentTableColumns != null && service.ContentTableColumns.Count > 0;
        }

        public static async Task<string> GetTableNameAsync(string pluginId)
        {
            foreach (var service in await PluginManager.GetServicesAsync())
            {
                if (service.PluginId == pluginId && IsContentTable(service))
                {
                    return service.ContentTableName;
                }
            }

            return string.Empty;
        }

        public static async Task SyncContentTableAsync(ServiceImpl service)
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
                await DataProvider.DatabaseDao.AlterSystemTableAsync(tableName, tableColumns, ContentAttribute.DropAttributes.Value);
            }

            await ContentTableCreateOrUpdateStylesAsync(tableName, service.ContentInputStyles);
        }

        private static async Task ContentTableCreateOrUpdateStylesAsync(string tableName, List<InputStyle> inputStyles)
        {
            var styleInfoList = new List<TableStyle>();
            var columnTaxis = 0;
            if (inputStyles != null)
            {
                foreach (var inputStyle in inputStyles)
                {
                    columnTaxis++;
                    var styleInfo = await TableStyleManager.GetTableStyleAsync(tableName, inputStyle.AttributeName, new List<int> { 0 });

                    var isEquals = true;

                    if (styleInfo.Type != inputStyle.InputType)
                    {
                        isEquals = false;
                        styleInfo.Type = inputStyle.InputType;
                    }

                    if (styleInfo.InputType == null)
                    {
                        styleInfo.Type = InputType.Text;
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

                    if (styleInfo.IsRequired != inputStyle.IsRequired)
                    {
                        isEquals = false;
                        styleInfo.IsRequired = inputStyle.IsRequired;
                    }

                    if (styleInfo.ValidateType != inputStyle.ValidateType)
                    {
                        isEquals = false;
                        styleInfo.ValidateType = inputStyle.ValidateType;
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
                        var styleItems = styleInfo.StyleItems ?? new List<TableStyleItem>();
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
                                styleItems.Add(new TableStyleItem
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
                    styleInfo.IsValidate = true;
                    styleInfoList.Add(styleInfo);
                }
            }

            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.Id == 0)
                {
                    await DataProvider.TableStyleDao.InsertAsync(styleInfo);
                }
                else
                {
                    await DataProvider.TableStyleDao.UpdateAsync(styleInfo);
                }
            }
        }
    }
}
