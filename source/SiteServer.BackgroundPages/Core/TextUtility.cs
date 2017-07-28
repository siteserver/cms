using BaiRong.Core.Model;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using System.Text;
using BaiRong.Core;
using System.Web.UI.WebControls;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Permissions;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Core
{
    public class TextUtility
    {
        private TextUtility()
        {
        }

        public static string PagePlaceHolder = "[SITESERVER_PAGE]";//内容翻页展位符

        public static string GetTitleFormatString(bool isStrong, bool isEM, bool isU, string color)
        {
            return $"{isStrong}_{isEM}_{isU}_{color}";
        }

        public static bool SetTitleFormatControls(string titleFormatString, CheckBox formatStrong, CheckBox formatEM, CheckBox formatU, TextBox formatColor)
        {
            var isTitleFormatted = false;
            if (!string.IsNullOrEmpty(titleFormatString))
            {
                var formats = titleFormatString.Split('_');
                if (formats.Length == 4)
                {
                    formatStrong.Checked = TranslateUtils.ToBool(formats[0]);
                    formatEM.Checked = TranslateUtils.ToBool(formats[1]);
                    formatU.Checked = TranslateUtils.ToBool(formats[2]);
                    formatColor.Text = formats[3];
                    if (formatStrong.Checked || formatEM.Checked || formatU.Checked || !string.IsNullOrEmpty(formatColor.Text))
                    {
                        isTitleFormatted = true;
                    }
                }
            }
            return isTitleFormatted;
        }

        public static bool SetTitleFormatControls(string titleFormatString, out bool formatStrong, out bool formatEM, out bool formatU, out string formatColor)
        {
            var isTitleFormatted = false;

            formatStrong = formatEM = formatU = false;
            formatColor = string.Empty;

            if (!string.IsNullOrEmpty(titleFormatString))
            {
                var formats = titleFormatString.Split('_');
                if (formats.Length == 4)
                {
                    formatStrong = TranslateUtils.ToBool(formats[0]);
                    formatEM = TranslateUtils.ToBool(formats[1]);
                    formatU = TranslateUtils.ToBool(formats[2]);
                    formatColor = formats[3];
                    if (formatStrong || formatEM || formatU || !string.IsNullOrEmpty(formatColor))
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
                    var isEM = TranslateUtils.ToBool(formats[1]);
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
                    if (isEM)
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
                var i = content.IndexOf("</P>", pageWordNum);
                if (i == -1)
                {
                    i = content.IndexOf("</p>", pageWordNum);
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

        public static int GetRealContentID(ETableStyle tableStyle, string tableName, int contentID)
        {
            var linkUrl = string.Empty;
            var referenceID = BaiRongDataProvider.ContentDao.GetReferenceId(tableStyle, tableName, contentID, out linkUrl);
            if (referenceID > 0)
            {
                return referenceID;
            }
            return contentID;
        }

        public static ContentInfo GetContentInfo(ETableStyle tableStyle)
        {
            if (tableStyle == ETableStyle.BackgroundContent)
            {
                return new BackgroundContentInfo();
            }
            else if (tableStyle == ETableStyle.GovPublicContent)
            {
                return new GovPublicContentInfo();
            }
            else if (tableStyle == ETableStyle.GovInteractContent)
            {
                return new GovInteractContentInfo();
            }
            else if (tableStyle == ETableStyle.VoteContent)
            {
                return new VoteContentInfo();
            }
            else if (tableStyle == ETableStyle.JobContent)
            {
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

        private static string GetColumnValue(Hashtable valueHashtable, ETableStyle tableStyle, PublishmentSystemInfo publishmentSystemInfo, ContentInfo contentInfo, TableStyleInfo styleInfo)
        {
            var value = string.Empty;
            if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.AddUserName))
            {
                if (!string.IsNullOrEmpty(contentInfo.AddUserName))
                {
                    var key = ContentAttribute.AddUserName + ":" + contentInfo.AddUserName;
                    value = valueHashtable[key] as string;
                    if (value == null)
                    {
                        value =
                            $@"<a rel=""popover"" class=""popover-hover"" data-content=""{AdminManager.GetFullName(
                                contentInfo.AddUserName)}"" data-original-title=""管理员"">{AdminManager.GetDisplayName(
                                contentInfo.AddUserName, false)}</a>";
                        valueHashtable[key] = value;
                    }
                }
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.LastEditUserName))
            {
                if (!string.IsNullOrEmpty(contentInfo.LastEditUserName))
                {
                    var key = ContentAttribute.LastEditUserName + ":" + contentInfo.LastEditUserName;
                    value = valueHashtable[key] as string;
                    if (value == null)
                    {
                        value =
                            $@"<a rel=""popover"" class=""popover-hover"" data-content=""{AdminManager.GetFullName(
                                contentInfo.LastEditUserName)}"" data-original-title=""管理员"">{AdminManager.GetDisplayName(
                                contentInfo.LastEditUserName, false)}</a>";
                        valueHashtable[key] = value;
                    }
                }
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.CheckUserName))
            {
                var checkUserName = contentInfo.GetExtendedAttribute(ContentAttribute.CheckUserName);
                if (!string.IsNullOrEmpty(checkUserName))
                {
                    var key = ContentAttribute.CheckUserName + ":" + checkUserName;
                    value = valueHashtable[key] as string;
                    if (value == null)
                    {
                        value =
                            $@"<a rel=""popover"" class=""popover-hover"" data-content=""{AdminManager.GetFullName(
                                checkUserName)}"" data-original-title=""管理员"">{AdminManager.GetDisplayName(checkUserName,
                                false)}</a>";

                        valueHashtable[key] = value;
                    }
                }
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.CheckCheckDate))
            {
                var checkDate = contentInfo.GetExtendedAttribute(ContentAttribute.CheckCheckDate);
                if (!string.IsNullOrEmpty(checkDate))
                {
                    value = DateUtils.GetDateAndTimeString(TranslateUtils.ToDateTime(checkDate));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.CheckReasons))
            {
                value = contentInfo.GetExtendedAttribute(ContentAttribute.CheckReasons);
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.AddDate))
            {
                value = DateUtils.GetDateString(contentInfo.AddDate);
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.LastEditDate))
            {
                value = DateUtils.GetDateString(contentInfo.LastEditDate);
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.SourceId))
            {
                value = SourceManager.GetSourceName(contentInfo.SourceId);
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.Tags))
            {
                value = contentInfo.Tags;
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.ContentGroupNameCollection))
            {
                value = contentInfo.ContentGroupNameCollection;
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.Hits))
            {
                value = contentInfo.Hits.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.HitsByDay))
            {
                value = contentInfo.HitsByDay.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.HitsByWeek))
            {
                value = contentInfo.HitsByWeek.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.HitsByMonth))
            {
                value = contentInfo.HitsByMonth.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.LastHitsDate))
            {
                value = DateUtils.GetDateAndTimeString(contentInfo.LastHitsDate);
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.IsTop))
            {
                value = StringUtils.GetTrueImageHtml(contentInfo.IsTop);
            }
            else
            {
                var isSettting = false;
                if (tableStyle == ETableStyle.BackgroundContent)
                {
                    isSettting = true;
                    if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, BackgroundContentAttribute.Star))
                    {
                        var showPopWinString = ModalContentStarSet.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, contentInfo.Id);
                        value =
                            $@"<a href=""javascript:;"" onclick=""{showPopWinString}"" title=""点击设置评分"">{StarsManager
                                .GetStarString(publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, contentInfo.Id)}</a>";
                    }
                    else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, BackgroundContentAttribute.Digg))
                    {
                        var showPopWinString = ModalContentDiggSet.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, contentInfo.Id);
                        var nums = BaiRongDataProvider.DiggDao.GetCount(publishmentSystemInfo.PublishmentSystemId, contentInfo.Id);
                        string display = $"赞同：{nums[0]} 不赞同：{nums[1]}";
                        value =
                            $@"<a href=""javascript:;"" onclick=""{showPopWinString}"" title=""点击设置Digg数"">{display}</a>";
                    }
                    else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, BackgroundContentAttribute.IsColor) || StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, BackgroundContentAttribute.IsHot) || StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, BackgroundContentAttribute.IsRecommend))
                    {
                        value = StringUtils.GetTrueImageHtml(contentInfo.GetExtendedAttribute(styleInfo.AttributeName));
                    }
                    else
                    {
                        isSettting = false;
                    }
                }

                if (!isSettting)
                {
                    value = InputParserUtility.GetContentByTableStyle(contentInfo.GetExtendedAttribute(styleInfo.AttributeName), publishmentSystemInfo, tableStyle, styleInfo);
                }
            }
            return value;
        }

        public static ArrayList GetAllTableStyleInfoArrayList(PublishmentSystemInfo publishmentSystemInfo, ETableStyle tableStyle, ArrayList tableStyleInfoArrayList)
        {
            var arraylist = new ArrayList();

            var styleInfo = new TableStyleInfo();
            styleInfo.AttributeName = ContentAttribute.Id;
            styleInfo.DisplayName = "编号";
            arraylist.Add(styleInfo);

            arraylist.AddRange(tableStyleInfoArrayList);

            styleInfo = new TableStyleInfo();
            styleInfo.AttributeName = ContentAttribute.Hits;
            styleInfo.DisplayName = "点击量";
            arraylist.Add(styleInfo);

            if (tableStyle == ETableStyle.BackgroundContent)
            {
                if (publishmentSystemInfo.Additional.IsRelatedByTags)
                {
                    styleInfo = new TableStyleInfo();
                    styleInfo.AttributeName = ContentAttribute.Tags;
                    styleInfo.DisplayName = "标签";
                    arraylist.Add(styleInfo);
                }

                styleInfo = new TableStyleInfo();
                styleInfo.AttributeName = BackgroundContentAttribute.Star;
                styleInfo.DisplayName = "评分";
                arraylist.Add(styleInfo);
            }
            else
            {
                styleInfo = new TableStyleInfo();
                styleInfo.AttributeName = ContentAttribute.AddDate;
                styleInfo.DisplayName = "添加时间";
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

            TableStyleInfo styleInfo = null;

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.AddUserName, 0, "添加者", string.Empty, true, true, false, EInputTypeUtils.GetValue(EInputType.Text), string.Empty, false, string.Empty);
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

        public static string GetColumnHeadRowsHtml(List<TableStyleInfo> tableStyleInfoArrayList, StringCollection attributesOfDisplay, ETableStyle tableStyle, PublishmentSystemInfo publishmentSystemInfo)
        {
            var builder = new StringBuilder();

            var arrayList = GetColumnTableStyleInfoList(publishmentSystemInfo, tableStyle, tableStyleInfoArrayList);
            foreach (var styleInfo in arrayList)
            {
                if (attributesOfDisplay.Contains(styleInfo.AttributeName))
                {
                    builder.Append(
                        $@"<td width=""{GetColumnWidth(tableStyle, styleInfo.AttributeName)}"">{styleInfo.DisplayName}</td>");
                }
            }

            return builder.ToString();
        }

        public static string GetCommandHeadRowsHtml(ETableStyle tableStyle, PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, string administratorName)
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

        public static string GetColumnItemRowsHtml(List<TableStyleInfo> styleInfoList, StringCollection attributesOfDisplay, Hashtable valueHashtable, ETableStyle tableStyle, PublishmentSystemInfo publishmentSystemInfo, ContentInfo contentInfo)
        {
            var builder = new StringBuilder();

            var arrayList = GetColumnTableStyleInfoList(publishmentSystemInfo, tableStyle, styleInfoList);
            foreach (var styleInfo in arrayList)
            {
                if (attributesOfDisplay.Contains(styleInfo.AttributeName))
                {
                    var value = GetColumnValue(valueHashtable, tableStyle, publishmentSystemInfo, contentInfo, styleInfo);
                    builder.Append($@"<td width=""{GetColumnWidth(tableStyle, styleInfo.AttributeName)}"">{value}</td>");
                }
            }

            return builder.ToString();
        }

        public static string GetCommandItemRowsHtml(ETableStyle tableStyle, PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, ContentInfo contentInfo, string pageUrl, string administratorName)
        {
            var builder = new StringBuilder();

            var modelType = EContentModelTypeUtils.GetEnumType(nodeInfo.ContentModelId);

            if (modelType == EContentModelType.Photo)
            {
                var contentPhotoUploadUrl = PageContentPhotoUpload.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, pageUrl);
                builder.Append(
                    $@"<td class=""center"" width=""50""><a href=""{contentPhotoUploadUrl}"">图片</a><span style=""color:gray"">({contentInfo
                        .Photos})</span></td>");
            }
            else if (modelType == EContentModelType.Job)
            {
                var resumeNum = DataProvider.ResumeContentDao.GetCount(publishmentSystemInfo.PublishmentSystemId, contentInfo.Id);
                var urlResume = PageResumeContent.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId, contentInfo.Id, StringUtils.ValueToUrl(pageUrl));;
                builder.Append(
                    $@"<td class=""center"" width=""50""><a href=""{urlResume}"">简历</a><span style=""color:gray"">({resumeNum})</span></td>");
            }

            if (publishmentSystemInfo.Additional.IsCommentable && modelType != EContentModelType.Job)
            {
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, AppManager.Cms.Permission.Channel.CommentCheck, AppManager.Cms.Permission.Channel.CommentDelete))
                {
                    //var urlComment = PageComment.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemID, contentInfo.NodeId, contentInfo.Id, pageUrl);
                    var urlComment = PageComments.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, pageUrl);
                    builder.Append(
                        $@"<td class=""center"" width=""50""><a href=""{urlComment}"">评论</a><span style=""color:gray"">({contentInfo
                            .Comments})</span></td>");
                }
            }

            return builder.ToString();
        }

        public static void Translate(PublishmentSystemInfo publishmentSystemInfo, int nodeID, int contentID, string translateCollection, ETranslateContentType translateType, string administratorName)
        {
            var translateArrayList = TranslateUtils.StringCollectionToStringList(translateCollection);
            foreach (string translate in translateArrayList)
            {
                if (!string.IsNullOrEmpty(translate))
                {
                    var translates = translate.Split('_');
                    if (translates.Length == 2)
                    {
                        var targetPublishmentSystemID = TranslateUtils.ToInt(translates[0]);
                        var targetNodeID = TranslateUtils.ToInt(translates[1]);

                        Translate(publishmentSystemInfo, nodeID, contentID, targetPublishmentSystemID, targetNodeID, translateType, administratorName);
                    }
                }
            }
        }

        public static void Translate(PublishmentSystemInfo publishmentSystemInfo, int nodeID, int contentID, int targetPublishmentSystemID, int targetNodeID, ETranslateContentType translateType, string administratorName)
        {
            if (publishmentSystemInfo == null || nodeID <= 0 || contentID <= 0 || targetPublishmentSystemID <= 0 || targetNodeID <= 0) return;

            var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemID);

            var targetTableName = NodeManager.GetTableName(targetPublishmentSystemInfo, targetNodeID);

            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeID);
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);

            var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentID);

            if (contentInfo != null)
            {
                if (translateType == ETranslateContentType.Copy)
                {
                    FileUtility.MoveFileByContentInfo(publishmentSystemInfo, targetPublishmentSystemInfo, contentInfo);

                    contentInfo.PublishmentSystemId = targetPublishmentSystemID;
                    contentInfo.SourceId = contentInfo.NodeId;
                    contentInfo.NodeId = targetNodeID;
                    contentInfo.Attributes[ContentAttribute.TranslateContentType] = ETranslateContentType.Copy.ToString();
                    //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, ETranslateContentType.Copy.ToString());
                    var theContentID = DataProvider.ContentDao.Insert(targetTableName, targetPublishmentSystemInfo, contentInfo);
                    if (EContentModelTypeUtils.IsPhoto(nodeInfo.ContentModelId))
                    {
                        var photoInfoList = DataProvider.PhotoDao.GetPhotoInfoList(publishmentSystemInfo.PublishmentSystemId, contentID);
                        if (photoInfoList.Count > 0)
                        {
                            foreach (var photoInfo in photoInfoList)
                            {
                                photoInfo.PublishmentSystemID = targetPublishmentSystemID;
                                photoInfo.ContentID = theContentID;

                                FileUtility.MoveFileByVirtaulUrl(publishmentSystemInfo, targetPublishmentSystemInfo, photoInfo.SmallUrl);
                                FileUtility.MoveFileByVirtaulUrl(publishmentSystemInfo, targetPublishmentSystemInfo, photoInfo.MiddleUrl);
                                FileUtility.MoveFileByVirtaulUrl(publishmentSystemInfo, targetPublishmentSystemInfo, photoInfo.LargeUrl);

                                DataProvider.PhotoDao.Insert(photoInfo);
                            }
                        }
                    }
                    if (contentInfo.IsChecked)
                    {
                        CreateManager.CreateContentAndTrigger(targetPublishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, theContentID);
                    }
                }
                else if (translateType == ETranslateContentType.Cut)
                {
                    FileUtility.MoveFileByContentInfo(publishmentSystemInfo, targetPublishmentSystemInfo, contentInfo);
                    contentInfo.PublishmentSystemId = targetPublishmentSystemID;
                    contentInfo.SourceId = contentInfo.NodeId;
                    contentInfo.NodeId = targetNodeID;
                    contentInfo.Attributes[ContentAttribute.TranslateContentType] = ETranslateContentType.Cut.ToString();
                    //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, ETranslateContentType.Cut.ToString());
                    if (StringUtils.EqualsIgnoreCase(tableName, targetTableName))
                    {
                        contentInfo.Taxis = DataProvider.ContentDao.GetTaxisToInsert(targetTableName, targetNodeID, contentInfo.IsTop);
                        DataProvider.ContentDao.Update(targetTableName, targetPublishmentSystemInfo, contentInfo);
                    }
                    else
                    {
                        DataProvider.ContentDao.Insert(targetTableName, targetPublishmentSystemInfo, contentInfo);
                        DataProvider.ContentDao.DeleteContents(publishmentSystemInfo.PublishmentSystemId, tableName, TranslateUtils.ToIntList(contentID), nodeID);
                    }

                    DataProvider.NodeDao.UpdateContentNum(publishmentSystemInfo, nodeID, true);
                    DataProvider.NodeDao.UpdateContentNum(targetPublishmentSystemInfo, targetNodeID, true);

                    if (EContentModelTypeUtils.IsPhoto(nodeInfo.ContentModelId))
                    {
                        var photoInfoList = DataProvider.PhotoDao.GetPhotoInfoList(publishmentSystemInfo.PublishmentSystemId, contentID);
                        if (photoInfoList.Count > 0)
                        {
                            foreach (var photoInfo in photoInfoList)
                            {
                                photoInfo.PublishmentSystemID = targetPublishmentSystemID;

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
                    if (contentInfo.ReferenceId == 0)
                    {
                        contentInfo.PublishmentSystemId = targetPublishmentSystemID;
                        contentInfo.SourceId = contentInfo.NodeId;
                        contentInfo.NodeId = targetNodeID;
                        contentInfo.ReferenceId = contentID;
                        contentInfo.Attributes[ContentAttribute.TranslateContentType] = ETranslateContentType.Reference.ToString();
                        //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, ETranslateContentType.Reference.ToString());
                        DataProvider.ContentDao.Insert(targetTableName, targetPublishmentSystemInfo, contentInfo);
                    }
                }
                else if (translateType == ETranslateContentType.ReferenceContent)
                {
                    if (contentInfo.ReferenceId == 0)
                    {
                        contentInfo.PublishmentSystemId = targetPublishmentSystemID;
                        contentInfo.SourceId = contentInfo.NodeId;
                        contentInfo.NodeId = targetNodeID;
                        contentInfo.ReferenceId = contentID;
                        contentInfo.Attributes[ContentAttribute.TranslateContentType] = ETranslateContentType.ReferenceContent.ToString();
                        var theContentID = DataProvider.ContentDao.Insert(targetTableName, targetPublishmentSystemInfo, contentInfo);
                        if (EContentModelTypeUtils.IsPhoto(nodeInfo.ContentModelId))
                        {
                            var photoInfoList = DataProvider.PhotoDao.GetPhotoInfoList(publishmentSystemInfo.PublishmentSystemId, contentID);
                            if (photoInfoList.Count > 0)
                            {
                                foreach (var photoInfo in photoInfoList)
                                {
                                    photoInfo.PublishmentSystemID = targetPublishmentSystemID;
                                    photoInfo.ContentID = theContentID;

                                    FileUtility.MoveFileByVirtaulUrl(publishmentSystemInfo, targetPublishmentSystemInfo, photoInfo.SmallUrl);
                                    FileUtility.MoveFileByVirtaulUrl(publishmentSystemInfo, targetPublishmentSystemInfo, photoInfo.MiddleUrl);
                                    FileUtility.MoveFileByVirtaulUrl(publishmentSystemInfo, targetPublishmentSystemInfo, photoInfo.LargeUrl);

                                    DataProvider.PhotoDao.Insert(photoInfo);
                                }
                            }
                        }

                        FileUtility.MoveFileByContentInfo(publishmentSystemInfo, targetPublishmentSystemInfo, contentInfo);

                        if (contentInfo.IsChecked)
                        {
                            CreateManager.CreateContentAndTrigger(targetPublishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, theContentID);
                        }
                    }
                }
            }
        }

        public static Hashtable GetIDsHashtable(NameValueCollection queryString)
        {
            var idsHashtable = new Hashtable();

            if (!string.IsNullOrEmpty(queryString["IDsCollection"]))
            {
                var idsArrayList = TranslateUtils.StringCollectionToStringList(queryString["IDsCollection"]);
                foreach (string ids in idsArrayList)
                {
                    var nodeID = TranslateUtils.ToInt(ids.Split('_')[0]);
                    var contentID = TranslateUtils.ToInt(ids.Split('_')[1]);
                    var contentIDArrayList = idsHashtable[nodeID] as ArrayList;
                    if (contentIDArrayList == null)
                    {
                        contentIDArrayList = new ArrayList();
                    }
                    contentIDArrayList.Add(contentID);
                    idsHashtable[nodeID] = contentIDArrayList;
                }
            }
            else
            {
                var nodeID = TranslateUtils.ToInt(queryString["NodeID"]);
                var contentIDArrayList = TranslateUtils.StringCollectionToIntList(queryString["ContentIDCollection"]);
                idsHashtable[nodeID] = contentIDArrayList;
            }

            return idsHashtable;
        }

        public static string GetTagStyleOpenWindowStringToAdd(string elementName, int publishmentSystemId)
        {
            if (StringUtils.EqualsIgnoreCase(elementName, StlGovPublicApply.ElementName))
            {
                return ModalTagStyleGovPublicApplyAdd.GetOpenWindowStringToAdd(publishmentSystemId);
            }
            else if (StringUtils.EqualsIgnoreCase(elementName, StlGovPublicQuery.ElementName))
            {
                return ModalTagStyleGovPublicQueryAdd.GetOpenWindowStringToAdd(publishmentSystemId);
            }
            else if (StringUtils.EqualsIgnoreCase(elementName, StlGovInteractApply.ElementName))
            {
                return ModalTagStyleGovInteractApplyAdd.GetOpenWindowStringToAdd(publishmentSystemId);
            }
            else if (StringUtils.EqualsIgnoreCase(elementName, StlGovInteractQuery.ElementName))
            {
                return ModalTagStyleGovInteractQueryAdd.GetOpenWindowStringToAdd(publishmentSystemId);
            }

            return string.Empty;
        }

        public static string GetTagStyleOpenWindowStringToEdit(string elementName, int publishmentSystemId, int styleId)
        {
            if (StringUtils.EqualsIgnoreCase(elementName, StlGovPublicApply.ElementName))
            {
                return ModalTagStyleGovPublicApplyAdd.GetOpenWindowStringToEdit(publishmentSystemId, styleId);
            }
            else if (StringUtils.EqualsIgnoreCase(elementName, StlGovPublicQuery.ElementName))
            {
                return ModalTagStyleGovPublicQueryAdd.GetOpenWindowStringToEdit(publishmentSystemId, styleId);
            }
            else if (StringUtils.EqualsIgnoreCase(elementName, StlGovInteractApply.ElementName))
            {
                return ModalTagStyleGovInteractApplyAdd.GetOpenWindowStringToEdit(publishmentSystemId, styleId);
            }
            else if (StringUtils.EqualsIgnoreCase(elementName, StlGovInteractQuery.ElementName))
            {
                return ModalTagStyleGovInteractQueryAdd.GetOpenWindowStringToEdit(publishmentSystemId, styleId);
            }
            return string.Empty;
        }
    }
}
