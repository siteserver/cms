using System.Collections.Specialized;
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
    public class StlContentEntities
    {
        private StlContentEntities()
        {
        }

        public const string EntityName = "Content";                  //内容实体

        public static string ContentID = "ContentID";//内容ID
        public static string Title = "Title";//内容标题
        public static string FullTitle = "FullTitle";//内容标题全称
        public static string NavigationUrl = "NavigationUrl";//内容链接地址
        public static string ImageUrl = "ImageUrl";//内容图片地址
        public static string VideoUrl = "VideoUrl";//内容视频地址
        public static string FileUrl = "FileUrl";//内容附件地址
        public static string DownloadUrl = "DownloadUrl";//内容附件地址(可统计下载量)
        public static string AddDate = "AddDate";//内容添加日期
        public static string LastEditDate = "LastEditDate";//内容最后修改日期
        public static string Content = "Content";//内容正文
        public static string Group = "Group";//内容组别
        public static string Tags = "Tags";//内容标签
        public static string AddUserName = "AddUserName";//内容添加人
        public static string ItemIndex = "ItemIndex";//内容排序

        public static ListDictionary AttributeList
        {
            get
            {
                var attributes = new ListDictionary();
                attributes.Add(ContentID, "内容ID");
                attributes.Add(Title, "内容标题");
                attributes.Add(FullTitle, "内容标题全称");
                attributes.Add(Content, "内容正文");
                attributes.Add(NavigationUrl, "内容链接地址");
                attributes.Add(ImageUrl, "内容图片地址");
                attributes.Add(VideoUrl, "内容视频地址");
                attributes.Add(FileUrl, "内容附件地址");
                attributes.Add(DownloadUrl, "内容附件地址(可统计下载量)");
                attributes.Add(AddDate, "内容添加日期");
                attributes.Add(LastEditDate, "内容最后修改日期");
                attributes.Add(Group, "内容组别");
                attributes.Add(Tags, "内容标签");
                attributes.Add(AddUserName, "内容添加人");
                attributes.Add(ItemIndex, "内容排序");
                return attributes;
            }
        }

        internal static string Parse(string stlEntity, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;

            if (contextInfo.ContentID != 0)
            {
                try
                {
                    if (contextInfo.ContentInfo != null && contextInfo.ContentInfo.ReferenceId > 0 && contextInfo.ContentInfo.SourceId > 0 && contextInfo.ContentInfo.GetExtendedAttribute(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString())
                    {
                        var targetNodeID = contextInfo.ContentInfo.SourceId;
                        var targetPublishmentSystemID = DataProvider.NodeDao.GetPublishmentSystemId(targetNodeID);
                        var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemID);
                        var targetNodeInfo = NodeManager.GetNodeInfo(targetPublishmentSystemID, targetNodeID);

                        var tableStyle = NodeManager.GetTableStyle(targetPublishmentSystemInfo, targetNodeInfo);
                        var tableName = NodeManager.GetTableName(targetPublishmentSystemInfo, targetNodeInfo);
                        var targetContentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contextInfo.ContentInfo.ReferenceId);
                        if (targetContentInfo != null || targetContentInfo.NodeId > 0)
                        {
                            //标题可以使用自己的
                            targetContentInfo.Title = contextInfo.ContentInfo.Title;

                            contextInfo.ContentInfo = targetContentInfo;
                        }
                    }



                    var entityName = StlParserUtility.GetNameFromEntity(stlEntity);
                    var attributeName = entityName.Substring(9, entityName.Length - 10);

                    if (StringUtils.EqualsIgnoreCase(ContentID, attributeName) || StringUtils.EqualsIgnoreCase(ContentAttribute.Id, attributeName))//内容ID
                    {
                        if (contextInfo.ContentInfo != null)
                        {
                            if (contextInfo.ContentInfo.ReferenceId > 0)
                            {
                                parsedContent = contextInfo.ContentInfo.ReferenceId.ToString();
                            }
                            else
                            {
                                parsedContent = contextInfo.ContentInfo.Id.ToString();
                            }
                        }
                        else
                        {
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelID));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentID, ContentAttribute.Id);
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
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelID));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentID, ContentAttribute.Title);
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
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelID));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentID, ContentAttribute.Title);
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
                            var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelID);
                            parsedContent = PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, nodeInfo, contextInfo.ContentID, false);
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
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelID));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentID, BackgroundContentAttribute.ImageUrl);
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
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelID));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentID, BackgroundContentAttribute.VideoUrl);
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
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelID));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentID, BackgroundContentAttribute.FileUrl);
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
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelID));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentID, BackgroundContentAttribute.FileUrl);
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            parsedContent = ActionsDownload.GetUrl(pageInfo.PublishmentSystemInfo.Additional.ApiUrl, pageInfo.PublishmentSystemId, contextInfo.ChannelID, contextInfo.ContentID);
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
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, contextInfo.ChannelID);
                            parsedContent = DateUtils.Format(BaiRongDataProvider.ContentDao.GetAddDate(tableName, contextInfo.ContentID), string.Empty);
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
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, contextInfo.ChannelID);
                            parsedContent = DateUtils.Format(BaiRongDataProvider.ContentDao.GetLastEditDate(tableName, contextInfo.ContentID), string.Empty);
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
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelID));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentID, BackgroundContentAttribute.Content);
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
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelID));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentID, ContentAttribute.ContentGroupNameCollection);
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
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelID));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentID, ContentAttribute.Tags);
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(AddUserName, attributeName))
                    {
                        var addUserName = string.Empty;
                        if (contextInfo.ContentInfo != null)
                        {
                            addUserName = contextInfo.ContentInfo.AddUserName;
                        }
                        else
                        {
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelID));
                            addUserName = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentID, ContentAttribute.AddUserName);
                        }
                        if (!string.IsNullOrEmpty(addUserName))
                        {
                            var displayName = BaiRongDataProvider.AdministratorDao.GetDisplayName(addUserName);
                            if (string.IsNullOrEmpty(displayName))
                            {
                                parsedContent = addUserName;
                            }
                            else
                            {
                                parsedContent = displayName;
                            }
                        }
                    }
                    else if (StringUtils.StartsWithIgnoreCase(attributeName, StlParserUtility.ItemIndex) && contextInfo.ItemContainer != null && contextInfo.ItemContainer.ContentItem != null)
                    {
                        parsedContent = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.ContentItem.ItemIndex, attributeName, contextInfo).ToString();
                    }
                    else
                    {
                        var contentNodeID = 0;
                        if (contextInfo.ContentInfo != null)
                        {
                            contentNodeID = contextInfo.ContentInfo.NodeId;
                            if (contextInfo.ContentInfo.ContainsKey(attributeName))
                            {
                                parsedContent = contextInfo.ContentInfo.GetExtendedAttribute(attributeName);
                            }
                        }
                        else
                        {
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, contextInfo.ChannelID);
                            contentNodeID = BaiRongDataProvider.ContentDao.GetNodeId(tableName, contextInfo.ContentID);
                            tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contentNodeID));
                            parsedContent = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentID, attributeName);
                        }

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(pageInfo.PublishmentSystemId, contentNodeID);
                            var styleInfo = TableStyleManager.GetTableStyleInfo(ETableStyle.BackgroundContent, pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, attributeName, relatedIdentities);
                            parsedContent = InputParserUtility.GetContentByTableStyle(parsedContent, ",", pageInfo.PublishmentSystemInfo, ETableStyle.BackgroundContent, styleInfo, string.Empty, null, string.Empty, true);
                        }

                    }
                }
                catch { }
            }
            return parsedContent;
        }
    }
}
