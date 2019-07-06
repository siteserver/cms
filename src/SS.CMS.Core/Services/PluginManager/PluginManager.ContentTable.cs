using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class PluginManager
    {
        public async Task SyncContentTableAsync(IService service)
        {
            if (!IsContentTable(service)) return;

            var tableName = service.ContentTableName;

            var tableColumns = new List<TableColumn>();

            var db = new Database(_settingsManager.DatabaseType, _settingsManager.DatabaseConnectionString);
            var defaultContentTableColumns = db.GetTableColumns<ContentInfo>();

            tableColumns.AddRange(defaultContentTableColumns);
            tableColumns.AddRange(service.ContentTableColumns);

            if (!await db.IsTableExistsAsync(tableName))
            {
                await _tableManager.CreateContentTableAsync(tableName, tableColumns);
            }
            else
            {
                await _tableManager.AlterSystemTableAsync(tableName, tableColumns, ContentAttribute.DropAttributes.Value);
            }

            await ContentTableCreateOrUpdateStylesAsync(tableName, service.ContentInputStyles);
        }

        public async Task<bool> IsContentTableUsedAsync(string tableName)
        {
            var count = _siteRepository.GetTableCount(tableName);

            if (count > 0) return true;

            var contentModelPluginIdList = _channelRepository.GetContentModelPluginIdList();
            foreach (var pluginId in contentModelPluginIdList)
            {
                var service = await GetServiceAsync(pluginId);
                if (service != null && IsContentTable(service) && service.ContentTableName == tableName)
                {
                    return true;
                }
            }

            return false;
        }

        private async Task ContentTableCreateOrUpdateStylesAsync(string tableName, List<InputStyle> inputStyles)
        {
            var styleInfoList = new List<TableStyleInfo>();
            var columnTaxis = 0;
            foreach (var inputStyle in inputStyles)
            {
                columnTaxis++;
                var styleInfo = await _tableManager.GetTableStyleInfoAsync(tableName, inputStyle.AttributeName, new List<int> { 0 });

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

                if (styleInfo.IsRequired != inputStyle.IsRequired)
                {
                    isEquals = false;
                    styleInfo.IsRequired = inputStyle.IsRequired;
                }

                var validateType = ValidateType.GetValidateType(styleInfo.ValidateType);
                if (validateType == inputStyle.ValidateType)
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
                styleInfo.IsValidate = true;
                styleInfoList.Add(styleInfo);
            }

            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.Id == 0)
                {
                    await _tableStyleRepository.InsertAsync(styleInfo);
                }
                else
                {
                    await _tableStyleRepository.UpdateAsync(styleInfo);
                }
            }
        }
    }
}
