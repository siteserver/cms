using System;
using SiteServer.Abstractions;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.Repositories;


namespace SiteServer.BackgroundPages.Core
{
    public static class TextUtility
    {
        private static async Task<string> GetColumnValueAsync(Dictionary<string, string> nameValueCacheDict, Site site, Content content, TableStyle style)
        {
            var value = string.Empty;
            if (StringUtils.EqualsIgnoreCase(style.AttributeName, ContentAttribute.AddUserName))
            {
                if (!string.IsNullOrEmpty(content.AddUserName))
                {
                    var key = ContentAttribute.AddUserName + ":" + content.AddUserName;
                    if (!nameValueCacheDict.TryGetValue(key, out value))
                    {
                        value = await DataProvider.AdministratorRepository.GetDisplayNameAsync(content.AddUserName);
                        nameValueCacheDict[key] = value;
                    }
                }
            }
            else if (StringUtils.EqualsIgnoreCase(style.AttributeName, ContentAttribute.LastEditUserName))
            {
                if (!string.IsNullOrEmpty(content.LastEditUserName))
                {
                    var key = ContentAttribute.LastEditUserName + ":" + content.LastEditUserName;
                    if (!nameValueCacheDict.TryGetValue(key, out value))
                    {
                        value = await DataProvider.AdministratorRepository.GetDisplayNameAsync(content.LastEditUserName);
                        nameValueCacheDict[key] = value;
                    }
                }
            }
            else if (StringUtils.EqualsIgnoreCase(style.AttributeName, nameof(Content.CheckUserName)))
            {
                var checkUserName = content.CheckUserName;
                if (!string.IsNullOrEmpty(checkUserName))
                {
                    var key = nameof(Content.CheckUserName) + ":" + checkUserName;
                    if (!nameValueCacheDict.TryGetValue(key, out value))
                    {
                        value = await DataProvider.AdministratorRepository.GetDisplayNameAsync(checkUserName);
                        nameValueCacheDict[key] = value;
                    }
                }
            }
            else if (StringUtils.EqualsIgnoreCase(style.AttributeName, nameof(Content.CheckDate)))
            {
                var checkDate = content.CheckDate;
                if (checkDate != null)
                {
                    value = DateUtils.GetDateAndTimeString(checkDate);
                }
            }
            else if (StringUtils.EqualsIgnoreCase(style.AttributeName, nameof(Content.CheckReasons)))
            {
                value = content.CheckReasons;
            }
            else if (StringUtils.EqualsIgnoreCase(style.AttributeName, ContentAttribute.AddDate))
            {
                value = DateUtils.GetDateAndTimeString(content.AddDate);
            }
            else if (StringUtils.EqualsIgnoreCase(style.AttributeName, ContentAttribute.LastEditDate))
            {
                value = DateUtils.GetDateAndTimeString(content.LastEditDate);
            }
            else if (StringUtils.EqualsIgnoreCase(style.AttributeName, ContentAttribute.SourceId))
            {
                value = await SourceManager.GetSourceNameAsync(content.SourceId);
            }
            else if (StringUtils.EqualsIgnoreCase(style.AttributeName, ContentAttribute.Tags))
            {
                value = content.Tags;
            }
            else if (StringUtils.EqualsIgnoreCase(style.AttributeName, ContentAttribute.GroupNameCollection))
            {
                value = content.GroupNameCollection;
            }
            else if (StringUtils.EqualsIgnoreCase(style.AttributeName, ContentAttribute.Hits))
            {
                value = content.Hits.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(style.AttributeName, ContentAttribute.HitsByDay))
            {
                value = content.HitsByDay.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(style.AttributeName, ContentAttribute.HitsByWeek))
            {
                value = content.HitsByWeek.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(style.AttributeName, ContentAttribute.HitsByMonth))
            {
                value = content.HitsByMonth.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(style.AttributeName, ContentAttribute.LastHitsDate))
            {
                value = DateUtils.GetDateAndTimeString(content.LastHitsDate);
            }
            else if (StringUtils.EqualsIgnoreCase(style.AttributeName, ContentAttribute.Downloads))
            {
                value = content.Downloads.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(style.AttributeName, ContentAttribute.IsTop) || StringUtils.EqualsIgnoreCase(style.AttributeName, ContentAttribute.IsColor) || StringUtils.EqualsIgnoreCase(style.AttributeName, ContentAttribute.IsHot) || StringUtils.EqualsIgnoreCase(style.AttributeName, ContentAttribute.IsRecommend))
            {
                value = StringUtils.GetTrueImageHtml(content.Get<string>(style.AttributeName));
            }
            else
            {
                value = await InputParserUtility.GetContentByTableStyleAsync(content.Get<string>(style.AttributeName), site, style);
            }
            return value;
        }

        public static async Task<bool> IsEditAsync(Site site, int channelId, PermissionsImpl permissionsImpl)
        {
            return await permissionsImpl.HasChannelPermissionsAsync(site.Id, channelId, Constants.ChannelPermissions.ContentEdit);
        }

        //public static bool IsComment(Site site, int channelId, string administratorName)
        //{
        //    return site.IsCommentable && AdminUtility.HasChannelPermissions(administratorName, site.Id, channelId, SystemManager.Permissions.Channel.CommentCheck, SystemManager.Permissions.Channel.CommentDelete);
        //}

        public static string GetColumnsHeadHtml(List<TableStyle> tableStyleList, Dictionary<string, Dictionary<string, Func<IContentContext, string>>> pluginColumns, StringCollection attributesOfDisplay)
        {
            var builder = new StringBuilder();

            var styleList = ContentUtility.GetAllTableStyleList(tableStyleList);

            foreach (var style in styleList)
            {
                if (!attributesOfDisplay.Contains(style.AttributeName) || style.AttributeName == ContentAttribute.Title) continue;
                builder.Append($@"<th class=""text-nowrap"">{style.DisplayName}</th>");
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

        public static async Task<string> GetColumnsHtmlAsync(Dictionary<string, string> nameValueCacheDict, Site site, Content content, StringCollection attributesOfDisplay, List<TableStyle> displayStyleList, Dictionary<string, Dictionary<string, Func<IContentContext, string>>> pluginColumns)
        {
            var builder = new StringBuilder();

            foreach (var style in displayStyleList)
            {
                if (!attributesOfDisplay.Contains(style.AttributeName) || style.AttributeName == ContentAttribute.Title) continue;

                var value = await GetColumnValueAsync(nameValueCacheDict, site, content, style);
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
                                SiteId = content.SiteId,
                                ChannelId = content.ChannelId,
                                ContentId = content.Id
                            });
                            builder.Append($@"<td class=""text-nowrap"">{value}</td>");
                        }
                        catch (Exception ex)
                        {
                            LogUtils.AddErrorLogAsync(pluginId, ex).GetAwaiter().GetResult();
                        }
                    }
                }
            }

            return builder.ToString();
        }

        public static string GetCommandsHtml(Site site, List<Menu> pluginMenus, Content content, string pageUrl, string administratorName, bool isEdit)
        {
            var builder = new StringBuilder();

            if (isEdit || administratorName == content.AddUserName)
            {
                builder.Append($@"<a href=""{PageContentAdd.GetRedirectUrlOfEdit(site.Id, content.ChannelId, content.Id, pageUrl)}"">编辑</a>");
            }

            if (pluginMenus != null)
            {
                foreach (var menu in pluginMenus)
                {
                    builder.Append(string.IsNullOrEmpty(menu.Target)
                        ? $@"<a class=""m-l-5"" href=""javascript:;"" onclick=""{LayerUtils.GetOpenScript(menu.Text, menu.Link)}"">{menu.Text}</a>"
                        : $@"<a class=""m-l-5"" href=""{menu.Link}"" target=""{menu.Target}"">{menu.Text}</a>");
                }
            }

            return builder.ToString();
        }
    }
}
