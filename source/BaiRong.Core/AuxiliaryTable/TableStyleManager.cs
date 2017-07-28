using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core.Collection;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;

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
                    CacheUtils.Max(CacheKey, entries);
                    _async = false;
                    return entries;
                }
                return (PairList)CacheUtils.Get(CacheKey);
            }
        }

        public static List<TableStyleInfo> GetTableStyleInfoList(ETableStyle tableStyle, string tableName, List<int> relatedIdentities)
        {
            relatedIdentities = GetRelatedIdentities(relatedIdentities);

            var sortedlist = new Dictionary<string, TableStyleInfo>();
            var allAttributeNames = new ArrayList();
            var arraylist = new List<TableStyleInfo>();

            var entries = GetAllTableStyleInfoPairs();
            var attributeNames = TableManager.GetAttributeNameList(tableStyle, tableName);
            
            //添加所有实际保存在数据库中的样式
            var i = 99;
            foreach (var relatedIdentity in relatedIdentities)
            {
                //if (ETableStyleUtils.IsContent(tableStyle)) continue;
                var startKey = GetCacheKeyStart(relatedIdentity, tableName);
                var keyArrayList = entries.GetKeys(startKey);
                foreach (string key in keyArrayList)
                {
                    var styleInfo = entries.GetValue(key) as TableStyleInfo;
                    if (styleInfo == null || allAttributeNames.Contains(styleInfo.AttributeName)) continue;

                    allAttributeNames.Add(styleInfo.AttributeName);
                    if (styleInfo.Taxis <= 0 && attributeNames.Contains(styleInfo.AttributeName))//数据库字段
                    {
                        sortedlist.Add(styleInfo.AttributeName, styleInfo);
                    }
                    else if (styleInfo.Taxis > 0)                       //排序字段
                    {
                        var iStr = relatedIdentity + styleInfo.Taxis.ToString().PadLeft(3, '0');
                        sortedlist.Add("1" + iStr + "_" + styleInfo.AttributeName, styleInfo);
                    }
                    else                                                //未排序字段
                    {
                        var iStr = relatedIdentity + i.ToString().PadLeft(3, '0');
                        sortedlist.Add("0" + iStr + "_" + styleInfo.AttributeName, styleInfo);
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
                        styleInfo = GetBackgroundContentTableStyleInfo(tableName, attributeName);
                    }
                    else if (tableStyle == ETableStyle.GovPublicContent)
                    {
                        styleInfo = GetGovPublicContentTableStyleInfo(tableName, attributeName);
                    }
                    else if (tableStyle == ETableStyle.GovInteractContent)
                    {
                        styleInfo = GetGovInteractContentTableStyleInfo(tableName, attributeName);
                    }
                    else if (tableStyle == ETableStyle.VoteContent)
                    {
                        styleInfo = GetVoteContentTableStyleInfo(tableName, attributeName);
                    }
                    else if (tableStyle == ETableStyle.JobContent)
                    {
                        styleInfo = GetJobContentTableStyleInfo(tableName, attributeName);
                    }
                    else if (tableStyle == ETableStyle.UserDefined)
                    {
                        styleInfo = GetUserDefinedTableStyleInfo(tableName, attributeName);
                    }
                    else
                    {
                        styleInfo = GetDefaultTableStyleInfo(tableName, attributeName);
                    }

                    arraylist.Add(styleInfo);
                }
                else if (sortedlist.ContainsKey(attributeName))
                {
                    styleInfo = sortedlist[attributeName];
                    if (styleInfo != null && styleInfo.Taxis <= 0 && attributeNames.Contains(styleInfo.AttributeName))
                    {
                        arraylist.Add(styleInfo);
                    }
                }
            }

            foreach (var key in sortedlist.Keys)
            {
                if (!attributeNames.Contains(key))
                {
                    arraylist.Add(sortedlist[key]);
                }
            }

            return arraylist;
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
                    styleInfo = GetBackgroundContentTableStyleInfo(tableName, attributeName);
                }
                else if (tableStyle == ETableStyle.GovPublicContent)
                {
                    styleInfo = GetGovPublicContentTableStyleInfo(tableName, attributeName);
                }
                else if (tableStyle == ETableStyle.GovInteractContent)
                {
                    styleInfo = GetGovInteractContentTableStyleInfo(tableName, attributeName);
                }
                else if (tableStyle == ETableStyle.VoteContent)
                {
                    styleInfo = GetVoteContentTableStyleInfo(tableName, attributeName);
                }
                else if (tableStyle == ETableStyle.JobContent)
                {
                    styleInfo = GetJobContentTableStyleInfo(tableName, attributeName);
                }
                else if (tableStyle == ETableStyle.UserDefined)
                {
                    styleInfo = GetUserDefinedTableStyleInfo(tableName, attributeName);
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

        public static void DeleteAndInsertStyleItems(int tableStyleId, ArrayList styleItems)
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
                if (EInputTypeUtils.IsWithStyleItems(EInputTypeUtils.GetEnumType(styleInfo.InputType)))
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

        public static TableStyleInfo GetBackgroundContentTableStyleInfo(string tableName, string attributeName)
        {
            var lowerAttributeName = attributeName.ToLower();
            if (ContentAttribute.HiddenAttributes.Contains(lowerAttributeName))
            {
                return GetContentHiddenTableStyleInfo(tableName, attributeName);
            }
            var styleInfo = new TableStyleInfo(0, 0, tableName, attributeName, 0, attributeName, string.Empty, true, false, true, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, true, string.Empty);

            if (lowerAttributeName == ContentAttribute.Title.ToLower())
            {
                styleInfo.DisplayName = "标题";
                styleInfo.Additional.IsValidate = true;
                styleInfo.Additional.IsRequired = true;
                styleInfo.Additional.IsFormatString = true;
            }
            else if (lowerAttributeName == BackgroundContentAttribute.SubTitle.ToLower())
            {
                styleInfo.DisplayName = "副标题";
            }
            else if (lowerAttributeName == BackgroundContentAttribute.ImageUrl.ToLower())
            {
                styleInfo.DisplayName = "图片";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.Image);
            }
            else if (lowerAttributeName == BackgroundContentAttribute.VideoUrl.ToLower())
            {
                styleInfo.DisplayName = "视频";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.Video);
            }
            else if (lowerAttributeName == BackgroundContentAttribute.FileUrl.ToLower())
            {
                styleInfo.DisplayName = "附件";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.File);
            }
            else if (lowerAttributeName == BackgroundContentAttribute.LinkUrl.ToLower())
            {
                styleInfo.DisplayName = "外部链接";
                styleInfo.HelpText = "设置后链接将指向此地址";
            }
            else if (lowerAttributeName == BackgroundContentAttribute.Content.ToLower())
            {
                styleInfo.DisplayName = "内容";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.TextEditor);
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
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.TextArea);
            }
            else if (lowerAttributeName == BackgroundContentAttribute.IsRecommend.ToLower())
            {
                styleInfo.DisplayName = "推荐";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.CheckBox);
            }
            else if (lowerAttributeName == BackgroundContentAttribute.IsHot.ToLower())
            {
                styleInfo.DisplayName = "热点";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.CheckBox);
            }
            else if (lowerAttributeName == BackgroundContentAttribute.IsColor.ToLower())
            {
                styleInfo.DisplayName = "醒目";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.CheckBox);
            }
            else if (lowerAttributeName == ContentAttribute.IsTop.ToLower())
            {
                styleInfo.DisplayName = "置顶";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.CheckBox);
            }
            else if (lowerAttributeName == ContentAttribute.AddDate.ToLower())
            {
                styleInfo.DisplayName = "添加日期";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.DateTime);
                styleInfo.DefaultValue = Current;
            }
            else if (!string.IsNullOrEmpty(attributeName))
            {
                var tableStyleInfo = BaiRongDataProvider.TableStyleDao.GetTableStyleInfo(0, tableName, attributeName);
                if (tableStyleInfo != null)
                {
                    //styleInfo.DisplayName = attributeName;
                    //styleInfo.InputType = tableStyleInfo.InputType;
                    styleInfo = tableStyleInfo;
                }
            }

            return styleInfo;
        }

        public static TableStyleInfo GetGovPublicContentTableStyleInfo(string tableName, string attributeName)
        {
            var lowerAttributeName = attributeName.ToLower();
            if (ContentAttribute.HiddenAttributes.Contains(lowerAttributeName))
            {
                return GetContentHiddenTableStyleInfo(tableName, attributeName);
            }
            var styleInfo = new TableStyleInfo(0, 0, tableName, attributeName, 0, attributeName, string.Empty, true, false, true, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, true, string.Empty);

            if (lowerAttributeName == ContentAttribute.Title.ToLower())
            {
                styleInfo.DisplayName = "标题";
                styleInfo.Additional.IsValidate = true;
                styleInfo.Additional.IsRequired = true;
                styleInfo.Additional.IsFormatString = true;
            }
            else if (lowerAttributeName == GovPublicContentAttribute.Identifier.ToLower())
            {
                styleInfo.DisplayName = "索引号";
            }
            else if (lowerAttributeName == GovPublicContentAttribute.Description.ToLower())
            {
                styleInfo.DisplayName = "内容概述";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.TextArea);
            }
            else if (lowerAttributeName == GovPublicContentAttribute.PublishDate.ToLower())
            {
                styleInfo.DisplayName = "发文日期";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.Date);
            }
            else if (lowerAttributeName == GovPublicContentAttribute.EffectDate.ToLower())
            {
                styleInfo.DisplayName = "生效日期";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.Date);
            }
            else if (lowerAttributeName == GovPublicContentAttribute.IsAbolition.ToLower())
            {
                styleInfo.DisplayName = "是否废止";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.CheckBox);
            }
            else if (lowerAttributeName == GovPublicContentAttribute.AbolitionDate.ToLower())
            {
                styleInfo.DisplayName = "废止日期";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.Date);
            }
            else if (lowerAttributeName == GovPublicContentAttribute.DocumentNo.ToLower())
            {
                styleInfo.DisplayName = "文号";
            }
            else if (lowerAttributeName == GovPublicContentAttribute.Publisher.ToLower())
            {
                styleInfo.DisplayName = "发布机构";
            }
            else if (lowerAttributeName == GovPublicContentAttribute.Keywords.ToLower())
            {
                styleInfo.DisplayName = "关键词";
            }
            else if (lowerAttributeName == GovPublicContentAttribute.ImageUrl.ToLower())
            {
                styleInfo.DisplayName = "图片";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.Image);
            }
            else if (lowerAttributeName == GovPublicContentAttribute.FileUrl.ToLower())
            {
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.File);
                styleInfo.IsSingleLine = true;
                styleInfo.DisplayName = "附件";
            }
            else if (lowerAttributeName == GovPublicContentAttribute.IsRecommend.ToLower())
            {
                styleInfo.DisplayName = "推荐";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.CheckBox);
            }
            else if (lowerAttributeName == GovPublicContentAttribute.IsHot.ToLower())
            {
                styleInfo.DisplayName = "热点";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.CheckBox);
            }
            else if (lowerAttributeName == GovPublicContentAttribute.IsColor.ToLower())
            {
                styleInfo.DisplayName = "醒目";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.CheckBox);
            }
            else if (lowerAttributeName == ContentAttribute.IsTop.ToLower())
            {
                styleInfo.DisplayName = "置顶";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.CheckBox);
            }
            else if (lowerAttributeName == GovPublicContentAttribute.Content.ToLower())
            {
                styleInfo.DisplayName = "内容";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.TextEditor);
            }
            else if (lowerAttributeName == ContentAttribute.AddDate.ToLower())
            {
                styleInfo.DisplayName = "添加日期";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.DateTime);
                styleInfo.DefaultValue = Current;
            }
            return styleInfo;
        }

        public static TableStyleInfo GetVoteContentTableStyleInfo(string tableName, string attributeName)
        {
            var styleInfo = new TableStyleInfo(0, 0, tableName, attributeName, 0, attributeName, string.Empty, true, false, true, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, true, string.Empty);

            var lowerAttributeName = attributeName.ToLower();
            if (VoteContentAttribute.HiddenAttributes.Contains(lowerAttributeName))
            {
                if (lowerAttributeName == VoteContentAttribute.IsImageVote.ToLower())
                {
                    styleInfo.DisplayName = "图片投票";
                }
                else if (lowerAttributeName == VoteContentAttribute.IsSummary.ToLower())
                {
                    styleInfo.DisplayName = "显示简介";
                }
                else if (lowerAttributeName == VoteContentAttribute.Participants.ToLower())
                {
                    styleInfo.DisplayName = "参与人数";
                }
                else if (lowerAttributeName == VoteContentAttribute.IsClosed.ToLower())
                {
                    styleInfo.DisplayName = "已结束";
                }
                else if (lowerAttributeName == VoteContentAttribute.IsTop.ToLower())
                {
                    styleInfo.DisplayName = "置顶";
                }
                else
                {
                    styleInfo = GetContentHiddenTableStyleInfo(tableName, attributeName);
                }
                return styleInfo;
            }

            if (lowerAttributeName == VoteContentAttribute.Title.ToLower())
            {
                styleInfo.DisplayName = "标题";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.Text);
                styleInfo.Additional.IsFormatString = true;
                styleInfo.Additional.IsValidate = true;
                styleInfo.Additional.IsRequired = true;
            }
            else if (lowerAttributeName == VoteContentAttribute.SubTitle.ToLower())
            {
                styleInfo.DisplayName = "副标题";
                styleInfo.IsVisible = false;
            }
            else if (lowerAttributeName == VoteContentAttribute.MaxSelectNum.ToLower())
            {
                styleInfo.DisplayName = "单选/多选";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.SelectOne);
            }
            else if (lowerAttributeName == VoteContentAttribute.ImageUrl.ToLower())
            {
                styleInfo.DisplayName = "图片";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.Image);
            }
            else if (lowerAttributeName == VoteContentAttribute.Content.ToLower())
            {
                styleInfo.DisplayName = "内容";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.TextEditor);
            }
            else if (lowerAttributeName == VoteContentAttribute.Summary.ToLower())
            {
                styleInfo.DisplayName = "内容摘要";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.TextArea);
                styleInfo.IsVisible = false;
            }
            else if (lowerAttributeName == VoteContentAttribute.AddDate.ToLower())
            {
                styleInfo.DisplayName = "开始日期";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.DateTime);
                styleInfo.DefaultValue = Current;
            }
            else if (lowerAttributeName == VoteContentAttribute.EndDate.ToLower())
            {
                styleInfo.DisplayName = "截止日期";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.DateTime);
                styleInfo.DefaultValue = Current;
            }
            else if (lowerAttributeName == VoteContentAttribute.IsVotedView.ToLower())
            {
                styleInfo.DisplayName = "投票结果";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.Radio);
            }
            else if (lowerAttributeName == VoteContentAttribute.HiddenContent.ToLower())
            {
                styleInfo.DisplayName = "隐藏内容";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.TextEditor);
            }
            return styleInfo;
        }

        public static TableStyleInfo GetJobContentTableStyleInfo(string tableName, string attributeName)
        {
            var lowerAttributeName = attributeName.ToLower();
            if (ContentAttribute.HiddenAttributes.Contains(lowerAttributeName))
            {
                return GetContentHiddenTableStyleInfo(tableName, attributeName);
            }
            var styleInfo = new TableStyleInfo(0, 0, tableName, attributeName, 0, attributeName, string.Empty, true, false, true, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, true, string.Empty);
            var styleItems = new List<TableStyleItemInfo>();

            if (lowerAttributeName == ContentAttribute.Title.ToLower())
            {
                styleInfo.DisplayName = "标题";
                styleInfo.HelpText = "标题的名称";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.Text);
                styleInfo.Additional.IsFormatString = true;
                styleInfo.Additional.IsValidate = true;
                styleInfo.Additional.IsRequired = true;
            }
            else if (lowerAttributeName == JobContentAttribute.Department.ToLower())
            {
                styleInfo.DisplayName = "所属部门";
                styleInfo.HelpText = "所属部门";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.Text);
                styleInfo.Additional.IsValidate = true;
                styleInfo.Additional.IsRequired = true;
            }
            else if (lowerAttributeName == JobContentAttribute.Location.ToLower())
            {
                styleInfo.DisplayName = "工作地点";
                styleInfo.HelpText = "工作地点";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.Text);
                styleInfo.Additional.IsValidate = true;
                styleInfo.Additional.IsRequired = true;
            }
            else if (lowerAttributeName == JobContentAttribute.NumberOfPeople.ToLower())
            {
                styleInfo.DisplayName = "招聘人数";
                styleInfo.HelpText = "招聘人数";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.Text);
                styleInfo.DefaultValue = "不限";
                styleInfo.Additional.Width = "60px";
            }
            else if (lowerAttributeName == JobContentAttribute.Responsibility.ToLower())
            {
                styleInfo.DisplayName = "工作职责";
                styleInfo.HelpText = "工作职责";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.TextEditor);
            }
            else if (lowerAttributeName == JobContentAttribute.Requirement.ToLower())
            {
                styleInfo.DisplayName = "工作要求";
                styleInfo.HelpText = "工作要求";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.TextEditor);
            }
            else if (lowerAttributeName == JobContentAttribute.IsUrgent.ToLower())
            {
                styleInfo.DisplayName = "是否急聘";
                styleInfo.HelpText = "是否急聘";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.CheckBox);
                var itemInfo = new TableStyleItemInfo(0, styleInfo.TableStyleId, "急聘", true.ToString(), false);
                styleItems.Add(itemInfo);
                styleInfo.StyleItems = styleItems;
            }
            else if (lowerAttributeName == ContentAttribute.IsTop.ToLower())
            {
                styleInfo.DisplayName = "置顶";
                styleInfo.HelpText = "是否为置顶内容";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.CheckBox);
            }
            else if (lowerAttributeName == ContentAttribute.AddDate.ToLower())
            {
                styleInfo.DisplayName = "添加日期";
                styleInfo.HelpText = "内容的添加日期";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.DateTime);
                styleInfo.DefaultValue = Current;
            }
            return styleInfo;
        }

        public static TableStyleInfo GetUserDefinedTableStyleInfo(string tableName, string attributeName)
        {
            var lowerAttributeName = attributeName.ToLower();
            if (ContentAttribute.HiddenAttributes.Contains(lowerAttributeName))
            {
                return GetContentHiddenTableStyleInfo(tableName, attributeName);
            }
            var styleInfo = new TableStyleInfo(0, 0, tableName, attributeName, 0, attributeName, string.Empty, true, false, true, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, true, string.Empty);

            if (lowerAttributeName == ContentAttribute.Title.ToLower())
            {
                styleInfo.DisplayName = "标题";
                styleInfo.HelpText = "标题的名称";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.Text);
                styleInfo.Additional.IsFormatString = true;
                styleInfo.Additional.IsValidate = true;
                styleInfo.Additional.IsRequired = true;
            }
            else if (lowerAttributeName == ContentAttribute.IsTop.ToLower())
            {
                styleInfo.DisplayName = "置顶";
                styleInfo.HelpText = "是否为置顶内容";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.CheckBox);
            }
            else if (lowerAttributeName == ContentAttribute.AddDate.ToLower())
            {
                styleInfo.DisplayName = "添加日期";
                styleInfo.HelpText = "内容的添加日期";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.DateTime);
                styleInfo.DefaultValue = Current;
            }

            return styleInfo;
        }

        public static TableStyleInfo GetGovInteractContentTableStyleInfo(string tableName, string attributeName)
        {
            var lowerAttributeName = attributeName.ToLower();
            if (GovInteractContentAttribute.HiddenAttributes.Contains(lowerAttributeName))
            {
                return GetContentHiddenTableStyleInfo(tableName, attributeName);
            }
            var styleInfo = new TableStyleInfo(0, 0, tableName, attributeName, 0, attributeName, string.Empty, true, false, false, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, true, string.Empty);

            if (lowerAttributeName == GovInteractContentAttribute.RealName.ToLower())
            {
                styleInfo.DisplayName = "姓名";
                styleInfo.Additional.IsValidate = true;
                styleInfo.Additional.IsRequired = true;
            }
            else if (lowerAttributeName == GovInteractContentAttribute.Organization.ToLower())
            {
                styleInfo.DisplayName = "工作单位";
            }
            else if (lowerAttributeName == GovInteractContentAttribute.CardType.ToLower())
            {
                styleInfo.DisplayName = "证件名称";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.SelectOne);
                styleInfo.StyleItems = new List<TableStyleItemInfo>
                {
                    new TableStyleItemInfo(0, 0, "身份证", "身份证", false),
                    new TableStyleItemInfo(0, 0, "学生证", "学生证", false),
                    new TableStyleItemInfo(0, 0, "军官证", "军官证", false),
                    new TableStyleItemInfo(0, 0, "工作证", "工作证", false)
                };
            }
            else if (lowerAttributeName == GovInteractContentAttribute.CardNo.ToLower())
            {
                styleInfo.DisplayName = "证件号码";
                styleInfo.Additional.IsValidate = true;
                styleInfo.Additional.IsRequired = true;
            }
            else if (lowerAttributeName == GovInteractContentAttribute.Phone.ToLower())
            {
                styleInfo.DisplayName = "联系电话";
                styleInfo.Additional.IsValidate = true;
                styleInfo.Additional.IsRequired = true;
            }
            else if (lowerAttributeName == GovInteractContentAttribute.PostCode.ToLower())
            {
                styleInfo.DisplayName = "邮政编码";
                styleInfo.Additional.IsValidate = true;
                styleInfo.Additional.IsRequired = true;
                styleInfo.Additional.ValidateType = EInputValidateType.Integer;
            }
            else if (lowerAttributeName == GovInteractContentAttribute.Address.ToLower())
            {
                styleInfo.DisplayName = "联系地址";
                styleInfo.IsSingleLine = true;
                styleInfo.Additional.IsValidate = true;
                styleInfo.Additional.IsRequired = true;
            }
            else if (lowerAttributeName == GovInteractContentAttribute.Email.ToLower())
            {
                styleInfo.DisplayName = "电子邮件";
                styleInfo.Additional.IsValidate = true;
                styleInfo.Additional.IsRequired = true;
                styleInfo.Additional.ValidateType = EInputValidateType.Email;
            }
            else if (lowerAttributeName == GovInteractContentAttribute.Fax.ToLower())
            {
                styleInfo.DisplayName = "传真";
                styleInfo.Additional.ValidateType = EInputValidateType.Integer;
            }
            else if (lowerAttributeName == GovInteractContentAttribute.TypeId.ToLower())
            {
                styleInfo.DisplayName = "类型";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.SpecifiedValue);
            }
            else if (lowerAttributeName == GovInteractContentAttribute.IsPublic.ToLower())
            {
                styleInfo.DisplayName = "是否公开";
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.Radio);
                styleInfo.StyleItems = new List<TableStyleItemInfo>
                {
                    new TableStyleItemInfo(0, 0, "公开", true.ToString(), true),
                    new TableStyleItemInfo(0, 0, "不公开", false.ToString(), false)
                };
            }
            else if (lowerAttributeName == GovInteractContentAttribute.Title.ToLower())
            {
                styleInfo.DisplayName = "标题";
                styleInfo.IsSingleLine = true;
                styleInfo.Additional.IsValidate = true;
                styleInfo.Additional.IsRequired = true;
            }
            else if (lowerAttributeName == GovInteractContentAttribute.Content.ToLower())
            {
                styleInfo.DisplayName = "内容";
                styleInfo.IsSingleLine = true;
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.TextArea);
                styleInfo.Additional.Width = "90%";
                styleInfo.Additional.Height = 180;
                styleInfo.Additional.IsValidate = true;
                styleInfo.Additional.IsRequired = true;
            }
            else if (lowerAttributeName == GovInteractContentAttribute.FileUrl.ToLower())
            {
                styleInfo.DisplayName = "附件";
                styleInfo.IsSingleLine = true;
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.File);
            }
            else if (lowerAttributeName == GovInteractContentAttribute.DepartmentId.ToLower())
            {
                styleInfo.DisplayName = "提交对象";
                styleInfo.IsSingleLine = true;
                styleInfo.InputType = EInputTypeUtils.GetValue(EInputType.SpecifiedValue);
            }
            return styleInfo;
        }

        private static TableStyleInfo GetContentHiddenTableStyleInfo(string tableName, string attributeName)
        {
            var styleInfo = new TableStyleInfo(0, 0, tableName, attributeName, 0, attributeName, string.Empty, true, false, true, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, true, string.Empty);
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
            return new TableStyleInfo(0, 0, tableName, attributeName, 0, attributeName, string.Empty, true, false, true, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, true, string.Empty);
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
