using System.Collections.Generic;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.DataCache.Stl;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlEntity
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

        internal static string Parse(string stlEntity, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;

            if (contextInfo.ContentId != 0)
            {
                try
                {
                    if (contextInfo.ContentInfo != null && contextInfo.ContentInfo.ReferenceId > 0 && contextInfo.ContentInfo.SourceId > 0 && contextInfo.ContentInfo.GetString(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString())
                    {
                        var targetChannelId = contextInfo.ContentInfo.SourceId;
                        var targetSiteId = StlChannelCache.GetSiteId(targetChannelId);
                        var targetSiteInfo = SiteManager.GetSiteInfo(targetSiteId);
                        var targetNodeInfo = ChannelManager.GetChannelInfo(targetSiteId, targetChannelId);

                        var targetContentInfo = ContentManager.GetContentInfo(targetSiteInfo, targetNodeInfo, contextInfo.ContentInfo.ReferenceId);
                        if (targetContentInfo != null && targetContentInfo.ChannelId > 0)
                        {
                            //标题可以使用自己的
                            targetContentInfo.Title = contextInfo.ContentInfo.Title;

                            contextInfo.ContentInfo = targetContentInfo;
                        }
                    }

                    var entityName = StlParserUtility.GetNameFromEntity(stlEntity);
                    var attributeName = entityName.Substring(9, entityName.Length - 10);

                    if (StringUtils.EqualsIgnoreCase(ContentAttribute.Id, attributeName))//内容ID
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = contextInfo.ContentInfo.ReferenceId > 0 ? contextInfo.ContentInfo.ReferenceId.ToString() : contextInfo.ContentInfo.Id.ToString();
                        }
                        else
                        {
                            var tableName = ChannelManager.GetTableName(pageInfo.SiteInfo, ChannelManager.GetChannelInfo(pageInfo.SiteId, contextInfo.ChannelId));
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.Id);
                            parsedContent = StlContentCache.GetValue(tableName, contextInfo.ContentId, ContentAttribute.Id);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(Title, attributeName))//内容标题
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = contextInfo.ContentInfo.Title;
                        }
                        else
                        {
                            var tableName = ChannelManager.GetTableName(pageInfo.SiteInfo, ChannelManager.GetChannelInfo(pageInfo.SiteId, contextInfo.ChannelId));
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.Title);
                            parsedContent = StlContentCache.GetValue(tableName, contextInfo.ContentId, ContentAttribute.Title);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(FullTitle, attributeName))//内容标题全称
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = contextInfo.ContentInfo.Title;
                        }
                        else
                        {
                            var tableName = ChannelManager.GetTableName(pageInfo.SiteInfo, ChannelManager.GetChannelInfo(pageInfo.SiteId, contextInfo.ChannelId));
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.Title);
                            parsedContent = StlContentCache.GetValue(tableName, contextInfo.ContentId, ContentAttribute.Title);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(NavigationUrl, attributeName))//内容链接地址
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = PageUtility.GetContentUrl(pageInfo.SiteInfo, contextInfo.ContentInfo, pageInfo.IsLocal);
                        }
                        else
                        {
                            var nodeInfo = ChannelManager.GetChannelInfo(pageInfo.SiteId, contextInfo.ChannelId);
                            parsedContent = PageUtility.GetContentUrl(pageInfo.SiteInfo, nodeInfo, contextInfo.ContentId, pageInfo.IsLocal);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(ImageUrl, attributeName))//内容图片地址
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = contextInfo.ContentInfo.GetString(BackgroundContentAttribute.ImageUrl);
                        }
                        else
                        {
                            var tableName = ChannelManager.GetTableName(pageInfo.SiteInfo, ChannelManager.GetChannelInfo(pageInfo.SiteId, contextInfo.ChannelId));
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, BackgroundContentAttribute.ImageUrl);
                            parsedContent = StlContentCache.GetValue(tableName, contextInfo.ContentId, BackgroundContentAttribute.ImageUrl);
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            parsedContent = PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, parsedContent, pageInfo.IsLocal);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(VideoUrl, attributeName))//内容视频地址
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = contextInfo.ContentInfo.GetString(BackgroundContentAttribute.VideoUrl);
                        }
                        else
                        {
                            var tableName = ChannelManager.GetTableName(pageInfo.SiteInfo, ChannelManager.GetChannelInfo(pageInfo.SiteId, contextInfo.ChannelId));
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, BackgroundContentAttribute.VideoUrl);
                            parsedContent = StlContentCache.GetValue(tableName, contextInfo.ContentId, BackgroundContentAttribute.VideoUrl);
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            parsedContent = PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, parsedContent, pageInfo.IsLocal);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(FileUrl, attributeName))//内容附件地址
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = contextInfo.ContentInfo.GetString(BackgroundContentAttribute.FileUrl);
                        }
                        else
                        {
                            var tableName = ChannelManager.GetTableName(pageInfo.SiteInfo, ChannelManager.GetChannelInfo(pageInfo.SiteId, contextInfo.ChannelId));
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, BackgroundContentAttribute.FileUrl);
                            parsedContent = StlContentCache.GetValue(tableName, contextInfo.ContentId, BackgroundContentAttribute.FileUrl);
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            parsedContent = PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, parsedContent, pageInfo.IsLocal);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(DownloadUrl, attributeName))//内容附件地址(可统计下载量)
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = contextInfo.ContentInfo.GetString(BackgroundContentAttribute.FileUrl);
                        }
                        else
                        {
                            var tableName = ChannelManager.GetTableName(pageInfo.SiteInfo, ChannelManager.GetChannelInfo(pageInfo.SiteId, contextInfo.ChannelId));
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, BackgroundContentAttribute.FileUrl);
                            parsedContent = StlContentCache.GetValue(tableName, contextInfo.ContentId, BackgroundContentAttribute.FileUrl);
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            parsedContent = ApiRouteActionsDownload.GetUrl(pageInfo.ApiUrl, pageInfo.SiteId, contextInfo.ChannelId, contextInfo.ContentId, parsedContent);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(AddDate, attributeName))//内容添加日期
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = DateUtils.Format(contextInfo.ContentInfo.AddDate, string.Empty);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(LastEditDate, attributeName))//替换最后修改日期
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = DateUtils.Format(contextInfo.ContentInfo.LastEditDate, string.Empty);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(Content, attributeName))//内容正文
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = contextInfo.ContentInfo.GetString(BackgroundContentAttribute.Content);
                        }
                        else
                        {
                            var tableName = ChannelManager.GetTableName(pageInfo.SiteInfo, ChannelManager.GetChannelInfo(pageInfo.SiteId, contextInfo.ChannelId));
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, BackgroundContentAttribute.Content);
                            parsedContent = StlContentCache.GetValue(tableName, contextInfo.ContentId, BackgroundContentAttribute.Content);
                        }
                        parsedContent = ContentUtility.TextEditorContentDecode(pageInfo.SiteInfo, parsedContent, pageInfo.IsLocal);
                    }
                    else if (StringUtils.EqualsIgnoreCase(Group, attributeName))//内容组别
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = contextInfo.ContentInfo.GroupNameCollection;
                        }
                        else
                        {
                            var tableName = ChannelManager.GetTableName(pageInfo.SiteInfo, ChannelManager.GetChannelInfo(pageInfo.SiteId, contextInfo.ChannelId));
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.ContentGroupNameCollection);
                            parsedContent = StlContentCache.GetValue(tableName, contextInfo.ContentId, ContentAttribute.GroupNameCollection);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(Tags, attributeName))//标签
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = contextInfo.ContentInfo.Tags;
                        }
                        else
                        {
                            var tableName = ChannelManager.GetTableName(pageInfo.SiteInfo, ChannelManager.GetChannelInfo(pageInfo.SiteId, contextInfo.ChannelId));
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.Tags);
                            parsedContent = StlContentCache.GetValue(tableName, contextInfo.ContentId, ContentAttribute.Tags);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(AddUserName, attributeName))
                    {
                        string addUserName;
                        if (contextInfo.ContentInfo != null)
                        {
                            addUserName = contextInfo.ContentInfo.AddUserName;
                        }
                        else
                        {
                            var tableName = ChannelManager.GetTableName(pageInfo.SiteInfo, ChannelManager.GetChannelInfo(pageInfo.SiteId, contextInfo.ChannelId));
                            //addUserName = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.AddUserName);
                            addUserName = StlContentCache.GetValue(tableName, contextInfo.ContentId, ContentAttribute.AddUserName);
                        }
                        if (!string.IsNullOrEmpty(addUserName))
                        {
                            parsedContent = addUserName;
                        }
                    }
                    else if (StringUtils.StartsWithIgnoreCase(attributeName, StlParserUtility.ItemIndex) && contextInfo.ItemContainer?.ContentItem != null)
                    {
                        parsedContent = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.ContentItem.ItemIndex, attributeName, contextInfo).ToString();
                    }
                    else
                    {
                        int contentChannelId;
                        if (contextInfo.ContentInfo != null)
                        {
                            contentChannelId = contextInfo.ContentInfo.ChannelId;
                            if (contextInfo.ContentInfo.ContainsKey(attributeName))
                            {
                                parsedContent = contextInfo.ContentInfo.GetString(attributeName);
                            }
                        }
                        else
                        {
                            var tableName = ChannelManager.GetTableName(pageInfo.SiteInfo, contextInfo.ChannelId);
                            //contentChannelId = DataProvider.ContentDao.GetChannelId(tableName, contextInfo.ContentId);
                            contentChannelId = StlContentCache.GetChannelId(tableName, contextInfo.ContentId);
                            tableName = ChannelManager.GetTableName(pageInfo.SiteInfo, ChannelManager.GetChannelInfo(pageInfo.SiteId, contentChannelId));
                            //parsedContent = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, attributeName);
                            parsedContent = StlContentCache.GetValue(tableName, contextInfo.ContentId, attributeName);
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            var channelInfo = ChannelManager.GetChannelInfo(pageInfo.SiteId, contentChannelId);
                            var tableName = ChannelManager.GetTableName(pageInfo.SiteInfo, channelInfo);
                            var relatedIdentities = TableStyleManager.GetRelatedIdentities(channelInfo);
                            var styleInfo = TableStyleManager.GetTableStyleInfo(tableName, attributeName, relatedIdentities);

                            //styleInfo.IsVisible = false 表示此字段不需要显示 styleInfo.TableStyleId = 0 不能排除，因为有可能是直接辅助表字段没有添加显示样式
                            parsedContent = InputParserUtility.GetContentByTableStyle(parsedContent, ",", pageInfo.SiteInfo, styleInfo, string.Empty, null, string.Empty, true);
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
