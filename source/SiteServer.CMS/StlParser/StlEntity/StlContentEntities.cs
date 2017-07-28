using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlEntity
{
    [Stl(Usage = "内容实体", Description = "通过 {content.} 实体在模板中显示内容值")]
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
                    if (contextInfo.ContentInfo != null && contextInfo.ContentInfo.ReferenceId > 0 && contextInfo.ContentInfo.SourceId > 0 && contextInfo.ContentInfo.GetExtendedAttribute(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString())
                    {
                        var targetNodeId = contextInfo.ContentInfo.SourceId;
                        var targetPublishmentSystemId = DataProvider.NodeDao.GetPublishmentSystemId(targetNodeId);
                        var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
                        var targetNodeInfo = NodeManager.GetNodeInfo(targetPublishmentSystemId, targetNodeId);

                        var tableStyle = NodeManager.GetTableStyle(targetPublishmentSystemInfo, targetNodeInfo);
                        var tableName = NodeManager.GetTableName(targetPublishmentSystemInfo, targetNodeInfo);
                        var targetContentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contextInfo.ContentInfo.ReferenceId);
                        if (targetContentInfo != null && targetContentInfo.NodeId > 0)
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
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.Id);
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
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.Title);
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
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.Title);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(NavigationUrl, attributeName))//内容链接地址
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, contextInfo.ContentInfo);
                        }
                        else
                        {
                            var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId);
                            parsedContent = PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, nodeInfo, contextInfo.ContentId, false);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(ImageUrl, attributeName))//内容图片地址
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = contextInfo.ContentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl);
                        }
                        else
                        {
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, BackgroundContentAttribute.ImageUrl);
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            parsedContent = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, parsedContent);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(VideoUrl, attributeName))//内容视频地址
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = contextInfo.ContentInfo.GetExtendedAttribute(BackgroundContentAttribute.VideoUrl);
                        }
                        else
                        {
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, BackgroundContentAttribute.VideoUrl);
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            parsedContent = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, parsedContent);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(FileUrl, attributeName))//内容附件地址
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = contextInfo.ContentInfo.GetExtendedAttribute(BackgroundContentAttribute.FileUrl);
                        }
                        else
                        {
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, BackgroundContentAttribute.FileUrl);
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            parsedContent = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, parsedContent);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(DownloadUrl, attributeName))//内容附件地址(可统计下载量)
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = contextInfo.ContentInfo.GetExtendedAttribute(BackgroundContentAttribute.FileUrl);
                        }
                        else
                        {
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, BackgroundContentAttribute.FileUrl);
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            parsedContent = ActionsDownload.GetUrl(pageInfo.PublishmentSystemInfo.Additional.ApiUrl, pageInfo.PublishmentSystemId, contextInfo.ChannelId, contextInfo.ContentId, parsedContent);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(AddDate, attributeName))//内容添加日期
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = DateUtils.Format(contextInfo.ContentInfo.AddDate, string.Empty);
                        }
                        else
                        {
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, contextInfo.ChannelId);
                            parsedContent = DateUtils.Format(BaiRongDataProvider.ContentDao.GetAddDate(tableName, contextInfo.ContentId), string.Empty);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(LastEditDate, attributeName))//替换最后修改日期
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = DateUtils.Format(contextInfo.ContentInfo.LastEditDate, string.Empty);
                        }
                        else
                        {
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, contextInfo.ChannelId);
                            parsedContent = DateUtils.Format(BaiRongDataProvider.ContentDao.GetLastEditDate(tableName, contextInfo.ContentId), string.Empty);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(Content, attributeName))//内容正文
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = contextInfo.ContentInfo.GetExtendedAttribute(BackgroundContentAttribute.Content);
                        }
                        else
                        {
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, BackgroundContentAttribute.Content);
                        }
                        parsedContent = StringUtility.TextEditorContentDecode(parsedContent, pageInfo.PublishmentSystemInfo);
                    }
                    else if (StringUtils.EqualsIgnoreCase(Group, attributeName))//内容组别
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            parsedContent = contextInfo.ContentInfo.ContentGroupNameCollection;
                        }
                        else
                        {
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.ContentGroupNameCollection);
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
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.Tags);
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
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId));
                            addUserName = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.AddUserName);
                        }
                        if (!string.IsNullOrEmpty(addUserName))
                        {
                            var displayName = BaiRongDataProvider.AdministratorDao.GetDisplayName(addUserName);
                            parsedContent = string.IsNullOrEmpty(displayName) ? addUserName : displayName;
                        }
                    }
                    else if (StringUtils.StartsWithIgnoreCase(attributeName, StlParserUtility.ItemIndex) && contextInfo.ItemContainer?.ContentItem != null)
                    {
                        parsedContent = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.ContentItem.ItemIndex, attributeName, contextInfo).ToString();
                    }
                    else
                    {
                        int contentNodeId;
                        if (contextInfo.ContentInfo != null)
                        {
                            contentNodeId = contextInfo.ContentInfo.NodeId;
                            if (contextInfo.ContentInfo.ContainsKey(attributeName))
                            {
                                parsedContent = contextInfo.ContentInfo.GetExtendedAttribute(attributeName);
                            }
                        }
                        else
                        {
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, contextInfo.ChannelId);
                            contentNodeId = BaiRongDataProvider.ContentDao.GetNodeId(tableName, contextInfo.ContentId);
                            tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contentNodeId));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, attributeName);
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(pageInfo.PublishmentSystemId, contentNodeId);
                            var styleInfo = TableStyleManager.GetTableStyleInfo(ETableStyle.BackgroundContent, pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, attributeName, relatedIdentities);
                            parsedContent = InputParserUtility.GetContentByTableStyle(parsedContent, ",", pageInfo.PublishmentSystemInfo, ETableStyle.BackgroundContent, styleInfo, string.Empty, null, string.Empty, true);
                        }

                    }
                }
                catch
                {
                    // ignored
                }
            }
            return parsedContent;
        }
    }
}
