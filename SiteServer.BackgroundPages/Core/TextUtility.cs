using System;
using SiteServer.CMS.Model;
using System.Text;
using SiteServer.Utils;
using System.Collections.Generic;
using System.Collections.Specialized;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;

namespace SiteServer.BackgroundPages.Core
{
    public static class TextUtility
    {
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
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.CheckDate))
            {
                var checkDate = contentInfo.GetDateTime(ContentAttribute.CheckDate, DateTime.MinValue);
                if (checkDate != DateTime.MinValue)
                {
                    value = DateUtils.GetDateAndTimeString(checkDate);
                }
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.CheckReasons))
            {
                value = contentInfo.GetString(ContentAttribute.CheckReasons);
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.AddDate))
            {
                value = DateUtils.GetDateAndTimeString(contentInfo.AddDate);
            }
            else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.LastEditDate))
            {
                value = DateUtils.GetDateAndTimeString(contentInfo.LastEditDate);
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

        public static bool IsEdit(SiteInfo siteInfo, int channelId, PermissionsImpl permissionsImpl)
        {
            return permissionsImpl.HasChannelPermissions(siteInfo.Id, channelId, ConfigManager.ChannelPermissions.ContentEdit);
        }

        //public static bool IsComment(SiteInfo siteInfo, int channelId, string administratorName)
        //{
        //    return siteInfo.Additional.IsCommentable && AdminUtility.HasChannelPermissions(administratorName, siteInfo.Id, channelId, ConfigManager.Permissions.Channel.CommentCheck, ConfigManager.Permissions.Channel.CommentDelete);
        //}

        public static string GetColumnsHeadHtml(List<TableStyleInfo> tableStyleInfoList, Dictionary<string, Dictionary<string, Func<IContentContext, string>>> pluginColumns, StringCollection attributesOfDisplay)
        {
            var builder = new StringBuilder();

            var styleInfoList = ContentUtility.GetAllTableStyleInfoList(tableStyleInfoList);

            foreach (var styleInfo in styleInfoList)
            {
                if (!attributesOfDisplay.Contains(styleInfo.AttributeName) || styleInfo.AttributeName == ContentAttribute.Title) continue;
                builder.Append($@"<th class=""text-nowrap"">{styleInfo.DisplayName}</th>");
            }

            if (pluginColumns != null)
            {
                foreach (var pluginId in pluginColumns.Keys)
                {
                    var contentColumns = pluginColumns[pluginId];
                    if (contentColumns == null || contentColumns.Count == 0) continue;

                    foreach (var columnName in contentColumns.Keys)
                    {
                        var attributeName = $"{pluginId}:{columnName}";
                        if (!attributesOfDisplay.Contains(attributeName)) continue;

                        builder.Append($@"<th class=""text-nowrap"">{columnName}</th>");
                    }
                }
            }

            return builder.ToString();
        }

        public static string GetColumnsHtml(Dictionary<string, string> nameValueCacheDict, SiteInfo siteInfo, ContentInfo contentInfo, StringCollection attributesOfDisplay, List<TableStyleInfo> displayStyleInfoList, Dictionary<string, Dictionary<string, Func<IContentContext, string>>> pluginColumns)
        {
            var builder = new StringBuilder();

            foreach (var styleInfo in displayStyleInfoList)
            {
                if (!attributesOfDisplay.Contains(styleInfo.AttributeName) || styleInfo.AttributeName == ContentAttribute.Title) continue;

                var value = GetColumnValue(nameValueCacheDict, siteInfo, contentInfo, styleInfo);
                builder.Append($@"<td class=""text-nowrap"">{value}</td>");
            }

            if (pluginColumns != null)
            {
                foreach (var pluginId in pluginColumns.Keys)
                {
                    var contentColumns = pluginColumns[pluginId];
                    if (contentColumns == null || contentColumns.Count == 0) continue;

                    foreach (var columnName in contentColumns.Keys)
                    {
                        var attributeName = $"{pluginId}:{columnName}";
                        if (!attributesOfDisplay.Contains(attributeName)) continue;

                        try
                        {
                            var func = contentColumns[columnName];
                            var value = func(new ContentContextImpl
                            {
                                SiteId = contentInfo.SiteId,
                                ChannelId = contentInfo.ChannelId,
                                ContentId = contentInfo.Id
                            });
                            builder.Append($@"<td class=""text-nowrap"">{value}</td>");
                        }
                        catch (Exception ex)
                        {
                            LogUtils.AddErrorLog(pluginId, ex);
                        }
                    }
                }
            }

            return builder.ToString();
        }

        public static string GetCommandsHtml(SiteInfo siteInfo, List<Menu> pluginMenus, ContentInfo contentInfo, string pageUrl, string administratorName, bool isEdit)
        {
            var builder = new StringBuilder();

            if (isEdit || administratorName == contentInfo.AddUserName)
            {
                builder.Append($@"<a href=""{PageContentAdd.GetRedirectUrlOfEdit(siteInfo.Id, contentInfo.ChannelId, contentInfo.Id, pageUrl)}"">编辑</a>");
            }

            if (pluginMenus != null)
            {
                foreach (var menu in pluginMenus)
                {
                    builder.Append(string.IsNullOrEmpty(menu.Target)
                        ? $@"<a class=""m-l-5"" href=""javascript:;"" onclick=""{LayerUtils.GetOpenScript(menu.Text, menu.Href)}"">{menu.Text}</a>"
                        : $@"<a class=""m-l-5"" href=""{menu.Href}"" target=""{menu.Target}"">{menu.Text}</a>");
                }
            }

            return builder.ToString();
        }
    }
}
