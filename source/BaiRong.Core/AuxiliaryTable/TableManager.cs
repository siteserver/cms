using System.Collections;
using System.Collections.Generic;
using System.Text;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.AuxiliaryTable
{
    public sealed class TableManager
    {
        private static readonly object LockObject = new object();
        private static bool _async = true;//缓存与数据库不同步
        private const string CacheKey = "BaiRong.Core.AuxiliaryTable.TableManager";

        /// <summary>
        /// 得到辅助表tableName数据库中的字段名称的集合
        /// </summary>
        public static List<string> GetAttributeNameList(ETableStyle tableStyle, string tableName, bool toLower)
        {
            var attributeNameList = new List<string>();
            if (ETableStyleUtils.IsContent(tableStyle))
            {
                var tableMetadataInfoList = GetTableMetadataInfoList(tableName);
                foreach (var tableMetadataInfo in tableMetadataInfoList)
                {
                    attributeNameList.Add(tableMetadataInfo.AttributeName);
                }
            } 

            if (attributeNameList.Count > 0 && toLower)
            {
                var lowerList = new List<string>();
                foreach (var attributeName in attributeNameList)
                {
                    lowerList.Add(attributeName.ToLower());
                }
                return lowerList;
            }

            return attributeNameList;
        }

        /// <summary>
        /// 得到辅助表的所有字段名称的集合
        /// </summary>
        public static List<string> GetAttributeNameList(ETableStyle tableStyle, string tableName)
        {
            return GetAttributeNameList(tableStyle, tableName, false);
        }


        //获取辅助表隐藏字段名称集合
        public static List<string> GetHiddenAttributeNameList(ETableStyle tableStyle)
        {
            if (ETableStyleUtils.IsContent(tableStyle))
            {
                return ContentAttribute.HiddenAttributes;
            }
            if (tableStyle == ETableStyle.Channel)
            {
                return ChannelAttribute.HiddenAttributes;
            }
            if (tableStyle == ETableStyle.InputContent)
            {
                return InputContentAttribute.HiddenAttributes;
            }
            return new List<string>();
        }

        public static List<string> GetExcludeAttributeNames(ETableStyle tableStyle)
        {
            if (tableStyle == ETableStyle.BackgroundContent)
            {
                return BackgroundContentAttribute.ExcludeAttributes;
            }
            if (tableStyle == ETableStyle.GovPublicContent)
            {
                return GovPublicContentAttribute.ExcludeAttributes;
            }
            if (tableStyle == ETableStyle.GovInteractContent)
            {
                return GovInteractContentAttribute.ExcludeAttributes;
            }
            if (tableStyle == ETableStyle.VoteContent)
            {
                return VoteContentAttribute.ExcludeAttributes;
            }
            if (tableStyle == ETableStyle.JobContent)
            {
                return JobContentAttribute.ExcludeAttributes;
            }
            if (tableStyle == ETableStyle.UserDefined)
            {
                return ContentAttribute.ExcludeAttributes;
            }
            return new List<string>();
        }

        public static bool IsAttributeNameExists(ETableStyle tableStyle, string tableName, string attributeName)
        {
            var list = GetAttributeNameList(tableStyle, tableName, true);
            return list.Contains(attributeName.ToLower());
        }

        /// <summary>
        /// 得到辅助表tableName的所有元数据的集合
        /// </summary>
        public static List<TableMetadataInfo> GetTableMetadataInfoList(string tableName)
        {
            var metadataList = new List<TableMetadataInfo>();

            var tableEnNameAndTableMetadataInfoListHashtable = GetTableEnNameAndTableMetadataInfoListHashtable();
            if (tableEnNameAndTableMetadataInfoListHashtable?[tableName] != null)
            {
                var additionMetadataList = (List<TableMetadataInfo>)tableEnNameAndTableMetadataInfoListHashtable[tableName];
                if (additionMetadataList != null)
                {
                    foreach (var metadataInfo in additionMetadataList)
                    {
                        var contains = false;
                        foreach (var info in metadataList)
                        {
                            if (StringUtils.EqualsIgnoreCase(info.AttributeName, metadataInfo.AttributeName))
                            {
                                contains = true;
                                break;
                            }
                        }
                        if (!contains)
                        {
                            metadataList.Add(metadataInfo);
                        }
                    }
                }
            }

            return metadataList;
        }


        public static Hashtable GetTableEnNameAndTableMetadataInfoListHashtable()
        {
            lock (LockObject)
            {
                if (_async || CacheUtils.Get(CacheKey) == null)
                {
                    var tableHashtable = BaiRongDataProvider.TableMetadataDao.GetTableEnNameAndTableMetadataInfoListHashtable();
                    CacheUtils.Max(CacheKey, tableHashtable);
                    _async = false;
                    return tableHashtable;
                }
                return (Hashtable)CacheUtils.Get(CacheKey);
            }
        }

        public static TableMetadataInfo GetTableMetadataInfo(string tableName, string attributeName)
        {
            var list = GetTableMetadataInfoList(tableName);
            foreach (var tableMetadataInfo in list)
            {
                if (StringUtils.EqualsIgnoreCase(tableMetadataInfo.AttributeName, attributeName))
                {
                    return tableMetadataInfo;
                }
            }
            return null;
        }

        public static string GetTableMetadataDataType(string tableName, string attributeName)
        {
            var metadataInfo = GetTableMetadataInfo(tableName, attributeName);
            if (metadataInfo != null)
            {
                return EDataTypeUtils.GetTextByAuxiliaryTable(metadataInfo.DataType, metadataInfo.DataLength);
            }
            return string.Empty;
        }

        public static int GetTableMetadataId(string tableName, string attributeName)
        {
            var metadataId = 0;
            var list = GetTableMetadataInfoList(tableName);
            foreach (var tableMetadataInfo in list)
            {
                if (StringUtils.EqualsIgnoreCase(tableMetadataInfo.AttributeName, attributeName))
                {
                    metadataId = tableMetadataInfo.TableMetadataId;
                    break;
                }
            }
            return metadataId;
        }

        public static bool IsChanged
        {
            set
            {
                lock (LockObject)
                {
                    if (value)
                    {
                        Data.SqlUtils.Cache_RemoveTableColumnInfoListCache();
                    }
                    _async = value;
                }
            }
        }

        public static string GetSerializedString(string tableName)
        {
            var builder = new StringBuilder();
            var list = GetTableMetadataInfoList(tableName);
            var sortedlist = new SortedList();
            foreach (var metadataInfo in list)
            {
                if (metadataInfo.IsSystem == false)
                {
                    /*
                     * AttributeName,
                     * DataType,
                     * DataLength,
                     * CanBeNull,
                     * DBDefaultValue
                     * */
                    string serialize =
                        $"AttributeName:{metadataInfo.AttributeName};DataType:{EDataTypeUtils.GetValue(metadataInfo.DataType)};DataLength={metadataInfo.DataLength}";
                    sortedlist.Add(metadataInfo.AttributeName, serialize);
                }
            }

            foreach (string attributeName in sortedlist.Keys)
            {
                builder.Append(sortedlist[attributeName]);
            }

            return builder.ToString();
        }

        public static string GetTableNameOfArchive(string tableName)
        {
            return tableName + "_Archive";
        }
    }

}
