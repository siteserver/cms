using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Core.Api.Sys.Stl;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Enums;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.StlEntity
{
    [StlElement(Title = "内容实体", Description = "通过 {content.} 实体在模板中显示内容值")]
    public class StlContentEntities
    {
        private StlContentEntities()
        {
        }

        public const string EntityName = "content";

        public const string Id = "Id";
        public const string Title = "Title";
        public const string FullTitle = "FullTitle";
        public const string NavigationUrl = "NavigationUrl";
        public const string ImageUrl = "ImageUrl";
        public const string VideoUrl = "VideoUrl";
        public const string FileUrl = "FileUrl";
        public const string DownloadUrl = "DownloadUrl";
        public const string AddDate = "AddDate";
        public const string LastEditDate = "LastEditDate";
        public const string Content = "Content";
        public const string Group = "Group";
        public const string Tags = "Tags";
        public const string AddUserName = "AddUserName";
        public const string ItemIndex = "ItemIndex";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {Id, "内容ID"},
            {Title, "内容标题"},
            {FullTitle, "内容标题全称"},
            {Content, "内容正文"},
            {NavigationUrl, "内容链接地址"},
            {ImageUrl, "内容图片地址"},
            {VideoUrl, "内容视频地址"},
            {FileUrl, "内容附件地址"},
            {DownloadUrl, "内容附件地址(可统计下载量)"},
            {AddDate, "内容添加日期"},
            {LastEditDate, "内容最后修改日期"},
            {Group, "内容组别"},
            {Tags, "内容标签"},
            {AddUserName, "内容添加人"},
            {ItemIndex, "内容排序"}
        };

        internal static async Task<string> ParseAsync(string stlEntity, ParseContext parseContext)
        {
            var parsedContent = string.Empty;

            var channelInfo = await parseContext.GetChannelInfoAsync();
            var contentInfo = await parseContext.GetContentInfoAsync();

            if (parseContext.ContentId != 0)
            {
                try
                {
                    if (contentInfo != null && contentInfo.ReferenceId > 0 && contentInfo.SourceId > 0 && contentInfo.TranslateContentType != TranslateContentType.ReferenceContent.ToString())
                    {
                        var targetChannelId = contentInfo.SourceId;
                        var targetSiteId = parseContext.ChannelRepository.StlGetSiteId(targetChannelId);
                        var targetSiteInfo = parseContext.SiteRepository.GetSiteInfo(targetSiteId);
                        var targetNodeInfo = await parseContext.ChannelRepository.GetChannelInfoAsync(targetSiteId, targetChannelId);

                        var targetContentInfo = targetNodeInfo.ContentRepository.GetContentInfo(targetSiteInfo, targetNodeInfo, contentInfo.ReferenceId);
                        if (targetContentInfo != null && targetContentInfo.ChannelId > 0)
                        {
                            //标题可以使用自己的
                            targetContentInfo.Title = contentInfo.Title;

                            contentInfo = targetContentInfo;
                        }
                    }

                    var entityName = StlParserUtility.GetNameFromEntity(stlEntity);
                    var attributeName = entityName.Substring(9, entityName.Length - 10);

                    if (StringUtils.EqualsIgnoreCase(ContentAttribute.Id, attributeName))//内容ID
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = contentInfo.ReferenceId > 0 ? contentInfo.ReferenceId.ToString() : contentInfo.Id.ToString();
                        }
                        else
                        {
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.Id);
                            parsedContent = channelInfo.ContentRepository.StlGetValue(channelInfo, parseContext.ContentId, ContentAttribute.Id);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(Title, attributeName))//内容标题
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = contentInfo.Title;
                        }
                        else
                        {
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.Title);
                            parsedContent = channelInfo.ContentRepository.StlGetValue(channelInfo, parseContext.ContentId, ContentAttribute.Title);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(FullTitle, attributeName))//内容标题全称
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = contentInfo.Title;
                        }
                        else
                        {
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.Title);
                            parsedContent = channelInfo.ContentRepository.StlGetValue(channelInfo, parseContext.ContentId, ContentAttribute.Title);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(NavigationUrl, attributeName))//内容链接地址
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = await parseContext.UrlManager.GetContentUrlAsync(parseContext.SiteInfo, contentInfo, parseContext.IsLocal);
                        }
                        else
                        {
                            var nodeInfo = await parseContext.ChannelRepository.GetChannelInfoAsync(parseContext.SiteId, parseContext.ChannelId);
                            parsedContent = await parseContext.UrlManager.GetContentUrlAsync(parseContext.SiteInfo, nodeInfo, parseContext.ContentId, parseContext.IsLocal);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(ImageUrl, attributeName))//内容图片地址
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = contentInfo.ImageUrl;
                        }
                        else
                        {
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.ImageUrl);
                            parsedContent = channelInfo.ContentRepository.StlGetValue(channelInfo, parseContext.ContentId, ContentAttribute.ImageUrl);
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            parsedContent = parseContext.UrlManager.ParseNavigationUrl(parseContext.SiteInfo, parsedContent, parseContext.IsLocal);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(VideoUrl, attributeName))//内容视频地址
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = contentInfo.VideoUrl;
                        }
                        else
                        {
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.VideoUrl);
                            parsedContent = channelInfo.ContentRepository.StlGetValue(channelInfo, parseContext.ContentId, ContentAttribute.VideoUrl);
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            parsedContent = parseContext.UrlManager.ParseNavigationUrl(parseContext.SiteInfo, parsedContent, parseContext.IsLocal);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(FileUrl, attributeName))//内容附件地址
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = contentInfo.FileUrl;
                        }
                        else
                        {
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.FileUrl);
                            parsedContent = channelInfo.ContentRepository.StlGetValue(channelInfo, parseContext.ContentId, ContentAttribute.FileUrl);
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            parsedContent = parseContext.UrlManager.ParseNavigationUrl(parseContext.SiteInfo, parsedContent, parseContext.IsLocal);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(DownloadUrl, attributeName))//内容附件地址(可统计下载量)
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = contentInfo.FileUrl;
                        }
                        else
                        {
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.FileUrl);
                            parsedContent = channelInfo.ContentRepository.StlGetValue(channelInfo, parseContext.ContentId, ContentAttribute.FileUrl);
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            parsedContent = ApiRouteActionsDownload.GetUrl(parseContext.SettingsManager, parseContext.SiteId, parseContext.ChannelId, parseContext.ContentId, parsedContent);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(AddDate, attributeName))//内容添加日期
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = DateUtils.Format(contentInfo.AddDate, string.Empty);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(LastEditDate, attributeName))//替换最后修改日期
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = DateUtils.Format(contentInfo.LastModifiedDate, string.Empty);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(Content, attributeName))//内容正文
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = contentInfo.Content;
                        }
                        else
                        {
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.Content);
                            parsedContent = channelInfo.ContentRepository.StlGetValue(channelInfo, parseContext.ContentId, ContentAttribute.Content);
                        }
                        parsedContent = parseContext.FileManager.TextEditorContentDecode(parseContext.SiteInfo, parsedContent, parseContext.IsLocal);
                    }
                    else if (StringUtils.EqualsIgnoreCase(Group, attributeName))//内容组别
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = contentInfo.GroupNameCollection;
                        }
                        else
                        {
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.ContentGroupNameCollection);
                            parsedContent = channelInfo.ContentRepository.StlGetValue(channelInfo, parseContext.ContentId, ContentAttribute.GroupNameCollection);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(Tags, attributeName))//标签
                    {
                        if (contentInfo != null)
                        {
                            parsedContent = contentInfo.Tags;
                        }
                        else
                        {
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.Tags);
                            parsedContent = channelInfo.ContentRepository.StlGetValue(channelInfo, parseContext.ContentId, ContentAttribute.Tags);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(AddUserName, attributeName))
                    {
                        string addUserName;
                        if (contentInfo != null)
                        {
                            addUserName = contentInfo.AddUserName;
                        }
                        else
                        {
                            //addUserName = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.AddUserName);
                            addUserName = channelInfo.ContentRepository.StlGetValue(channelInfo, parseContext.ContentId, ContentAttribute.AddUserName);
                        }
                        if (!string.IsNullOrEmpty(addUserName))
                        {
                            parsedContent = addUserName;
                        }
                    }
                    else if (StringUtils.StartsWithIgnoreCase(attributeName, StlParserUtility.ItemIndex) && parseContext.Container?.ContentItem != null)
                    {
                        parsedContent = StlParserUtility.ParseItemIndex(parseContext.Container.ContentItem.Key, attributeName, parseContext).ToString();
                    }
                    else
                    {
                        int contentChannelId;
                        if (contentInfo != null)
                        {
                            contentChannelId = contentInfo.ChannelId;
                            if (contentInfo.ContainsKey(attributeName))
                            {
                                parsedContent = contentInfo.Get<string>(attributeName);
                            }
                        }
                        else
                        {
                            contentChannelId = channelInfo.ContentRepository.StlGetChannelId(channelInfo, parseContext.ContentId);

                            channelInfo = await parseContext.ChannelRepository.GetChannelInfoAsync(parseContext.SiteId, contentChannelId);
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, attributeName);
                            parsedContent = channelInfo.ContentRepository.StlGetValue(channelInfo, parseContext.ContentId, attributeName);
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            channelInfo = await parseContext.ChannelRepository.GetChannelInfoAsync(parseContext.SiteId, contentChannelId);
                            var tableName = await parseContext.ChannelRepository.GetTableNameAsync(parseContext.PluginManager, parseContext.SiteInfo, channelInfo);
                            var relatedIdentities = parseContext.TableManager.GetRelatedIdentities(channelInfo);
                            var styleInfo = parseContext.TableManager.GetTableStyleInfo(tableName, attributeName, relatedIdentities);

                            //styleInfo.IsVisible = false 表示此字段不需要显示 styleInfo.TableStyleId = 0 不能排除，因为有可能是直接辅助表字段没有添加显示样式
                            parsedContent = InputParserUtility.GetContentByTableStyle(parseContext.FileManager, parseContext.UrlManager, parseContext.SettingsManager, parsedContent, ",", parseContext.SiteInfo, styleInfo, string.Empty, null, string.Empty, true);
                        }

                    }
                }
                catch
                {
                    // ignored
                }
            }

            parsedContent = parsedContent.Replace(ContentUtility.PagePlaceHolder, string.Empty);

            return parsedContent;
        }
    }
}
