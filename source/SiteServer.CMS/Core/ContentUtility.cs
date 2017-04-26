using System;
using BaiRong.Core.Model;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using System.Text;
using BaiRong.Core;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.Model;
using BaiRong.Core.AuxiliaryTable;

namespace SiteServer.CMS.Core
{
    public class ContentUtility
    {
        private ContentUtility()
        {
        }

        public static string PagePlaceHolder = "[SITESERVER_PAGE]";//内容翻页展位符

        public static string GetTitleFormatString(bool isStrong, bool isEm, bool isU, string color)
        {
            return $"{isStrong}_{isEm}_{isU}_{color}";
        }

        public static bool SetTitleFormatControls(string titleFormatString, CheckBox formatStrong, CheckBox formatEm, CheckBox formatU, TextBox formatColor)
        {
            var isTitleFormatted = false;
            if (!string.IsNullOrEmpty(titleFormatString))
            {
                var formats = titleFormatString.Split('_');
                if (formats.Length == 4)
                {
                    formatStrong.Checked = TranslateUtils.ToBool(formats[0]);
                    formatEm.Checked = TranslateUtils.ToBool(formats[1]);
                    formatU.Checked = TranslateUtils.ToBool(formats[2]);
                    formatColor.Text = formats[3];
                    if (formatStrong.Checked || formatEm.Checked || formatU.Checked || !string.IsNullOrEmpty(formatColor.Text))
                    {
                        isTitleFormatted = true;
                    }
                }
            }
            return isTitleFormatted;
        }

        public static bool SetTitleFormatControls(string titleFormatString, out bool formatStrong, out bool formatEm, out bool formatU, out string formatColor)
        {
            var isTitleFormatted = false;

            formatStrong = formatEm = formatU = false;
            formatColor = string.Empty;

            if (!string.IsNullOrEmpty(titleFormatString))
            {
                var formats = titleFormatString.Split('_');
                if (formats.Length == 4)
                {
                    formatStrong = TranslateUtils.ToBool(formats[0]);
                    formatEm = TranslateUtils.ToBool(formats[1]);
                    formatU = TranslateUtils.ToBool(formats[2]);
                    formatColor = formats[3];
                    if (formatStrong || formatEm || formatU || !string.IsNullOrEmpty(formatColor))
                    {
                        isTitleFormatted = true;
                    }
                }
            }
            return isTitleFormatted;
        }

        public static string FormatTitle(string titleFormatString, string title)
        {
            var formattedTitle = title;
            if (!string.IsNullOrEmpty(titleFormatString))
            {
                var formats = titleFormatString.Split('_');
                if (formats.Length == 4)
                {
                    var isStrong = TranslateUtils.ToBool(formats[0]);
                    var isEm = TranslateUtils.ToBool(formats[1]);
                    var isU = TranslateUtils.ToBool(formats[2]);
                    var color = formats[3];

                    if (!string.IsNullOrEmpty(color))
                    {
                        if (!color.StartsWith("#"))
                        {
                            color = "#" + color;
                        }
                        formattedTitle = $@"<span style=""color:{color}"">{formattedTitle}</span>";
                    }
                    if (isStrong)
                    {
                        formattedTitle = $"<strong>{formattedTitle}</strong>";
                    }
                    if (isEm)
                    {
                        formattedTitle = $"<em>{formattedTitle}</em>";
                    }
                    if (isU)
                    {
                        formattedTitle = $"<u>{formattedTitle}</u>";
                    }
                }
            }
            return formattedTitle;
        }

        public static void PutImagePaths(PublishmentSystemInfo publishmentSystemInfo, BackgroundContentInfo contentInfo, NameValueCollection collection)
        {
            if (contentInfo != null)
            {
                //如果是图片模型
                var nodeInfo = DataProvider.NodeDao.GetNodeInfo(contentInfo.NodeId);
                if (EContentModelTypeUtils.IsPhoto(nodeInfo.ContentModelId))
                {
                    var photoInfoList = DataProvider.PhotoDao.GetPhotoInfoList(publishmentSystemInfo.PublishmentSystemId, contentInfo.Id);
                    foreach (var photoInfo in photoInfoList)
                    {
                        collection[photoInfo.SmallUrl] = PathUtility.MapPath(publishmentSystemInfo, photoInfo.SmallUrl);
                        collection[photoInfo.MiddleUrl] = PathUtility.MapPath(publishmentSystemInfo, photoInfo.MiddleUrl);
                        collection[photoInfo.LargeUrl] = PathUtility.MapPath(publishmentSystemInfo, photoInfo.LargeUrl);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(contentInfo.ImageUrl) && PageUtility.IsVirtualUrl(contentInfo.ImageUrl))
                    {
                        collection[contentInfo.ImageUrl] = PathUtility.MapPath(publishmentSystemInfo, contentInfo.ImageUrl);
                    }
                    if (!string.IsNullOrEmpty(contentInfo.VideoUrl) && PageUtility.IsVirtualUrl(contentInfo.VideoUrl))
                    {
                        collection[contentInfo.VideoUrl] = PathUtility.MapPath(publishmentSystemInfo, contentInfo.VideoUrl);
                    }
                    if (!string.IsNullOrEmpty(contentInfo.FileUrl) && PageUtility.IsVirtualUrl(contentInfo.FileUrl))
                    {
                        collection[contentInfo.FileUrl] = PathUtility.MapPath(publishmentSystemInfo, contentInfo.FileUrl);
                    }
                }
                var srcArrayList = RegexUtils.GetOriginalImageSrcs(contentInfo.Content);
                foreach (string src in srcArrayList)
                {
                    if (PageUtility.IsVirtualUrl(src))
                    {
                        collection[src] = PathUtility.MapPath(publishmentSystemInfo, src);
                    }
                }
            }
        }

        public static string GetAutoPageContent(string content, int pageWordNum)
        {
            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(content))
            {
                content = content.Replace(PagePlaceHolder, string.Empty);
                AutoPage(builder, content, pageWordNum);
            }
            return builder.ToString();
        }

        private static void AutoPage(StringBuilder builder, string content, int pageWordNum)
        {
            if (content.Length > pageWordNum)
            {
                var i = content.IndexOf("</P>", pageWordNum, StringComparison.Ordinal);
                if (i == -1)
                {
                    i = content.IndexOf("</p>", pageWordNum, StringComparison.Ordinal);
                }

                if (i != -1)
                {
                    var start = i + 4;
                    builder.Append(content.Substring(0, start));
                    content = content.Substring(start);
                    if (!string.IsNullOrEmpty(content))
                    {
                        builder.Append(PagePlaceHolder);
                        AutoPage(builder, content, pageWordNum);
                    }
                }
                else
                {
                    builder.Append(content);
                }
            }
            else
            {
                builder.Append(content);
            }
        }

        public static int GetRealContentId(ETableStyle tableStyle, string tableName, int contentId)
        {
            string linkUrl;
            var referenceId = BaiRongDataProvider.ContentDao.GetReferenceId(tableStyle, tableName, contentId, out linkUrl);
            return referenceId > 0 ? referenceId : contentId;
        }

        public static ContentInfo GetContentInfo(ETableStyle tableStyle)
        {
            switch (tableStyle)
            {
                case ETableStyle.BackgroundContent:
                    return new BackgroundContentInfo();
                case ETableStyle.GovPublicContent:
                    return new GovPublicContentInfo();
                case ETableStyle.GovInteractContent:
                    return new GovInteractContentInfo();
                case ETableStyle.VoteContent:
                    return new VoteContentInfo();
                case ETableStyle.JobContent:
                    return new JobContentInfo();
            }
            return new ContentInfo();
        }

        public static int GetColumnWidth(ETableStyle tableStyle, string attributeName)
        {
            var width = 80;
            if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.Hits) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.HitsByDay) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.HitsByMonth) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.HitsByWeek))
            {
                width = 50;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.AddUserName) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.LastEditUserName) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.CheckUserName))
            {
                width = 60;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.AddDate) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.LastEditDate))
            {
                width = 70;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.LastHitsDate) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.CheckCheckDate))
            {
                width = 110;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, BackgroundContentAttribute.Digg) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.CheckReasons))
            {
                width = 110;
            }
            return width;
        }

        public static List<TableStyleInfo> GetAllTableStyleInfoList(PublishmentSystemInfo publishmentSystemInfo, ETableStyle tableStyle, List<TableStyleInfo> tableStyleInfoList)
        {
            var arraylist = new List<TableStyleInfo>();

            var styleInfo = new TableStyleInfo
            {
                AttributeName = ContentAttribute.Id,
                DisplayName = "编号"
            };
            arraylist.Add(styleInfo);

            arraylist.AddRange(tableStyleInfoList);

            styleInfo = new TableStyleInfo
            {
                AttributeName = ContentAttribute.Hits,
                DisplayName = "点击量"
            };
            arraylist.Add(styleInfo);

            if (tableStyle == ETableStyle.BackgroundContent)
            {
                if (publishmentSystemInfo.Additional.IsRelatedByTags)
                {
                    styleInfo = new TableStyleInfo
                    {
                        AttributeName = ContentAttribute.Tags,
                        DisplayName = "标签"
                    };
                    arraylist.Add(styleInfo);
                }

                styleInfo = new TableStyleInfo
                {
                    AttributeName = BackgroundContentAttribute.Star,
                    DisplayName = "评分"
                };
                arraylist.Add(styleInfo);
            }
            else
            {
                styleInfo = new TableStyleInfo
                {
                    AttributeName = ContentAttribute.AddDate,
                    DisplayName = "添加时间"
                };
                arraylist.Add(styleInfo);
            }

            return arraylist;
        }

        public static List<TableStyleInfo> GetColumnTableStyleInfoList(PublishmentSystemInfo publishmentSystemInfo, ETableStyle tableStyle, List<TableStyleInfo> tableStyleInfoArrayList)
        {
            var arraylist = new List<TableStyleInfo>();

            if (tableStyleInfoArrayList != null)
            {
                foreach (var tableStyleInfo in tableStyleInfoArrayList)
                {
                    arraylist.Add(tableStyleInfo);
                }
            }

            var styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.AddUserName, 0, "添加者", string.Empty, true, true, false, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.LastEditUserName, 0, "修改者", string.Empty, true, true, false, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.LastEditDate, 0, "修改时间", string.Empty, true, true, false, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.CheckUserName, 0, "审核者", string.Empty, true, true, false, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.CheckCheckDate, 0, "审核时间", string.Empty, true, true, false, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.CheckReasons, 0, "审核原因", string.Empty, true, true, false, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.SourceId, 0, "来源标识", string.Empty, true, true, false, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            if (publishmentSystemInfo.Additional.IsRelatedByTags)
            {
                styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.Tags, 0, "标签", string.Empty, true, true, false, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, false, string.Empty);
                arraylist.Add(styleInfo);
            }

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.ContentGroupNameCollection, 0, "所属内容组", string.Empty, true, true, false, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.Hits, 0, "点击量", string.Empty, true, true, false, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.HitsByDay, 0, "日点击", string.Empty, true, true, false, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.HitsByWeek, 0, "周点击", string.Empty, true, true, false, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.HitsByMonth, 0, "月点击", string.Empty, true, true, false, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.LastHitsDate, 0, "最后点击时间", string.Empty, true, true, false, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            if (tableStyle == ETableStyle.BackgroundContent)
            {
                styleInfo = new TableStyleInfo(0, 0, string.Empty, BackgroundContentAttribute.Star, 0, "评分", string.Empty, true, true, false, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, false, string.Empty);
                arraylist.Add(styleInfo);

                styleInfo = new TableStyleInfo(0, 0, string.Empty, BackgroundContentAttribute.Digg, 0, "Digg", string.Empty, true, true, false, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, false, string.Empty);
                arraylist.Add(styleInfo);
            }

            return arraylist;
        }

        public static string GetColumnHeadRowsHtml(List<TableStyleInfo> tableStyleInfoList, StringCollection attributesOfDisplay, ETableStyle tableStyle, PublishmentSystemInfo publishmentSystemInfo)
        {
            var builder = new StringBuilder();

            var arrayList = GetColumnTableStyleInfoList(publishmentSystemInfo, tableStyle, tableStyleInfoList);
            foreach (TableStyleInfo styleInfo in arrayList)
            {
                if (attributesOfDisplay.Contains(styleInfo.AttributeName))
                {
                    builder.Append(
                        $@"<td width=""{GetColumnWidth(tableStyle, styleInfo.AttributeName)}"">{styleInfo.DisplayName}</td>");
                }
            }

            return builder.ToString();
        }

        public static string GetCommandHeadRowsHtml(string administratorName, ETableStyle tableStyle, PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo)
        {
            var builder = new StringBuilder();

            var modelType = EContentModelTypeUtils.GetEnumType(nodeInfo.ContentModelId);

            if (modelType == EContentModelType.Photo)
            {
                builder.Append(@"<td class=""center"" width=""50"">&nbsp;</td>");
            }
            else if (modelType == EContentModelType.Job)
            {
                builder.Append(@"<td class=""center"" width=""50"">&nbsp;</td>");
            }

            if (publishmentSystemInfo.Additional.IsCommentable && modelType != EContentModelType.Job)
            {
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Cms.Permission.Channel.CommentCheck, AppManager.Cms.Permission.Channel.CommentDelete))
                {
                    builder.Append(@"<td class=""center"" width=""50"">&nbsp;</td>");
                }
            }
            return builder.ToString();
        }

        public static void Translate(PublishmentSystemInfo publishmentSystemInfo, int nodeId, int contentId, string translateCollection, ETranslateContentType translateType, string administratorName)
        {
            var translateArrayList = TranslateUtils.StringCollectionToStringList(translateCollection);
            foreach (string translate in translateArrayList)
            {
                if (!string.IsNullOrEmpty(translate))
                {
                    var translates = translate.Split('_');
                    if (translates.Length == 2)
                    {
                        var targetPublishmentSystemId = TranslateUtils.ToInt(translates[0]);
                        var targetNodeId = TranslateUtils.ToInt(translates[1]);

                        Translate(administratorName, publishmentSystemInfo, nodeId, contentId, targetPublishmentSystemId, targetNodeId, translateType);
                    }
                }
            }
        }

        public static void Translate(string administratorName, PublishmentSystemInfo publishmentSystemInfo, int nodeId, int contentId, int targetPublishmentSystemId, int targetNodeId, ETranslateContentType translateType)
        {
            if (publishmentSystemInfo == null || nodeId <= 0 || contentId <= 0 || targetPublishmentSystemId <= 0 || targetNodeId <= 0) return;

            var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);

            var targetTableName = NodeManager.GetTableName(targetPublishmentSystemInfo, targetNodeId);

            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);

            var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);

            if (contentInfo == null) return;

            if (translateType == ETranslateContentType.Copy)
            {
                FileUtility.MoveFileByContentInfo(publishmentSystemInfo, targetPublishmentSystemInfo, contentInfo);

                contentInfo.PublishmentSystemId = targetPublishmentSystemId;
                contentInfo.SourceId = contentInfo.NodeId;
                contentInfo.NodeId = targetNodeId;
                contentInfo.Attributes[ContentAttribute.TranslateContentType] = ETranslateContentType.Copy.ToString();
                //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, ETranslateContentType.Copy.ToString());
                var theContentId = DataProvider.ContentDao.Insert(targetTableName, targetPublishmentSystemInfo, contentInfo);
                if (EContentModelTypeUtils.IsPhoto(nodeInfo.ContentModelId))
                {
                    var photoInfoList = DataProvider.PhotoDao.GetPhotoInfoList(publishmentSystemInfo.PublishmentSystemId, contentId);
                    if (photoInfoList.Count > 0)
                    {
                        foreach (var photoInfo in photoInfoList)
                        {
                            photoInfo.PublishmentSystemID = targetPublishmentSystemId;
                            photoInfo.ContentID = theContentId;

                            FileUtility.MoveFileByVirtaulUrl(publishmentSystemInfo, targetPublishmentSystemInfo, photoInfo.SmallUrl);
                            FileUtility.MoveFileByVirtaulUrl(publishmentSystemInfo, targetPublishmentSystemInfo, photoInfo.MiddleUrl);
                            FileUtility.MoveFileByVirtaulUrl(publishmentSystemInfo, targetPublishmentSystemInfo, photoInfo.LargeUrl);

                            DataProvider.PhotoDao.Insert(photoInfo);
                        }
                    }
                }
                if (contentInfo.IsChecked)
                {
                    CreateManager.CreateContentAndTrigger(targetPublishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, theContentId);
                }
            }
            else if (translateType == ETranslateContentType.Cut)
            {
                FileUtility.MoveFileByContentInfo(publishmentSystemInfo, targetPublishmentSystemInfo, contentInfo);

                contentInfo.PublishmentSystemId = targetPublishmentSystemId;
                contentInfo.SourceId = contentInfo.NodeId;
                contentInfo.NodeId = targetNodeId;
                contentInfo.Attributes[ContentAttribute.TranslateContentType] = ETranslateContentType.Cut.ToString();
                //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, ETranslateContentType.Cut.ToString());
                if (StringUtils.EqualsIgnoreCase(tableName, targetTableName))
                {
                    contentInfo.Taxis = DataProvider.ContentDao.GetTaxisToInsert(targetTableName, targetNodeId, contentInfo.IsTop);
                    DataProvider.ContentDao.Update(targetTableName, targetPublishmentSystemInfo, contentInfo);
                }
                else
                {
                    DataProvider.ContentDao.Insert(targetTableName, targetPublishmentSystemInfo, contentInfo);
                    DataProvider.ContentDao.DeleteContents(publishmentSystemInfo.PublishmentSystemId, tableName, TranslateUtils.ToIntList(contentId), nodeId);
                }

                DataProvider.NodeDao.UpdateContentNum(publishmentSystemInfo, nodeId, true);
                DataProvider.NodeDao.UpdateContentNum(targetPublishmentSystemInfo, targetNodeId, true);

                if (EContentModelTypeUtils.IsPhoto(nodeInfo.ContentModelId))
                {
                    var photoInfoList = DataProvider.PhotoDao.GetPhotoInfoList(publishmentSystemInfo.PublishmentSystemId, contentId);
                    if (photoInfoList.Count > 0)
                    {
                        foreach (var photoInfo in photoInfoList)
                        {
                            photoInfo.PublishmentSystemID = targetPublishmentSystemId;

                            FileUtility.MoveFileByVirtaulUrl(publishmentSystemInfo, targetPublishmentSystemInfo, photoInfo.SmallUrl);
                            FileUtility.MoveFileByVirtaulUrl(publishmentSystemInfo, targetPublishmentSystemInfo, photoInfo.MiddleUrl);
                            FileUtility.MoveFileByVirtaulUrl(publishmentSystemInfo, targetPublishmentSystemInfo, photoInfo.LargeUrl);

                            DataProvider.PhotoDao.Update(photoInfo);
                        }
                    }
                }
                if (contentInfo.IsChecked)
                {
                    CreateManager.CreateContentAndTrigger(targetPublishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, contentInfo.Id);
                }
            }
            else if (translateType == ETranslateContentType.Reference)
            {
                if (contentInfo.ReferenceId != 0) return;

                contentInfo.PublishmentSystemId = targetPublishmentSystemId;
                contentInfo.SourceId = contentInfo.NodeId;
                contentInfo.NodeId = targetNodeId;
                contentInfo.ReferenceId = contentId;
                contentInfo.Attributes[ContentAttribute.TranslateContentType] = ETranslateContentType.Reference.ToString();
                //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, ETranslateContentType.Reference.ToString());
                DataProvider.ContentDao.Insert(targetTableName, targetPublishmentSystemInfo, contentInfo);
            }
            else if (translateType == ETranslateContentType.ReferenceContent)
            {
                if (contentInfo.ReferenceId != 0) return;

                FileUtility.MoveFileByContentInfo(publishmentSystemInfo, targetPublishmentSystemInfo, contentInfo);

                contentInfo.PublishmentSystemId = targetPublishmentSystemId;
                contentInfo.SourceId = contentInfo.NodeId;
                contentInfo.NodeId = targetNodeId;
                contentInfo.ReferenceId = contentId;
                contentInfo.Attributes[ContentAttribute.TranslateContentType] = ETranslateContentType.ReferenceContent.ToString();
                var theContentId = DataProvider.ContentDao.Insert(targetTableName, targetPublishmentSystemInfo, contentInfo);
                if (EContentModelTypeUtils.IsPhoto(nodeInfo.ContentModelId))
                {
                    var photoInfoList = DataProvider.PhotoDao.GetPhotoInfoList(publishmentSystemInfo.PublishmentSystemId, contentId);
                    if (photoInfoList.Count > 0)
                    {
                        foreach (var photoInfo in photoInfoList)
                        {
                            photoInfo.PublishmentSystemID = targetPublishmentSystemId;
                            photoInfo.ContentID = theContentId;

                            FileUtility.MoveFileByVirtaulUrl(publishmentSystemInfo, targetPublishmentSystemInfo, photoInfo.SmallUrl);
                            FileUtility.MoveFileByVirtaulUrl(publishmentSystemInfo, targetPublishmentSystemInfo, photoInfo.MiddleUrl);
                            FileUtility.MoveFileByVirtaulUrl(publishmentSystemInfo, targetPublishmentSystemInfo, photoInfo.LargeUrl);

                            DataProvider.PhotoDao.Insert(photoInfo);
                        }
                    }
                }

                if (contentInfo.IsChecked)
                {
                    CreateManager.CreateContentAndTrigger(targetPublishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, theContentId);
                }
            }
        }

        public static Dictionary<int, List<int>> GetIDsDictionary(NameValueCollection queryString)
        {
            var dic = new Dictionary<int, List<int>>();

            if (!string.IsNullOrEmpty(queryString["IDsCollection"]))
            {
                foreach (var ids in TranslateUtils.StringCollectionToStringList(queryString["IDsCollection"]))
                {
                    var nodeId = TranslateUtils.ToInt(ids.Split('_')[0]);
                    var contentId = TranslateUtils.ToInt(ids.Split('_')[1]);
                    var contentIdList = new List<int>();
                    if (dic.ContainsKey(nodeId))
                    {
                        contentIdList = dic[nodeId];
                    }
                    if (!contentIdList.Contains(contentId))
                    {
                        contentIdList.Add(contentId);
                    }

                    dic[nodeId] = contentIdList;
                }
            }
            else
            {
                var nodeId = TranslateUtils.ToInt(queryString["NodeID"]);
                dic[nodeId] = TranslateUtils.StringCollectionToIntList(queryString["ContentIDCollection"]);
            }

            return dic;
        }

        public static Dictionary<string, object> ContentToDictionary(ContentInfo contentInfo, ETableStyle tableStyle, string tableName, List<int> relatedIdentities)
        {
            var dict = TranslateUtils.ObjectToDictionary(contentInfo);
            dict.Remove("Attributes");

            var styleInfoList = TableStyleManager.GetTableStyleInfoList(tableStyle, tableName, relatedIdentities);
            foreach (var styleInfo in styleInfoList)
            {
                if (!styleInfo.IsVisible) continue;
                if (!dict.ContainsKey(styleInfo.AttributeName))
                {
                    dict[styleInfo.AttributeName] = contentInfo.GetExtendedAttribute(styleInfo.AttributeName);
                }
                if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.Image))
                {
                    var extendName = ContentAttribute.GetExtendAttributeName(styleInfo.AttributeName);
                    var extendValue = contentInfo.GetExtendedAttribute(extendName);
                    if (!string.IsNullOrEmpty(extendValue))
                    {
                        dict[extendName] = extendValue;
                    }
                }
            }

            return dict;
        }
    }
}
