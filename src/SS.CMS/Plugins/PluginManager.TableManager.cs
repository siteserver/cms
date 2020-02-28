using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SS.CMS.Abstractions;
using SS.CMS.Core;
using SS.CMS.Plugins.Impl;

namespace SS.CMS.Plugins
{
    public static partial class PluginManager
    {
        public static bool IsContentTable(ServiceImpl service)
        {
            return !string.IsNullOrEmpty(service.ContentTableName) &&
                                     service.ContentTableColumns != null && service.ContentTableColumns.Count > 0;
        }

        public static async Task<string> GetTableNameAsync(string pluginId)
        {
            foreach (var service in await GetServicesAsync())
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
            tableColumns.AddRange(GlobalSettings.ContentRepository.GetTableColumns(tableName));
            tableColumns.AddRange(service.ContentTableColumns);

            var database = GlobalSettings.Database;

            if (!await database.IsTableExistsAsync(tableName))
            {
                await GlobalSettings.ContentRepository.CreateContentTableAsync(tableName, tableColumns);
            }
            else
            {
                await database.AlterTableAsync(tableName, tableColumns, ContentAttribute.DropAttributes.Value);
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
                    var styleInfo = await GlobalSettings.TableStyleRepository.GetTableStyleAsync(tableName, inputStyle.AttributeName, GlobalSettings.TableStyleRepository.EmptyRelatedIdentities);

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

                    if (!(styleInfo.Items == null && inputStyle.ListItems == null))
                    {
                        var styleItems = styleInfo.Items ?? new List<TableStyleItem>();
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
                                    Label = listItem.Text,
                                    Value = listItem.Value,
                                    Selected = listItem.Selected
                                });
                            }
                            else
                            {
                                var styleItem = styleItems[i];

                                if (!StringUtils.EqualsIgnoreNull(styleItem.Label, listItem.Text))
                                {
                                    isEquals = false;
                                    styleItem.Label = listItem.Text;
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
                    styleInfo.IsValidate = true;
                    styleInfoList.Add(styleInfo);
                }
            }

            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.Id == 0)
                {
                    await GlobalSettings.TableStyleRepository.InsertAsync(GlobalSettings.TableStyleRepository.EmptyRelatedIdentities,  styleInfo);
                }
                else
                {
                    await GlobalSettings.TableStyleRepository.UpdateAsync(styleInfo);
                }
            }
        }
    }
}
