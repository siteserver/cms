using System.Collections.Generic;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Table;
using SiteServer.Plugin.Features;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Plugin
{
    public static class PluginTableUtils
    {
        public static void SyncContentModel(IContentModel contentModel, PluginMetadata metadata)
        {
            if (string.IsNullOrEmpty(contentModel?.ContentTableName) || contentModel.ContentTableColumns == null ||
                contentModel.ContentTableColumns.Count <= 0) return;

            var tableName = contentModel.ContentTableName;

            if (!BaiRongDataProvider.TableCollectionDao.IsExists(tableName))
            {
                ContentModelCreateMetadatas(metadata, tableName, contentModel.ContentTableColumns);
            }
            else
            {
                ContentModelUpdateMetadatas(tableName, contentModel.ContentTableColumns);
            }

            if (!BaiRongDataProvider.DatabaseDao.IsTableExists(tableName))
            {
                BaiRongDataProvider.TableCollectionDao.CreateDbTable(tableName);
            }
            else
            {
                BaiRongDataProvider.TableCollectionDao.SyncDbTable(tableName);
            }

            ContentModelCreateOrUpdateStyles(tableName, contentModel.ContentTableColumns);
        }

        private static void ContentModelCreateMetadatas(PluginMetadata metadata, string tableName, List<PluginTableColumn> tableColumns)
        {
            BaiRongDataProvider.TableCollectionDao.DeleteCollectionTableInfoAndDbTable(tableName);
            BaiRongDataProvider.TableMetadataDao.Delete(tableName);
            BaiRongDataProvider.TableStyleDao.Delete(tableName);

            var metadataInfoList = new List<TableMetadataInfo>();
            foreach (var tableColumn in tableColumns)
            {
                if (string.IsNullOrEmpty(tableColumn.AttributeName) ||
                    ContentAttribute.AllAttributesLowercase.Contains(tableColumn.AttributeName.ToLower()))
                    continue;

                metadataInfoList.Add(new TableMetadataInfo(0, tableName,
                    tableColumn.AttributeName, DataTypeUtils.GetEnumType(tableColumn.DataType), tableColumn.DataLength,
                    0, true));
            }

            BaiRongDataProvider.TableCollectionDao.Insert(new TableCollectionInfo(tableName,
                $"插件内容表：{metadata.DisplayName}", 0, false, false, false, string.Empty), metadataInfoList);
        }

        private static void ContentModelUpdateMetadatas(string tableName, List<PluginTableColumn> tableColumns)
        {
            var metadataInfoListToInsert = new List<TableMetadataInfo>();
            var metadataInfoListToUpdate = new List<TableMetadataInfo>();

            foreach (var tableColumn in tableColumns)
            {
                if (string.IsNullOrEmpty(tableColumn.AttributeName) ||
                    ContentAttribute.AllAttributesLowercase.Contains(tableColumn.AttributeName.ToLower()))
                    continue;

                if (!TableMetadataManager.IsAttributeNameExists(tableName, tableColumn.AttributeName))
                {
                    var metadataInfo = new TableMetadataInfo(0, tableName,
                        tableColumn.AttributeName, DataTypeUtils.GetEnumType(tableColumn.DataType),
                        tableColumn.DataLength, 0, true);
                    metadataInfoListToInsert.Add(metadataInfo);
                }
                else
                {
                    var isEquals = true;

                    var metadataInfo = TableMetadataManager.GetTableMetadataInfo(tableName, tableColumn.AttributeName);

                    if (metadataInfo.DataType != DataTypeUtils.GetEnumType(tableColumn.DataType))
                    {
                        isEquals = false;
                        metadataInfo.DataType = DataTypeUtils.GetEnumType(tableColumn.DataType);
                    }

                    if (metadataInfo.DataLength != tableColumn.DataLength)
                    {
                        isEquals = false;
                        metadataInfo.DataLength = tableColumn.DataLength;
                    }

                    if (isEquals) continue;

                    metadataInfoListToUpdate.Add(metadataInfo);
                }
            }

            foreach (var metadataInfo in metadataInfoListToInsert)
            {
                BaiRongDataProvider.TableMetadataDao.Insert(metadataInfo);
            }
            foreach (var metadataInfo in metadataInfoListToUpdate)
            {
                BaiRongDataProvider.TableMetadataDao.Update(metadataInfo);
            }
        }

        private static void ContentModelCreateOrUpdateStyles(string tableName, List<PluginTableColumn> tableColumns)
        {
            var styleInfoList = new List<TableStyleInfo>();
            var columnTaxis = 0;
            foreach (var tableColumn in tableColumns)
            {
                var inputStyle = tableColumn.InputStyle ?? new PluginInputStyle
                {
                    InputType = nameof(InputType.Hidden)
                };

                columnTaxis++;
                var styleInfo = TableStyleManager.GetTableStyleInfo(tableName, tableColumn.AttributeName, new List<int> { 0 });

                var isEquals = true;

                if (!StringUtils.EqualsIgnoreNull(styleInfo.InputType, inputStyle.InputType))
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

                if (styleInfo.Additional.ValidateType != ValidateTypeUtils.GetEnumType(inputStyle.ValidateType))
                {
                    isEquals = false;
                    styleInfo.Additional.ValidateType = ValidateTypeUtils.GetEnumType(inputStyle.ValidateType);
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
                    var listItems = inputStyle.ListItems ?? new List<ListItem>();

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
                                TableStyleId = styleInfo.TableStyleId,
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
                if (styleInfo.TableStyleId == 0)
                {
                    TableStyleManager.Insert(styleInfo);
                }
                else
                {
                    TableStyleManager.Update(styleInfo);
                    TableStyleManager.DeleteAndInsertStyleItems(styleInfo.TableStyleId, styleInfo.StyleItems);
                }
            }
        }

        public static void SyncTable(ITable table, PluginMetadata metadata)
        {
            if (table?.Tables == null) return;

            foreach (var tableName in table.Tables.Keys)
            {
                var tableColumns = table.Tables[tableName];
                if (tableColumns == null || tableColumns.Count == 0) continue;

                if (!BaiRongDataProvider.DatabaseDao.IsTableExists(tableName))
                {
                    BaiRongDataProvider.DatabaseDao.CreatePluginTable(metadata.Id, tableName, tableColumns);
                }
                else
                {
                    BaiRongDataProvider.DatabaseDao.AlterPluginTable(metadata.Id, tableName, tableColumns);
                }
            }
        }
    }
}
