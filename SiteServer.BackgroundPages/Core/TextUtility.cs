using BaiRong.Core.Model;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using System.Text;
using BaiRong.Core;
using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core.Model.Attributes;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin.Features;

namespace SiteServer.BackgroundPages.Core
{
    public class TextUtility
    {
        private TextUtility()
        {
        }

        public static int GetColumnWidth(string attributeName)
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
                width = 140;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.LastHitsDate) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.CheckCheckDate))
            {
                width = 140;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, BackgroundContentAttribute.Digg) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.CheckReasons))
            {
                width = 110;
            }
            return width;
        }

        private static string GetColumnValue(Dictionary<string, string> nameValueCacheDict, PublishmentSystemInfo publishmentSystemInfo, ContentInfo contentInfo, TableStyleInfo styleInfo)
        {
            var value = string.Empty;
            if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.AddUserName))
            {
                if (!string.IsNullOrEmpty(contentInfo.AddUserName))
                {
                    var key = ContentAttribute.AddUserName + ":" + contentInfo.AddUserName;
                    if (!nameValueCacheDict.TryGetValue(key, out value))
                    {
                        value = AdminManager.GetDisplayName(contentInfo.AddUserName, false);
                        nameValueCacheDict[key] = value;
                    }
                }
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.LastEditUserName))
            {
                if (!string.IsNullOrEmpty(contentInfo.LastEditUserName))
                {
                    var key = ContentAttribute.LastEditUserName + ":" + contentInfo.LastEditUserName;
                    if (!nameValueCacheDict.TryGetValue(key, out value))
                    {
                        value =
                            $@"<a rel=""popover"" class=""popover-hover"" data-content=""{AdminManager.GetFullName(
                                contentInfo.LastEditUserName)}"" data-original-title=""管理员"">{AdminManager.GetDisplayName(
                                contentInfo.LastEditUserName, false)}</a>";
                        nameValueCacheDict[key] = value;
                    }
                }
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.CheckUserName))
            {
                var checkUserName = contentInfo.GetString(ContentAttribute.CheckUserName);
                if (!string.IsNullOrEmpty(checkUserName))
                {
                    var key = ContentAttribute.CheckUserName + ":" + checkUserName;
                    if (!nameValueCacheDict.TryGetValue(key, out value))
                    {
                        value =
                            $@"<a rel=""popover"" class=""popover-hover"" data-content=""{AdminManager.GetFullName(
                                checkUserName)}"" data-original-title=""管理员"">{AdminManager.GetDisplayName(checkUserName,
                                false)}</a>";
                        nameValueCacheDict[key] = value;
                    }
                }
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.CheckCheckDate))
            {
                var checkDate = contentInfo.GetString(ContentAttribute.CheckCheckDate);
                if (!string.IsNullOrEmpty(checkDate))
                {
                    value = DateUtils.GetDateAndTimeString(TranslateUtils.ToDateTime(checkDate));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.CheckReasons))
            {
                value = contentInfo.GetString(ContentAttribute.CheckReasons);
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
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.IsTop) || StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.IsColor) || StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.IsHot) || StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.IsRecommend))
            {
                value = StringUtils.GetTrueImageHtml(contentInfo.GetString(styleInfo.AttributeName));
            }
            else
            {
                var isSettting = true;
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
                else
                {
                    isSettting = false;
                }

                if (!isSettting)
                {
                    value = InputParserUtility.GetContentByTableStyle(contentInfo.GetString(styleInfo.AttributeName), publishmentSystemInfo, styleInfo);
                }
            }
            return value;
        }

        public static bool IsEdit(PublishmentSystemInfo publishmentSystemInfo, int nodeId, string administratorName)
        {
            return AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeId, AppManager.Permissions.Channel.ContentEdit);
        }

        public static bool IsComment(PublishmentSystemInfo publishmentSystemInfo, int nodeId, string administratorName)
        {
            return publishmentSystemInfo.Additional.IsCommentable && AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeId, AppManager.Permissions.Channel.CommentCheck, AppManager.Permissions.Channel.CommentDelete);
        }

        public static string GetColumnsHeadHtml(List<TableStyleInfo> tableStyleInfoArrayList, StringCollection attributesOfDisplay, PublishmentSystemInfo publishmentSystemInfo)
        {
            var builder = new StringBuilder();

            var arrayList = ContentUtility.GetColumnTableStyleInfoList(publishmentSystemInfo, tableStyleInfoArrayList);
            foreach (var styleInfo in arrayList)
            {
                if (attributesOfDisplay.Contains(styleInfo.AttributeName))
                {
                    builder.Append(
                        $@"<th style=""width: {GetColumnWidth(styleInfo.AttributeName)}px"">{styleInfo.DisplayName}</th>");
                }
            }

            return builder.ToString();
        }

        public static string GetCommandsHeadHtml(PublishmentSystemInfo publishmentSystemInfo, Dictionary<string, IContentRelated> pluginChannels, bool isEdit, bool isComment)
        {
            var commandCount = 0;
            if (isEdit)
            {
                commandCount += 1;
            }
            if (isComment)
            {
                commandCount += 1;
            }

            foreach (var pluginChannel in pluginChannels.Values)
            {
                if (pluginChannel?.ContentLinks != null)
                {
                    commandCount += pluginChannel.ContentLinks.Count;
                }
            }

            return $@"<th style=""width: {commandCount * 80}px"" class=""text-center"">操作</th>";
        }

        public static string GetColumnsHtml(Dictionary<string, string> nameValueCacheDict, PublishmentSystemInfo publishmentSystemInfo, ContentInfo contentInfo, StringCollection attributesOfDisplay, List<TableStyleInfo> displayStyleInfoList)
        {
            var builder = new StringBuilder();

            foreach (var styleInfo in displayStyleInfoList)
            {
                if (!attributesOfDisplay.Contains(styleInfo.AttributeName)) continue;

                var value = GetColumnValue(nameValueCacheDict, publishmentSystemInfo, contentInfo, styleInfo);
                builder.Append($@"<td width=""{GetColumnWidth(styleInfo.AttributeName)}"">{value}</td>");
            }

            return builder.ToString();
        }

        public static string GetCommandsHtml(PublishmentSystemInfo publishmentSystemInfo, Dictionary<string, IContentRelated> pluginChannels, ContentInfo contentInfo, string pageUrl, string administratorName, bool isEdit, bool isComment)
        {
            var builder = new StringBuilder();

            if (isEdit || administratorName == contentInfo.AddUserName)
            {
                builder.Append($@"<a style=""margin:0 5px"" href=""{PageContentAdd.GetRedirectUrlOfEdit(publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, pageUrl)}"">编辑</a>");
            }

            if (isComment)
            {
                //var urlComment = PageComment.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemID, contentInfo.NodeId, contentInfo.Id, pageUrl);
                var urlComment = PageComments.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, pageUrl);
                builder.Append(
                    $@"<a style=""margin:0 5px"" href=""{urlComment}"">评论<span style=""color:gray"">({contentInfo
                        .Comments})</span></a>");
            }

            foreach (var pluginId in pluginChannels.Keys)
            {
                var pluginChannel = pluginChannels[pluginId];
                if (pluginChannel?.ContentLinks != null && pluginChannel.ContentLinks.Count > 0)
                {
                    foreach (var link in pluginChannel.ContentLinks)
                    {
                        var href = PluginManager.GetMenuContentHref(pluginId, link.Href,
                            publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, pageUrl);
                        builder.Append(
                            $@"<a style=""margin:0 5px"" href=""{href}"" {(string.IsNullOrEmpty(link.Target) ? string.Empty : "target='" + link.Target + "'")}>{link.Text}</a>");
                    }
                }
            }

            return builder.ToString();
        }

        //public static string GetCommandItemRowsHtml(PublishmentSystemInfo publishmentSystemInfo, Dictionary<string, IContentRelated> pluginChannels, ContentInfo contentInfo, string pageUrl, string administratorName)
        //{
        //    var builder = new StringBuilder();

        //    foreach (var pluginId in pluginChannels.Keys)
        //    {
        //        var pluginChannel = pluginChannels[pluginId];
        //        if (pluginChannel?.ContentLinks != null && pluginChannel.ContentLinks.Count > 0)
        //        {
        //            foreach (var link in pluginChannel.ContentLinks)
        //            {
        //                var href = PluginUtils.GetMenuContentHref(pluginId, link.Href,
        //                    publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, pageUrl);
        //                builder.Append(
        //                    $@"<td class=""center"" width=""80""><a href=""{href}"" {(string.IsNullOrEmpty(link.Target) ? string.Empty : "target='" + link.Target + "'")}>{link.Text}</a></td>");
        //            }
        //        }
        //    }

        //    //if (modelType == EContentModelType.Photo)
        //    //{
        //    //    var contentPhotoUploadUrl = PageContentPhotoUpload.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, pageUrl);
        //    //    builder.Append(
        //    //        $@"<td class=""center"" width=""50""><a href=""{contentPhotoUploadUrl}"">图片</a><span style=""color:gray"">({contentInfo
        //    //            .Photos})</span></td>");
        //    //}
        //    //else if (modelType == EContentModelType.Job)
        //    //{
        //    //    var resumeNum = DataProvider.ResumeContentDao.GetCount(publishmentSystemInfo.PublishmentSystemId, contentInfo.Id);
        //    //    var urlResume = PageResumeContent.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId, contentInfo.Id, StringUtils.ValueToUrl(pageUrl));;
        //    //    builder.Append(
        //    //        $@"<td class=""center"" width=""50""><a href=""{urlResume}"">简历</a><span style=""color:gray"">({resumeNum})</span></td>");
        //    //}

        //    if (publishmentSystemInfo.Additional.IsCommentable)
        //    {
        //        if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, AppManager.Permissions.Channel.CommentCheck, AppManager.Permissions.Channel.CommentDelete))
        //        {
        //            //var urlComment = PageComment.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemID, contentInfo.NodeId, contentInfo.Id, pageUrl);
        //            var urlComment = PageComments.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, pageUrl);
        //            builder.Append(
        //                $@"<td class=""center"" width=""50""><a href=""{urlComment}"">评论</a><span style=""color:gray"">({contentInfo
        //                    .Comments})</span></td>");
        //        }
        //    }

        //    return builder.ToString();
        //}
    }
}
