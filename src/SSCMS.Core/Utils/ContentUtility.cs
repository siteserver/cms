using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Datory;
using Datory.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    public static class ContentUtility
    {
        public static string GetTitleFormatString(bool isStrong, bool isEm, bool isU, string color)
        {
            return $"{isStrong}_{isEm}_{isU}_{color}";
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

        public static string GetAutoPageBody(string content, int pageWordNum)
        {
            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(content))
            {
                content = content.Replace(Constants.PagePlaceHolder, string.Empty);
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
                        builder.Append(Constants.PagePlaceHolder);
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

        public static async Task TranslateAsync(IPathManager pathManager, IDatabaseManager databaseManager, IOldPluginManager pluginManager, Site site, int channelId, int contentId, int targetSiteId, int targetChannelId, TranslateContentType translateType, ICreateManager createManager)
        {
            if (site == null || channelId <= 0 || contentId <= 0 || targetSiteId <= 0 || targetChannelId <= 0) return;

            var targetSite = await databaseManager.SiteRepository.GetAsync(targetSiteId);
            var targetChannelInfo = await databaseManager.ChannelRepository.GetAsync(targetChannelId);

            var channel = await databaseManager.ChannelRepository.GetAsync(channelId);

            var contentInfo = await databaseManager.ContentRepository.GetAsync(site, channel, contentId);

            if (contentInfo == null) return;

            if (translateType == TranslateContentType.Copy)
            {
                await pathManager.MoveFileByContentAsync(site, targetSite, contentInfo);

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.Set(ColumnsManager.TranslateContentType, TranslateContentType.Copy.GetValue());
                var theContentId = await databaseManager.ContentRepository.InsertAsync(targetSite, targetChannelInfo, contentInfo);

                foreach (var plugin in pluginManager.GetPlugins())
                {
                    try
                    {
                        plugin.OnContentTranslateCompleted(new ContentTranslateEventArgs(site.Id, channel.Id, contentId, targetSiteId, targetChannelId, theContentId));
                    }
                    catch (Exception ex)
                    {
                        await databaseManager.ErrorLogRepository.AddErrorLogAsync(plugin.PluginId, ex, nameof(plugin.OnContentTranslateCompleted));
                    }
                }

                await createManager.CreateContentAsync(targetSite.Id, contentInfo.ChannelId, theContentId);
                await createManager.TriggerContentChangedEventAsync(targetSite.Id, contentInfo.ChannelId);
            }
            else if (translateType == TranslateContentType.Cut)
            {
                await pathManager.MoveFileByContentAsync(site, targetSite, contentInfo);

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.Set(ColumnsManager.TranslateContentType, TranslateContentType.Cut.GetValue());

                var newContentId = await databaseManager.ContentRepository.InsertAsync(targetSite, targetChannelInfo, contentInfo);

                foreach (var plugin in pluginManager.GetPlugins())
                {
                    try
                    {
                        plugin.OnContentTranslateCompleted(new ContentTranslateEventArgs(site.Id, channel.Id, contentId, targetSiteId, targetChannelId, newContentId));
                    }
                    catch (Exception ex)
                    {
                        await databaseManager.ErrorLogRepository.AddErrorLogAsync(plugin.PluginId, ex, nameof(plugin.OnContentTranslateCompleted));
                    }
                }

                await databaseManager.ContentRepository.DeleteAsync(pluginManager, site, channel, contentId);

                //GlobalSettings.ContentRepository.DeleteContents(site.Id, tableName, TranslateUtils.ToIntList(contentId), channelId);

                await createManager.CreateContentAsync(targetSite.Id, contentInfo.ChannelId, newContentId);
                await createManager.TriggerContentChangedEventAsync(targetSite.Id, contentInfo.ChannelId);
            }
            else if (translateType == TranslateContentType.Reference)
            {
                if (contentInfo.ReferenceId != 0) return;

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.Set(ColumnsManager.TranslateContentType, TranslateContentType.Reference.GetValue());
                //content.Attributes.Add(ContentAttribute.TranslateContentType, TranslateContentType.Reference.ToString());
                int theContentId = await databaseManager.ContentRepository.InsertAsync(targetSite, targetChannelInfo, contentInfo);

                await createManager.CreateContentAsync(targetSite.Id, contentInfo.ChannelId, theContentId);
                await createManager.TriggerContentChangedEventAsync(targetSite.Id, contentInfo.ChannelId);
            }
            else if (translateType == TranslateContentType.ReferenceContent)
            {
                if (contentInfo.ReferenceId != 0) return;

                await pathManager.MoveFileByContentAsync(site, targetSite, contentInfo);

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.Set(ColumnsManager.TranslateContentType, TranslateContentType.ReferenceContent.GetValue());
                var theContentId = await databaseManager.ContentRepository.InsertAsync(targetSite, targetChannelInfo, contentInfo);

                foreach (var plugin in pluginManager.GetPlugins())
                {
                    try
                    {
                        plugin.OnContentTranslateCompleted(new ContentTranslateEventArgs(site.Id, channel.Id, contentId, targetSiteId, targetChannelId, theContentId));
                    }
                    catch (Exception ex)
                    {
                        await databaseManager.ErrorLogRepository.AddErrorLogAsync(plugin.PluginId, ex, nameof(plugin.OnContentTranslateCompleted));
                    }
                }

                await createManager.CreateContentAsync(targetSite.Id, contentInfo.ChannelId, theContentId);
                await createManager.TriggerContentChangedEventAsync(targetSite.Id, contentInfo.ChannelId);
            }
        }

        public static bool IsCreatable(Channel channel, Content content)
        {
            if (channel == null || content == null) return false;

            //引用链接，不需要生成内容页；引用内容，需要生成内容页；
            if (content.ReferenceId > 0 &&
                content.Get<string>(ColumnsManager.TranslateContentType) != TranslateContentType.ReferenceContent.GetValue())
            {
                return false;
            }

            return string.IsNullOrEmpty(content.LinkUrl) && content.Checked && content.SourceId != SourceManager.Preview && content.ChannelId > 0;
        }

        private static ContentSummary ParseSummary(string channelContentId)
        {
            var arr = channelContentId.Split('_');
            if (arr.Length == 2)
            {
                return new ContentSummary
                {
                    ChannelId = TranslateUtils.ToIntWithNegative(arr[0]),
                    Id = TranslateUtils.ToInt(arr[1])
                };
            }
            return null;
        }

        public static List<ContentSummary> ParseSummaries(string summaries)
        {
            var channelContentIds = new List<ContentSummary>();
            if (string.IsNullOrEmpty(summaries)) return channelContentIds;

            foreach (var channelContentId in Utilities.GetStringList(summaries))
            {
                var summary = ParseSummary(channelContentId);
                if (summary != null)
                {
                    channelContentIds.Add(summary);
                }
            }

            return channelContentIds;
        }
    }
}
