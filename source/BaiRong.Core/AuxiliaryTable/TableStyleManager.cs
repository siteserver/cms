using System.Collections.Generic;
using System.Data;
using BaiRong.Core.Collection;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.Plugin.Models;

namespace BaiRong.Core.AuxiliaryTable
{
    public sealed class TableStyleManager
    {
        private static readonly object LockObject = new object();
        private static bool _async = true;//缓存与数据库不同步
        private const string CacheKey = "BaiRong.Core.AuxiliaryTable.TableStyleManager";

        public static PairList GetAllTableStyleInfoPairs()
        {
            lock (LockObject)
            { 
                if (_async || CacheUtils.Get(CacheKey) == null)
                {
                    var entries = BaiRongDataProvider.TableStyleDao.GetAllTableStyleInfoPairs();
                    CacheUtils.Insert(CacheKey, entries);
                    _async = false;
                    return entries;
                }
                return (PairList)CacheUtils.Get(CacheKey);
            }
        }

        public static List<TableStyleInfo> GetTableStyleInfoList(ETableStyle tableStyle, string tableName, List<int> relatedIdentities)
        {
            relatedIdentities = GetRelatedIdentities(relatedIdentities);

            var dict = new Dictionary<string, TableStyleInfo>();
            var allAttributeNames = new List<string>();
            var styleInfoList = new List<TableStyleInfo>();

            var entries = GetAllTableStyleInfoPairs();
            var attributeNames = TableManager.GetAttributeNameList(tableStyle, tableName);
            
            //添加所有实际保存在数据库中的样式
            var i = 99;
            foreach (var relatedIdentity in relatedIdentities)
            {
                //if (ETableStyleUtils.IsContent(tableStyle)) continue;
                var startKey = GetCacheKeyStart(relatedIdentity, tableName);
                var keyList = entries.GetKeys(startKey);
                foreach (var key in keyList)
                {
                    var styleInfo = entries.GetValue(key) as TableStyleInfo;
                    if (styleInfo == null || allAttributeNames.Contains(styleInfo.AttributeName)) continue;

                    allAttributeNames.Add(styleInfo.AttributeName);
                    if (styleInfo.Taxis <= 0 && attributeNames.Contains(styleInfo.AttributeName))//数据库字段
                    {
                        dict.Add(styleInfo.AttributeName, styleInfo);
                    }
                    else if (styleInfo.Taxis > 0)                       //排序字段
                    {
                        var iStr = relatedIdentity + styleInfo.Taxis.ToString().PadLeft(3, '0');
                        dict.Add("1" + iStr + "_" + styleInfo.AttributeName, styleInfo);
                    }
                    else                                                //未排序字段
                    {
                        var iStr = relatedIdentity + i.ToString().PadLeft(3, '0');
                        dict.Add("0" + iStr + "_" + styleInfo.AttributeName, styleInfo);
                        i = i - 1;
                    }
                }
            }

            foreach (var attributeName in attributeNames)
            {
                TableStyleInfo styleInfo;
                if (!allAttributeNames.Contains(attributeName))
                {
                    allAttributeNames.Add(attributeName);

                    if (tableStyle == ETableStyle.BackgroundContent)
                    {
                        styleInfo = GetBackgroundContentTableStyleInfo(entries, tableName, attributeName);
                    }
                    else if (tableStyle == ETableStyle.Custom)
                    {
                        styleInfo = GetCustomTableStyleInfo(tableName, attributeName);
                    }
                    else
                    {
                        styleInfo = GetDefaultTableStyleInfo(tableName, attributeName);
                    }

                    styleInfoList.Add(styleInfo);
                }
                else if (dict.ContainsKey(attributeName))
                {
                    styleInfo = dict[attributeName];
                    if (styleInfo != null && styleInfo.Taxis <= 0 && attributeNames.Contains(styleInfo.AttributeName))
                    {
                        styleInfoList.Add(styleInfo);
                    }
                }
            }

            foreach (var key in dict.Keys)
            {
                if (!attributeNames.Contains(key))
                {
                    styleInfoList.Add(dict[key]);
                }
            }

            return styleInfoList;
        }

        public static bool IsExistsInParents(List<int> relatedIdentities, string tableName, string attributeName)
        {
            var entries = GetAllTableStyleInfoPairs();
            for (var i = 1; i < relatedIdentities.Count - 1; i++)
            {
                var relatedIdentity = relatedIdentities[i];
                var startKey = GetCacheKeyStart(relatedIdentity, tableName);
                var keyArrayList = entries.GetKeys(startKey);
                foreach (string key in keyArrayList)
                {
                    var styleInfo = entries.GetValue(key) as TableStyleInfo;
                    if (styleInfo != null && styleInfo.AttributeName == attributeName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsChanged
        {
            set
            {
                lock (LockObject)
                {
                    if (value)
                    {
                        CacheUtils.Remove(CacheKey);
                    }
                    _async = value;
                }
            }
        }

        public static string GetCacheKey(int relatedIdentity, string tableName, string attributeName)
        {
            return $"{relatedIdentity}${tableName}${attributeName}".ToLower();
        }

        public static string GetCacheKeyStart(int relatedIdentity, string tableName)
        {
            return $"{relatedIdentity}${tableName}$".ToLower();
        }

        public static TableStyleInfo GetTableStyleInfoActually(ETableStyle tableStyle, string tableName, string attributeName, List<int> relatedIdentities)
        {
            relatedIdentities = GetRelatedIdentities(relatedIdentities);
            var styleInfo = GetTableStyleInfo(tableStyle, tableName, attributeName, relatedIdentities);
            if (styleInfo.RelatedIdentity != relatedIdentities[0])
            {
                styleInfo.TableStyleId = 0;
                styleInfo.RelatedIdentity = relatedIdentities[0];
            }
            return styleInfo;
        }

        //relatedIdentities从大到小，最后是0
        public static TableStyleInfo GetTableStyleInfo(ETableStyle tableStyle, string tableName, string attributeName, List<int> relatedIdentities)
        {
            TableStyleInfo styleInfo = null;
            if (attributeName == null) attributeName = string.Empty;

            relatedIdentities = GetRelatedIdentities(relatedIdentities);

            var entries = GetAllTableStyleInfoPairs();

            foreach (int relatedIdentity in relatedIdentities)
            {
                var key = GetCacheKey(relatedIdentity, tableName, attributeName);
                if (entries.ContainsKey(key))
                {
                    styleInfo = entries.GetValue(key) as TableStyleInfo;
                    if (styleInfo != null)
                    {
                        break;
                    }
                }
            }

            if (styleInfo == null)
            {
                if (tableStyle == ETableStyle.BackgroundContent)
                {
                    styleInfo = GetBackgroundContentTableStyleInfo(entries, tableName, attributeName);
                }
                else if (tableStyle == ETableStyle.Custom)
                {
                    styleInfo = GetCustomTableStyleInfo(tableName, attributeName);
                }
                else
                {
                    styleInfo = GetDefaultTableStyleInfo(tableName, attributeName);
                }
            }

            return styleInfo;
        }

        private static List<int> GetRelatedIdentities(List<int> arraylist)
        {
            var relatedIdentities = new List<int>();
            if (arraylist != null && arraylist.Count > 0)
            {
                relatedIdentities.AddRange(arraylist);
            }
            if (relatedIdentities.Count == 0)
            {
                relatedIdentities.Add(0);
            }
            else if (relatedIdentities[relatedIdentities.Count - 1] != 0)
            {
                relatedIdentities.Add(0);
            }
            return relatedIdentities;
        }

        public static int Insert(TableStyleInfo styleInfo, ETableStyle tableStyle)
        {
            var tableStyleId = BaiRongDataProvider.TableStyleDao.Insert(styleInfo, tableStyle);
            IsChanged = true;
            return tableStyleId;
        }

        public static void InsertWithTaxis(TableStyleInfo styleInfo, ETableStyle tableStyle)
        {
            BaiRongDataProvider.TableStyleDao.InsertWithTaxis(styleInfo, tableStyle);
            IsChanged = true;
        }

        public static void Update(TableStyleInfo info)
        {
            BaiRongDataProvider.TableStyleDao.Update(info);
            IsChanged = true;
        }

        public static void InsertOrUpdate(TableStyleInfo styleInfo, ETableStyle tableStyle)
        {
            if (styleInfo.TableStyleId == 0)
            {
                Insert(styleInfo, tableStyle);
            }
            else
            {
                Update(styleInfo);
            }
        }

        public static void DeleteAndInsertStyleItems(int tableStyleId, List<TableStyleItemInfo> styleItems)
        {
            if (styleItems == null || styleItems.Count <= 0) return;

            BaiRongDataProvider.TableStyleDao.DeleteStyleItems(tableStyleId);
            BaiRongDataProvider.TableStyleDao.InsertStyleItems(styleItems);
        }

        public static void Delete(int relatedIdentity, string tableName, string attributeName)
        {
            BaiRongDataProvider.TableStyleDao.Delete(relatedIdentity, tableName, attributeName);
            IsChanged = true;
        }

        public static bool IsExists(int relatedIdentity, string tableName, string attributeName)
        {
            var key = GetCacheKey(relatedIdentity, tableName, attributeName);
            var entries = GetAllTableStyleInfoPairs();
            return entries.Keys.Contains(key);
        }

        public static bool IsVisibleInList(TableStyleInfo styleInfo)
        {
            if (styleInfo.AttributeName == ContentAttribute.Title || styleInfo.IsVisibleInList == false)
            {
                return false;
            }
            return true;
        }

        public static DataSet GetStyleItemDataSet(int styleItemCount, List<TableStyleItemInfo> styleItemInfoArrayList)
        {
            var dataset = new DataSet();

            var dataTable = new DataTable("StyleItems");

            dataTable.Columns.Add(new DataColumn
            {
                DataType = System.Type.GetType("System.Int32"),
                ColumnName = "TableStyleItemID"
            });

            dataTable.Columns.Add(new DataColumn
            {
                DataType = System.Type.GetType("System.Int32"),
                ColumnName = "TableStyleID"
            });

            dataTable.Columns.Add(new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "ItemTitle"
            });

            dataTable.Columns.Add(new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "ItemValue"
            });

            dataTable.Columns.Add(new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "IsSelected"
            });

            for (var i = 0; i < styleItemCount; i++)
            {
                var dataRow = dataTable.NewRow();

                var itemInfo = styleItemInfoArrayList != null && styleItemInfoArrayList.Count > i ? styleItemInfoArrayList[i] : new TableStyleItemInfo();

                dataRow["TableStyleItemID"] = itemInfo.TableStyleItemId;
                dataRow["TableStyleID"] = itemInfo.TableStyleId;
                dataRow["ItemTitle"] = itemInfo.ItemTitle;
                dataRow["ItemValue"] = itemInfo.ItemValue;
                dataRow["IsSelected"] = itemInfo.IsSelected.ToString();

                dataTable.Rows.Add(dataRow);
            }

            dataset.Tables.Add(dataTable);
            return dataset;
        }

        public static Dictionary<string, List<TableStyleInfo>> GetTableStyleInfoWithItemsDictinary(string tableName, List<int> allRelatedIdentities)
        {
            var dict = new Dictionary<string, List<TableStyleInfo>>();

            var entries = GetAllTableStyleInfoPairs();
            foreach (var key in entries.Keys)
            {
                var identityFromKey = TranslateUtils.ToInt(key.Split('$')[0]);
                var tableNameFromKey = key.Split('$')[1];
                if (!StringUtils.EqualsIgnoreCase(tableNameFromKey, tableName) ||
                    !allRelatedIdentities.Contains(identityFromKey)) continue;

                var styleInfo = (TableStyleInfo)entries.GetValue(key);
                if (InputTypeUtils.IsWithStyleItems(InputTypeUtils.GetEnumType(styleInfo.InputType)))
                {
                    styleInfo.StyleItems = BaiRongDataProvider.TableStyleDao.GetStyleItemInfoList(styleInfo.TableStyleId);
                }
                var tableStyleInfoWithItemList = dict.ContainsKey(styleInfo.AttributeName) ? dict[styleInfo.AttributeName] : new List<TableStyleInfo>();
                tableStyleInfoWithItemList.Add(styleInfo);
                dict[styleInfo.AttributeName] = tableStyleInfoWithItemList;
            }

            return dict;
        }

        public const string Current = "{Current}";

        public static TableStyleInfo GetBackgroundContentTableStyleInfo(PairList entries, string tableName, string attributeName)
        {
            var lowerAttributeName = attributeName.ToLower();
            if (ContentAttribute.HiddenAttributes.Contains(lowerAttributeName))
            {
                return GetContentHiddenTableStyleInfo(tableName, attributeName);
            }
            var styleInfo = new TableStyleInfo(0, 0, tableName, attributeName, 0, attributeName, string.Empty, true, false, true, InputTypeUtils.GetValue(InputType.Text), string.Empty, true, string.Empty);

            if (lowerAttributeName == BackgroundContentAttribute.SubTitle.ToLower())
            {
                styleInfo.DisplayName = "副标题";
            }
            else if (lowerAttributeName == BackgroundContentAttribute.ImageUrl.ToLower())
            {
                styleInfo.DisplayName = "图片";
                styleInfo.InputType = InputTypeUtils.GetValue(InputType.Image);
            }
            else if (lowerAttributeName == BackgroundContentAttribute.VideoUrl.ToLower())
            {
                styleInfo.DisplayName = "视频";
                styleInfo.InputType = InputTypeUtils.GetValue(InputType.Video);
            }
            else if (lowerAttributeName == BackgroundContentAttribute.FileUrl.ToLower())
            {
                styleInfo.DisplayName = "附件";
                styleInfo.InputType = InputTypeUtils.GetValue(InputType.File);
            }
            else if (lowerAttributeName == BackgroundContentAttribute.LinkUrl.ToLower())
            {
                styleInfo.DisplayName = "外部链接";
                styleInfo.HelpText = "设置后链接将指向此地址";
            }
            else if (lowerAttributeName == BackgroundContentAttribute.Content.ToLower())
            {
                styleInfo.DisplayName = "内容";
                styleInfo.InputType = InputTypeUtils.GetValue(InputType.TextEditor);
            }
            else if (lowerAttributeName == BackgroundContentAttribute.Author.ToLower())
            {
                styleInfo.DisplayName = "作者";
            }
            else if (lowerAttributeName == BackgroundContentAttribute.Source.ToLower())
            {
                styleInfo.DisplayName = "来源";
            }
            else if (lowerAttributeName == BackgroundContentAttribute.Summary.ToLower())
            {
                styleInfo.DisplayName = "内容摘要";
                styleInfo.InputType = InputTypeUtils.GetValue(InputType.TextArea);
            }
            else if (!string.IsNullOrEmpty(attributeName))
            {
                TableStyleInfo tableStyleInfo = null;
                foreach (var key in entries.Keys)
                {
                    if (key == GetCacheKey(0, tableName, attributeName))
                    {
                        tableStyleInfo = (TableStyleInfo)entries.GetValue(key);
                        break;
                    }
                }
                //var tableStyleInfo = BaiRongDataProvider.TableStyleDao.GetTableStyleInfo(0, tableName, attributeName);
                if (tableStyleInfo != null)
                {
                    //styleInfo.DisplayName = attributeName;
                    //styleInfo.InputType = tableStyleInfo.InputType;
                    styleInfo = tableStyleInfo;
                }
            }

            return styleInfo;
        }

        public static TableStyleInfo GetCustomTableStyleInfo(string tableName, string attributeName)
        {
            var lowerAttributeName = attributeName.ToLower();
            if (ContentAttribute.HiddenAttributes.Contains(lowerAttributeName))
            {
                return GetContentHiddenTableStyleInfo(tableName, attributeName);
            }
            var styleInfo = new TableStyleInfo(0, 0, tableName, attributeName, 0, attributeName, string.Empty, true, false, true, InputTypeUtils.GetValue(InputType.Text), string.Empty, true, string.Empty);

            if (lowerAttributeName == ContentAttribute.Title.ToLower())
            {
                styleInfo.DisplayName = "标题";
                styleInfo.HelpText = "标题的名称";
                styleInfo.InputType = InputTypeUtils.GetValue(InputType.Text);
                styleInfo.Additional.IsFormatString = true;
                styleInfo.Additional.IsValidate = true;
                styleInfo.Additional.IsRequired = true;
            }
            else if (lowerAttributeName == ContentAttribute.IsTop.ToLower())
            {
                styleInfo.DisplayName = "置顶";
                styleInfo.HelpText = "是否为置顶内容";
                styleInfo.InputType = InputTypeUtils.GetValue(InputType.CheckBox);
            }
            else if (lowerAttributeName == ContentAttribute.AddDate.ToLower())
            {
                styleInfo.DisplayName = "添加日期";
                styleInfo.HelpText = "内容的添加日期";
                styleInfo.InputType = InputTypeUtils.GetValue(InputType.DateTime);
                styleInfo.DefaultValue = Current;
            }

            return styleInfo;
        }

        private static TableStyleInfo GetContentHiddenTableStyleInfo(string tableName, string attributeName)
        {
            var styleInfo = new TableStyleInfo(0, 0, tableName, attributeName, 0, attributeName, string.Empty, true, false, true, InputTypeUtils.GetValue(InputType.Text), string.Empty, true, string.Empty);
            switch (attributeName)
            {
                case ContentAttribute.Id:
                    styleInfo.DisplayName = "内容ID";
                    styleInfo.HelpText = "内容ID";
                    break;
                case ContentAttribute.NodeId:
                    styleInfo.DisplayName = "栏目ID";
                    styleInfo.HelpText = "栏目ID";
                    break;
                case ContentAttribute.PublishmentSystemId:
                    styleInfo.DisplayName = "站点ID";
                    styleInfo.HelpText = "站点ID";
                    break;
                case ContentAttribute.AddUserName:
                    styleInfo.DisplayName = "添加者";
                    styleInfo.HelpText = "添加者";
                    break;
                case ContentAttribute.LastEditUserName:
                    styleInfo.DisplayName = "最后修改者";
                    styleInfo.HelpText = "最后修改者";
                    break;
                case ContentAttribute.LastEditDate:
                    styleInfo.DisplayName = "最后修改时间";
                    styleInfo.HelpText = "最后修改时间";
                    break;
                case ContentAttribute.Taxis:
                    styleInfo.DisplayName = "排序";
                    styleInfo.HelpText = "排序";
                    break;
                case ContentAttribute.ContentGroupNameCollection:
                    styleInfo.DisplayName = "内容组";
                    styleInfo.HelpText = "内容组";
                    break;
                case ContentAttribute.Tags:
                    styleInfo.DisplayName = "标签";
                    styleInfo.HelpText = "标签";
                    break;
                case ContentAttribute.IsChecked:
                    styleInfo.DisplayName = "是否审核通过";
                    styleInfo.HelpText = "是否审核通过";
                    break;
                case ContentAttribute.CheckedLevel:
                    styleInfo.DisplayName = "审核级别";
                    styleInfo.HelpText = "审核级别";
                    break;
                case ContentAttribute.Comments:
                    styleInfo.DisplayName = "评论数";
                    styleInfo.HelpText = "评论数";
                    break;
                case ContentAttribute.Photos:
                    styleInfo.DisplayName = "图片数";
                    styleInfo.HelpText = "图片数";
                    break;
                case ContentAttribute.Hits:
                    styleInfo.DisplayName = "点击量";
                    styleInfo.HelpText = "点击量";
                    break;
                case ContentAttribute.HitsByDay:
                    styleInfo.DisplayName = "日点击量";
                    styleInfo.HelpText = "日点击量";
                    break;
                case ContentAttribute.HitsByWeek:
                    styleInfo.DisplayName = "周点击量";
                    styleInfo.HelpText = "周点击量";
                    break;
                case ContentAttribute.HitsByMonth:
                    styleInfo.DisplayName = "月点击量";
                    styleInfo.HelpText = "月点击量";
                    break;
                case ContentAttribute.LastHitsDate:
                    styleInfo.DisplayName = "最后点击时间";
                    styleInfo.HelpText = "最后点击时间";
                    break;
            }
            return styleInfo;
        }

        public static TableStyleInfo GetDefaultTableStyleInfo(string tableName, string attributeName)
        {
            return new TableStyleInfo(0, 0, tableName, attributeName, 0, attributeName, string.Empty, true, false, true, InputTypeUtils.GetValue(InputType.Text), string.Empty, true, string.Empty);
        }

        public static bool IsMetadata(ETableStyle tableStyle, string attributeName)
        {
            var retval = false;
            if (tableStyle == ETableStyle.BackgroundContent)
            {
                retval = BackgroundContentAttribute.AllAttributes.Contains(attributeName.ToLower());
            }
            else if (tableStyle == ETableStyle.Channel)
            {
                retval = ChannelAttribute.HiddenAttributes.Contains(attributeName.ToLower());
            }
            else if (tableStyle == ETableStyle.InputContent)
            {
                retval = InputContentAttribute.AllAttributes.Contains(attributeName.ToLower());
            }
            return retval;
        }
    }

}
