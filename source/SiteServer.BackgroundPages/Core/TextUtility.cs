using BaiRong.Core.Model;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using System.Text;
using BaiRong.Core;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Controllers.Plugins;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin.Features;
using SiteServer.CMS.Core.User;

namespace SiteServer.BackgroundPages.Core
{
    public class TextUtility
    {
        private TextUtility()
        {
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
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.IsTop) || StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.IsColor) || StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.IsHot) || StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.IsRecommend))
            {
                value = StringUtils.GetTrueImageHtml(contentInfo.GetExtendedAttribute(styleInfo.AttributeName));
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

        public static string GetColumnHeadRowsHtml(List<TableStyleInfo> tableStyleInfoArrayList, StringCollection attributesOfDisplay, ETableStyle tableStyle, PublishmentSystemInfo publishmentSystemInfo)
        {
            var builder = new StringBuilder();

            var arrayList = ContentUtility.GetColumnTableStyleInfoList(publishmentSystemInfo, tableStyle, tableStyleInfoArrayList);
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

        public static string GetCommandHeadRowsHtml(string administratorName, PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, Dictionary<string, IChannel> pluginChannels)
        {
            var builder = new StringBuilder();

            foreach (var pluginId in pluginChannels.Keys)
            {
                var pluginChannel = pluginChannels[pluginId];
                if (pluginChannel?.ContentLinks != null && pluginChannel.ContentLinks.Count > 0)
                {
                    for (var i = 0; i < pluginChannel.ContentLinks.Count; i++)
                    {
                        builder.Append(@"<td class=""center"" width=""80"">&nbsp;</td>");
                    }
                }
            }

            if (publishmentSystemInfo.Additional.IsCommentable)
            {
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Permissions.Channel.CommentCheck, AppManager.Permissions.Channel.CommentDelete))
                {
                    builder.Append(@"<td class=""center"" width=""50"">&nbsp;</td>");
                }
            }
            return builder.ToString();
        }

        public static string GetColumnItemRowsHtml(List<TableStyleInfo> styleInfoList, StringCollection attributesOfDisplay, Hashtable valueHashtable, ETableStyle tableStyle, PublishmentSystemInfo publishmentSystemInfo, ContentInfo contentInfo)
        {
            var builder = new StringBuilder();

            var arrayList = ContentUtility.GetColumnTableStyleInfoList(publishmentSystemInfo, tableStyle, styleInfoList);
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

        public static string GetCommandItemRowsHtml(PublishmentSystemInfo publishmentSystemInfo, Dictionary<string, IChannel> pluginChannels, ContentInfo contentInfo, string pageUrl, string administratorName)
        {
            var builder = new StringBuilder();

            foreach (var pluginId in pluginChannels.Keys)
            {
                var pluginChannel = pluginChannels[pluginId];
                if (pluginChannel?.ContentLinks != null && pluginChannel.ContentLinks.Count > 0)
                {
                    var apiUrl = JsonApi.GetUrl(PageUtility.GetInnerApiUrl(publishmentSystemInfo), pluginId);
                    apiUrl = PageUtils.AddProtocolToUrl(apiUrl);
                    foreach (var link in pluginChannel.ContentLinks)
                    {
                        var href = PageUtils.AddQueryString(PageUtils.GetPluginDirectoryUrl(pluginId, link.Href), new NameValueCollection
                    {
                        {"apiUrl", apiUrl},
                        {"publishmentSystemId", publishmentSystemInfo.PublishmentSystemId.ToString()},
                        {"channelId", contentInfo.NodeId.ToString()},
                        {"contentId", contentInfo.Id.ToString()},
                        {"returnUrl", StringUtils.ValueToUrl(pageUrl)}
                    });
                        builder.Append(
                            $@"<td class=""center"" width=""80""><a href=""{href}"" {(string.IsNullOrEmpty(link.Target) ? string.Empty : "target='" + link.Target + "'")}>{link.Text}</a></td>");
                    }
                }
            }

            //if (modelType == EContentModelType.Photo)
            //{
            //    var contentPhotoUploadUrl = PageContentPhotoUpload.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, pageUrl);
            //    builder.Append(
            //        $@"<td class=""center"" width=""50""><a href=""{contentPhotoUploadUrl}"">图片</a><span style=""color:gray"">({contentInfo
            //            .Photos})</span></td>");
            //}
            //else if (modelType == EContentModelType.Job)
            //{
            //    var resumeNum = DataProvider.ResumeContentDao.GetCount(publishmentSystemInfo.PublishmentSystemId, contentInfo.Id);
            //    var urlResume = PageResumeContent.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId, contentInfo.Id, StringUtils.ValueToUrl(pageUrl));;
            //    builder.Append(
            //        $@"<td class=""center"" width=""50""><a href=""{urlResume}"">简历</a><span style=""color:gray"">({resumeNum})</span></td>");
            //}

            if (publishmentSystemInfo.Additional.IsCommentable)
            {
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, AppManager.Permissions.Channel.CommentCheck, AppManager.Permissions.Channel.CommentDelete))
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

        public static bool IsEdit(PublishmentSystemInfo publishmentSystemInfo, int nodeId, string administratorName)
        {
            return AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeId, AppManager.Permissions.Channel.ContentEdit);
        }

        public static bool IsComment(PublishmentSystemInfo publishmentSystemInfo, int nodeId, string administratorName)
        {
            return publishmentSystemInfo.Additional.IsCommentable && AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeId, AppManager.Permissions.Channel.CommentCheck, AppManager.Permissions.Channel.CommentDelete);
        }

        public static int GetContentLinkCount(PublishmentSystemInfo publishmentSystemInfo, Dictionary<string, IChannel> pluginChannels)
        {
            var count = 0;
            foreach (var pluginChannel in pluginChannels.Values)
            {
                if (pluginChannel?.ContentLinks != null)
                {
                    count += pluginChannel.ContentLinks.Count;
                }
            }

            return count;
        }

        public static string GetCommandHtml(PublishmentSystemInfo publishmentSystemInfo, Dictionary<string, IChannel> pluginChannels, ContentInfo contentInfo, string pageUrl, string administratorName, bool isEdit, bool isComment)
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
                    var apiUrl = JsonApi.GetUrl(PageUtility.GetInnerApiUrl(publishmentSystemInfo), pluginId);
                    apiUrl = PageUtils.AddProtocolToUrl(apiUrl);
                    foreach (var link in pluginChannel.ContentLinks)
                    {
                        var href = PageUtils.AddQueryString(PageUtils.GetPluginDirectoryUrl(pluginId, link.Href), new NameValueCollection
                    {
                        {"apiUrl", apiUrl},
                        {"publishmentSystemId", publishmentSystemInfo.PublishmentSystemId.ToString()},
                        {"channelId", contentInfo.NodeId.ToString()},
                        {"contentId", contentInfo.Id.ToString()},
                        {"returnUrl", StringUtils.ValueToUrl(pageUrl)}
                    });
                        builder.Append(
                            $@"<a style=""margin:0 5px"" href=""{href}"" {(string.IsNullOrEmpty(link.Target) ? string.Empty : "target='" + link.Target + "'")}>{link.Text}</a>");
                    }
                }
            }

            return builder.ToString();
        }
    }
}
