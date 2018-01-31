using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using System.Text;
using SiteServer.Utils;
using System.Collections.Generic;
using System.Collections.Specialized;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using System.Web.UI.WebControls;

namespace SiteServer.BackgroundPages.Core
{
    public class TextUtility
    {
        private TextUtility()
        {
        }

        private static string GetColumnValue(Dictionary<string, string> nameValueCacheDict, SiteInfo siteInfo, ContentInfo contentInfo, TableStyleInfo styleInfo)
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
                        value = AdminManager.GetDisplayName(contentInfo.LastEditUserName, false);
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
                        value = AdminManager.GetDisplayName(checkUserName, false);
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
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.GroupNameCollection))
            {
                value = contentInfo.GroupNameCollection;
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
                value = InputParserUtility.GetContentByTableStyle(contentInfo.GetString(styleInfo.AttributeName), siteInfo, styleInfo);
            }
            return value;
        }

        public static bool IsEdit(SiteInfo siteInfo, int channelId, string administratorName)
        {
            return AdminUtility.HasChannelPermissions(administratorName, siteInfo.Id, channelId, ConfigManager.Permissions.Channel.ContentEdit);
        }

        //public static bool IsComment(SiteInfo siteInfo, int channelId, string administratorName)
        //{
        //    return siteInfo.Additional.IsCommentable && AdminUtility.HasChannelPermissions(administratorName, siteInfo.Id, channelId, ConfigManager.Permissions.Channel.CommentCheck, ConfigManager.Permissions.Channel.CommentDelete);
        //}

        public static string GetColumnsHeadHtml(List<TableStyleInfo> tableStyleInfoArrayList, StringCollection attributesOfDisplay, SiteInfo siteInfo)
        {
            var builder = new StringBuilder();

            var arrayList = ContentUtility.GetColumnTableStyleInfoList(siteInfo, tableStyleInfoArrayList);
            foreach (var styleInfo in arrayList)
            {
                if (attributesOfDisplay.Contains(styleInfo.AttributeName))
                {
                    builder.Append(
                        $@"<th class=""text-nowrap"">{styleInfo.DisplayName}</th>");
                }
            }

            return builder.ToString();
        }

        public static string GetColumnsHtml(Dictionary<string, string> nameValueCacheDict, SiteInfo siteInfo, ContentInfo contentInfo, StringCollection attributesOfDisplay, List<TableStyleInfo> displayStyleInfoList)
        {
            var builder = new StringBuilder();

            foreach (var styleInfo in displayStyleInfoList)
            {
                if (!attributesOfDisplay.Contains(styleInfo.AttributeName)) continue;

                var value = GetColumnValue(nameValueCacheDict, siteInfo, contentInfo, styleInfo);
                builder.Append($@"<td class=""text-nowrap"">{value}</td>");
            }

            return builder.ToString();
        }

        public static string GetCommandsHtml(SiteInfo siteInfo, Dictionary<string, List<HyperLink>> pluginLinks, ContentInfo contentInfo, string pageUrl, string administratorName, bool isEdit)
        {
            var builder = new StringBuilder();

            if (isEdit || administratorName == contentInfo.AddUserName)
            {
                builder.Append($@"<a href=""{PageContentAdd.GetRedirectUrlOfEdit(siteInfo.Id, contentInfo.ChannelId, contentInfo.Id, pageUrl)}"">编辑</a>");
            }

            //if (isComment)
            //{
            //    //var urlComment = PageComment.GetRedirectUrl(siteInfo.SiteId, contentInfo.ChannelId, contentInfo.Id, pageUrl);
            //    var urlComment = PageComments.GetRedirectUrl(siteInfo.Id, contentInfo.ChannelId, contentInfo.Id, pageUrl);
            //    builder.Append(
            //        $@"<a style=""margin:0 5px"" href=""{urlComment}"">评论<span style=""color:gray"">({contentInfo
            //            .Comments})</span></a>");
            //}

            if (pluginLinks != null)
            {
                foreach (var pluginId in pluginLinks.Keys)
                {
                    var contentLinks = pluginLinks[pluginId];
                    if (contentLinks != null && contentLinks.Count > 0)
                    {
                        foreach (var link in contentLinks)
                        {
                            var originalUrl = link.NavigateUrl;
                            link.NavigateUrl = PluginMenuManager.GetMenuContentHref(pluginId, originalUrl, siteInfo.Id, contentInfo.ChannelId, contentInfo.Id, pageUrl);

                            builder.Append("&nbsp;&nbsp;").Append(ControlUtils.GetControlRenderHtml(link));

                            link.NavigateUrl = originalUrl;

                            //builder.Append(
                            //    $@"<a style=""margin:0 5px"" href=""{href}"" {(string.IsNullOrEmpty(link.Target) ? string.Empty : "target='" + link.Target + "'")}>{link.Text}</a>");
                        }
                    }
                }
            }

            return builder.ToString();
        }

        //public static string GetCommandItemRowsHtml(SiteInfo siteInfo, Dictionary<string, IContentRelated> pluginChannels, ContentInfo contentInfo, string pageUrl, string administratorName)
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
        //                    siteInfo.Id, contentInfo.ChannelId, contentInfo.Id, pageUrl);
        //                builder.Append(
        //                    $@"<td class=""center"" width=""80""><a href=""{href}"" {(string.IsNullOrEmpty(link.Target) ? string.Empty : "target='" + link.Target + "'")}>{link.Text}</a></td>");
        //            }
        //        }
        //    }

        //    //if (modelType == EContentModelType.Photo)
        //    //{
        //    //    var contentPhotoUploadUrl = PageContentPhotoUpload.GetRedirectUrl(siteInfo.Id, contentInfo.ChannelId, contentInfo.Id, pageUrl);
        //    //    builder.Append(
        //    //        $@"<td class=""center"" width=""50""><a href=""{contentPhotoUploadUrl}"">图片</a><span style=""color:gray"">({contentInfo
        //    //            .Photos})</span></td>");
        //    //}
        //    //else if (modelType == EContentModelType.Job)
        //    //{
        //    //    var resumeNum = DataProvider.ResumeContentDao.GetCount(siteInfo.Id, contentInfo.Id);
        //    //    var urlResume = PageResumeContent.GetRedirectUrl(siteInfo.Id, contentInfo.Id, StringUtils.ValueToUrl(pageUrl));;
        //    //    builder.Append(
        //    //        $@"<td class=""center"" width=""50""><a href=""{urlResume}"">简历</a><span style=""color:gray"">({resumeNum})</span></td>");
        //    //}

        //    if (siteInfo.Additional.IsCommentable)
        //    {
        //        if (AdminUtility.HasChannelPermissions(administratorName, siteInfo.Id, contentInfo.ChannelId, ConfigManager.Permissions.Channel.CommentCheck, ConfigManager.Permissions.Channel.CommentDelete))
        //        {
        //            //var urlComment = PageComment.GetRedirectUrl(siteInfo.SiteId, contentInfo.ChannelId, contentInfo.Id, pageUrl);
        //            var urlComment = PageComments.GetRedirectUrl(siteInfo.Id, contentInfo.ChannelId, contentInfo.Id, pageUrl);
        //            builder.Append(
        //                $@"<td class=""center"" width=""50""><a href=""{urlComment}"">评论</a><span style=""color:gray"">({contentInfo
        //                    .Comments})</span></td>");
        //        }
        //    }

        //    return builder.ToString();
        //}
    }
}
