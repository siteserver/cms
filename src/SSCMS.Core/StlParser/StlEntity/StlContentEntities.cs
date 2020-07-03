using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.StlParser.Model;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlEntity
{
    [StlElement(Title = "内容实体", Description = "通过 {content.} 实体在模板中显示内容值")]
    public static class StlContentEntities
    {
        public const string EntityName = "content";

        public const string Id = nameof(Models.Content.Id);
        public const string Title = nameof(Models.Content.Title);
        public const string FullTitle = "FullTitle";
        public const string NavigationUrl = "NavigationUrl";
        public const string ImageUrl = nameof(Models.Content.ImageUrl);
        public const string VideoUrl = nameof(Models.Content.VideoUrl);
        public const string FileUrl = nameof(Models.Content.FileUrl);
        public const string DownloadUrl = "DownloadUrl";
        public const string AddDate = nameof(Models.Content.AddDate);
        public const string LastModifiedDate = nameof(Models.Content.LastModifiedDate);
        public const string Body = nameof(Models.Content.Body);
        public const string Group = "Group";
        public const string Tags = "Tags";
        public const string ItemIndex = "ItemIndex";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {Id, "内容ID"},
            {Title, "内容标题"},
            {FullTitle, "内容标题全称"},
            {Body, "内容正文"},
            {NavigationUrl, "内容链接地址"},
            {ImageUrl, "内容图片地址"},
            {VideoUrl, "内容视频地址"},
            {FileUrl, "内容附件地址"},
            {DownloadUrl, "内容附件地址(可统计下载量)"},
            {AddDate, "内容添加日期"},
            {LastModifiedDate, "内容最后修改日期"},
            {Group, "内容组别"},
            {Tags, "内容标签"},
            {ItemIndex, "内容排序"}
        };

        internal static async Task<string> ParseAsync(string stlEntity, IParseManager parseManager)
        {
            var databaseManager = parseManager.DatabaseManager;
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            var parsedContent = string.Empty;

            if (parseManager.ContextInfo.ContentId != 0)
            {
                try
                {
                    var contentInfo = await parseManager.GetContentAsync();

                    if (contentInfo != null && contentInfo.ReferenceId > 0 && contentInfo.SourceId > 0 && TranslateContentType.ReferenceContent.GetValue() == contentInfo.Get<string>(ColumnsManager.TranslateContentType))
                    {
                        var targetChannelId = contentInfo.SourceId;
                        var targetSiteId = await databaseManager.ChannelRepository.GetSiteIdAsync(targetChannelId);
                        var targetSite = await databaseManager.SiteRepository.GetAsync(targetSiteId);
                        var targetNodeInfo = await databaseManager.ChannelRepository.GetAsync(targetChannelId);

                        var targetContentInfo = await databaseManager.ContentRepository.GetAsync(targetSite, targetNodeInfo, contentInfo.ReferenceId);
                        if (targetContentInfo != null && targetContentInfo.ChannelId > 0)
                        {
                            //标题可以使用自己的
                            targetContentInfo.Title = contentInfo.Title;

                            contentInfo = targetContentInfo;
                        }
                    }

                    var entityName = StlParserUtility.GetNameFromEntity(stlEntity);
                    var attributeName = entityName.Substring(9, entityName.Length - 10);

                    if (StringUtils.EqualsIgnoreCase(nameof(Models.Content.Id), attributeName))//内容ID
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = contentInfo.ReferenceId > 0 ? contentInfo.ReferenceId.ToString() : contentInfo.Id.ToString();
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(Title, attributeName))//内容标题
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = contentInfo.Title;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(FullTitle, attributeName))//内容标题全称
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = contentInfo.Title;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(NavigationUrl, attributeName))//内容链接地址
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = await parseManager.PathManager.GetContentUrlAsync(pageInfo.Site, contentInfo, pageInfo.IsLocal);
                        }
                        else
                        {
                            var nodeInfo = await databaseManager.ChannelRepository.GetAsync(contextInfo.ChannelId);
                            parsedContent = await parseManager.PathManager.GetContentUrlAsync(pageInfo.Site, nodeInfo, contextInfo.ContentId, pageInfo.IsLocal);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(ImageUrl, attributeName))//内容图片地址
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = contentInfo.ImageUrl;
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            parsedContent = await parseManager.PathManager.ParseSiteUrlAsync(pageInfo.Site, parsedContent, pageInfo.IsLocal);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(VideoUrl, attributeName))//内容视频地址
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = contentInfo.VideoUrl;
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            parsedContent = await parseManager.PathManager.ParseSiteUrlAsync(pageInfo.Site, parsedContent, pageInfo.IsLocal);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(FileUrl, attributeName))//内容附件地址
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = contentInfo.FileUrl;
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            parsedContent = await parseManager.PathManager.ParseSiteUrlAsync(pageInfo.Site, parsedContent, pageInfo.IsLocal);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(DownloadUrl, attributeName))//内容附件地址(可统计下载量)
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = contentInfo.FileUrl;
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            parsedContent = parseManager.PathManager.GetDownloadApiUrl(pageInfo.SiteId, contextInfo.ChannelId, contextInfo.ContentId, parsedContent);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(AddDate, attributeName))//内容添加日期
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = DateUtils.Format(contentInfo.AddDate, string.Empty);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(LastModifiedDate, attributeName))//替换最后修改日期
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = DateUtils.Format(contentInfo.LastModifiedDate, string.Empty);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(Body, attributeName) || StringUtils.EqualsIgnoreCase("Content", attributeName))//内容正文
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = contentInfo.Body;
                        }

                        parsedContent = await parseManager.PathManager.DecodeTextEditorAsync(pageInfo.Site, parsedContent, pageInfo.IsLocal);
                    }
                    else if (StringUtils.EqualsIgnoreCase(Group, attributeName))//内容组别
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = ListUtils.ToString(contentInfo.GroupNames);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(Tags, attributeName))//标签
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = ListUtils.ToString(contentInfo.TagNames);
                        }
                    }
                    else if (StringUtils.StartsWithIgnoreCase(attributeName, StlParserUtility.ItemIndex) && contextInfo.ItemContainer?.ContentItem != null)
                    {
                        parsedContent = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.ContentItem.Key, attributeName, contextInfo).ToString();
                    }
                    else
                    {
                        var contentChannelId = 0;

                        if (contentInfo == null)
                        {
                            contentInfo = await databaseManager.ContentRepository.GetAsync(pageInfo.Site, contextInfo.ChannelId, contextInfo.ContentId);
                        }

                        if (contentInfo != null)
                        {
                            contentChannelId = contentInfo.ChannelId;
                            if (contentInfo.ContainsKey(attributeName))
                            {
                                parsedContent = contentInfo.Get<string>(attributeName);
                            }
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            var channel = await databaseManager.ChannelRepository.GetAsync(contentChannelId);
                            var tableName = databaseManager.ChannelRepository.GetTableName(pageInfo.Site, channel);
                            var relatedIdentities = databaseManager.TableStyleRepository.GetRelatedIdentities(channel);
                            var styleInfo = await databaseManager.TableStyleRepository.GetTableStyleAsync(tableName, attributeName, relatedIdentities);

                            //styleInfo.IsVisible = false 表示此字段不需要显示 styleInfo.TableStyleId = 0 不能排除，因为有可能是直接辅助表字段没有添加显示样式
                            var inputParser = new InputParserManager(parseManager.PathManager);

                            parsedContent = await inputParser.GetContentByTableStyleAsync(parsedContent, ",", pageInfo.Site, styleInfo, string.Empty, null, string.Empty, true);
                        }

                    }
                }
                catch
                {
                    // ignored
                }
            }

            return parsedContent?.Replace(Constants.PagePlaceHolder, string.Empty);
        }
    }
}
