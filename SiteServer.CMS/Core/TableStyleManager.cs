using System.Collections.Generic;
using System.Text;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Core
{
    public static class TableStyleManager
    {
        private static readonly object LockObject = new object();
        private static bool _async = true;//缓存与数据库不同步
        private const string CacheKey = "BaiRong.Core.Table.TableStyleManager";

        public static PairList GetAllTableStyleInfoPairs()
        {
            lock (LockObject)
            {
                if (!_async && CacheUtils.Get(CacheKey) != null) return (PairList) CacheUtils.Get(CacheKey);

                var entries = DataProvider.TableStyleDao.GetAllTableStyleInfoPairs();
                CacheUtils.Insert(CacheKey, entries);
                _async = false;
                return entries;
            }
        }

        public static List<TableStyleInfo> GetTableStyleInfoList(string tableName, List<int> relatedIdentities)
        {
            var allAttributeNames = new List<string>();
            var styleInfoList = new List<TableStyleInfo>();

            var entries = GetAllTableStyleInfoPairs();
            relatedIdentities = GetRelatedIdentities(relatedIdentities);
            foreach (var relatedIdentity in relatedIdentities)
            {
                var startKey = GetCacheKeyStart(relatedIdentity, tableName);
                var keyList = entries.GetKeys(startKey);
                foreach (var key in keyList)
                {
                    var styleInfo = entries.GetValue(key) as TableStyleInfo;
                    if (styleInfo == null) continue;

                    if (!allAttributeNames.Contains(styleInfo.AttributeName.ToLower()))
                    {
                        allAttributeNames.Add(styleInfo.AttributeName.ToLower());
                        styleInfoList.Add(styleInfo);
                    }
                }
            }

            var attributeNames = TableMetadataManager.GetAttributeNameList(tableName);
            foreach (var attributeName in attributeNames)
            {
                if (!allAttributeNames.Contains(attributeName.ToLower()))
                {
                    allAttributeNames.Add(attributeName.ToLower());

                    styleInfoList.Add(GetDefaultTableStyleInfo(tableName, attributeName));
                }
            }
            
            styleInfoList.Sort();

            return styleInfoList;
        }

        public static IAttributes GetDefaultAttributes(List<TableStyleInfo> styleInfoList)
        {
            var attributes = new ExtendedAttributes();

            foreach (var styleInfo in styleInfoList)
            {
                var defaultValue = string.Empty;
                if (!string.IsNullOrEmpty(styleInfo.DefaultValue))
                {
                    defaultValue = styleInfo.DefaultValue;
                }
                else if (styleInfo.StyleItems != null)
                {
                    var defaultValues = new List<string>();
                    foreach (var styleItem in styleInfo.StyleItems)
                    {
                        if (styleItem.IsSelected)
                        {
                            defaultValues.Add(styleItem.ItemValue);
                        }
                    }
                    if (defaultValues.Count > 0)
                    {
                        defaultValue = TranslateUtils.ObjectCollectionToString(defaultValues);
                    }
                }

                if (!string.IsNullOrEmpty(defaultValue))
                {
                    attributes.Set(styleInfo.AttributeName, defaultValue);
                }
            }

            return attributes;
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

        //relatedIdentities从大到小，最后是0
        public static TableStyleInfo GetTableStyleInfo(string tableName, string attributeName, List<int> relatedIdentities)
        {
            TableStyleInfo styleInfo = null;
            if (attributeName == null) attributeName = string.Empty;

            relatedIdentities = GetRelatedIdentities(relatedIdentities);

            var entries = GetAllTableStyleInfoPairs();

            foreach (var relatedIdentity in relatedIdentities)
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

            return styleInfo ?? GetDefaultTableStyleInfo(tableName, attributeName);
        }

        private static List<int> GetRelatedIdentities(IReadOnlyCollection<int> list)
        {
            var relatedIdentities = new List<int>();

            if (list != null && list.Count > 0)
            {
                foreach (var i in list)
                {
                    if (!relatedIdentities.Contains(i))
                    {
                        relatedIdentities.Add(i);
                    }
                }
            }

            relatedIdentities.Sort();
            relatedIdentities.Reverse();

            if (!relatedIdentities.Contains(0))
            {
                relatedIdentities.Add(0);
            }

            return relatedIdentities;
        }

        public static int Insert(TableStyleInfo styleInfo)
        {
            var tableStyleId = DataProvider.TableStyleDao.Insert(styleInfo);
            IsChanged = true;
            return tableStyleId;
        }

        public static void Update(TableStyleInfo info)
        {
            DataProvider.TableStyleDao.Update(info);
            IsChanged = true;
        }

        public static void DeleteAndInsertStyleItems(int tableStyleId, List<TableStyleItemInfo> styleItems)
        {
            if (styleItems == null || styleItems.Count <= 0) return;

            DataProvider.TableStyleItemDao.DeleteStyleItems(tableStyleId);
            DataProvider.TableStyleItemDao.InsertStyleItems(styleItems);
        }

        public static void Delete(int relatedIdentity, string tableName, string attributeName)
        {
            DataProvider.TableStyleDao.Delete(relatedIdentity, tableName, attributeName);
            IsChanged = true;
        }

        public static bool IsExists(int relatedIdentity, string tableName, string attributeName)
        {
            var key = GetCacheKey(relatedIdentity, tableName, attributeName);
            var entries = GetAllTableStyleInfoPairs();
            return entries.Keys.Contains(key);
        }

        //public static DataSet GetStyleItemDataSet(int styleItemCount, List<TableStyleItemInfo> styleItemInfoList)
        //{
        //    var dataset = new DataSet();

        //    var dataTable = new DataTable("StyleItems");

        //    dataTable.Columns.Add(new DataColumn
        //    {
        //        DataType = System.Type.GetType("System.Int32"),
        //        AttributeName = "TableStyleItemID"
        //    });

        //    dataTable.Columns.Add(new DataColumn
        //    {
        //        DataType = System.Type.GetType("System.Int32"),
        //        AttributeName = "TableStyleID"
        //    });

        //    dataTable.Columns.Add(new DataColumn
        //    {
        //        DataType = System.Type.GetType("System.String"),
        //        AttributeName = "ItemTitle"
        //    });

        //    dataTable.Columns.Add(new DataColumn
        //    {
        //        DataType = System.Type.GetType("System.String"),
        //        AttributeName = "ItemValue"
        //    });

        //    dataTable.Columns.Add(new DataColumn
        //    {
        //        DataType = System.Type.GetType("System.String"),
        //        AttributeName = "IsSelected"
        //    });

        //    for (var i = 0; i < styleItemCount; i++)
        //    {
        //        var dataRow = dataTable.NewRow();

        //        var itemInfo = styleItemInfoList != null && styleItemInfoList.Count > i ? styleItemInfoList[i] : new TableStyleItemInfo();

        //        dataRow["TableStyleItemID"] = itemInfo.TableStyleItemId;
        //        dataRow["TableStyleID"] = itemInfo.TableStyleId;
        //        dataRow["ItemTitle"] = itemInfo.ItemTitle;
        //        dataRow["ItemValue"] = itemInfo.ItemValue;
        //        dataRow["IsSelected"] = itemInfo.IsSelected.ToString();

        //        dataTable.Rows.Add(dataRow);
        //    }

        //    dataset.Tables.Add(dataTable);
        //    return dataset;
        //}

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
                if (InputTypeUtils.IsWithStyleItems(styleInfo.InputType))
                {
                    styleInfo.StyleItems = DataProvider.TableStyleItemDao.GetStyleItemInfoList(styleInfo.Id);
                }
                var tableStyleInfoWithItemList = dict.ContainsKey(styleInfo.AttributeName) ? dict[styleInfo.AttributeName] : new List<TableStyleInfo>();
                tableStyleInfoWithItemList.Add(styleInfo);
                dict[styleInfo.AttributeName] = tableStyleInfoWithItemList;
            }

            return dict;
        }

        public static string GetValidateInfo(TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();
            if (styleInfo.Additional.IsRequired)
            {
                builder.Append("必填项;");
            }
            if (styleInfo.Additional.MinNum > 0)
            {
                builder.Append($"最少{styleInfo.Additional.MinNum}个字符;");
            }
            if (styleInfo.Additional.MaxNum > 0)
            {
                builder.Append($"最多{styleInfo.Additional.MaxNum}个字符;");
            }
            if (styleInfo.Additional.ValidateType != ValidateType.None)
            {
                builder.Append($"验证:{ValidateTypeUtils.GetText(styleInfo.Additional.ValidateType)};");
            }

            if (builder.Length > 0)
            {
                builder.Length = builder.Length - 1;
            }
            else
            {
                builder.Append("无验证");
            }
            return builder.ToString();
        }

        public static TableStyleInfo GetDefaultTableStyleInfo(string tableName, string attributeName)
        {
            var lowerAttributeName = attributeName.ToLower();
            var styleInfo = new TableStyleInfo(0, 0, tableName, attributeName, 0, attributeName, string.Empty, false, InputType.Text, string.Empty, true, string.Empty);

            if (lowerAttributeName == BackgroundContentAttribute.SubTitle.ToLower())
            {
                styleInfo.DisplayName = "副标题";
                styleInfo.Taxis = 1;
            }
            else if (lowerAttributeName == BackgroundContentAttribute.ImageUrl.ToLower())
            {
                styleInfo.DisplayName = "图片";
                styleInfo.InputType = InputType.Image;
                styleInfo.Taxis = 2;
            }
            else if (lowerAttributeName == BackgroundContentAttribute.VideoUrl.ToLower())
            {
                styleInfo.DisplayName = "视频";
                styleInfo.InputType = InputType.Video;
                styleInfo.Taxis = 3;
            }
            else if (lowerAttributeName == BackgroundContentAttribute.FileUrl.ToLower())
            {
                styleInfo.DisplayName = "附件";
                styleInfo.InputType = InputType.File;
                styleInfo.Taxis = 4;
            }
            else if (lowerAttributeName == BackgroundContentAttribute.Content.ToLower())
            {
                styleInfo.DisplayName = "内容";
                styleInfo.InputType = InputType.TextEditor;
                styleInfo.Taxis = 5;
            }
            else if (lowerAttributeName == BackgroundContentAttribute.Summary.ToLower())
            {
                styleInfo.DisplayName = "内容摘要";
                styleInfo.InputType = InputType.TextArea;
                styleInfo.Taxis = 6;
            }
            else if (lowerAttributeName == BackgroundContentAttribute.Author.ToLower())
            {
                styleInfo.DisplayName = "作者";
                styleInfo.Taxis = 7;
            }
            else if (lowerAttributeName == BackgroundContentAttribute.Source.ToLower())
            {
                styleInfo.DisplayName = "来源";
                styleInfo.Taxis = 8;
            }

            return styleInfo;
        }
    }

}
