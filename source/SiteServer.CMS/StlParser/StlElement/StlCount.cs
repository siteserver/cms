using System;
using System.Collections.Generic;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "显示数值", Description = "通过 stl:count 标签在模板中显示统计数字")]
    public class StlCount
	{
        private StlCount() { }
		public const string ElementName = "stl:count";

		public const string AttributeType = "type";
        public const string AttributeChannelIndex = "channelIndex";
        public const string AttributeChannelName = "channelName";
        public const string AttributeUpLevel = "upLevel";
        public const string AttributeTopLevel = "topLevel";
        public const string AttributeScope = "scope";
        public const string AttributeSince = "since";
        public const string AttributeIsDynamic = "isDynamic";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeType, StringUtils.SortedListToAttributeValueString("需要获取值的类型", TypeList)},
            {AttributeChannelIndex, "栏目索引"},
            {AttributeChannelName, "栏目名称"},
            {AttributeUpLevel, "上级栏目的级别"},
            {AttributeTopLevel, "从首页向下的栏目级别"},
            {AttributeScope, "内容范围"},
            {AttributeSince, "时间段"},
            {AttributeIsDynamic, "是否动态显示"}
        };

        public const string TypeChannels = "Channels";
        public const string TypeContents = "Contents";
        public const string TypeComments = "Comments";
        public const string TypeDownloads = "Downloads";

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {TypeChannels, "栏目数"},
            {TypeContents, "内容数"},
            {TypeComments, "评论数"},
            {TypeDownloads, "下载次数"}
        };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			string parsedContent;
			try
			{
                var type = string.Empty;
                var channelIndex = string.Empty;
                var channelName = string.Empty;
                var upLevel = 0;
                var topLevel = -1;
                var scope = EScopeType.Self;
                var since = string.Empty;
                var isDynamic = false;

                var ie = node.Attributes?.GetEnumerator();
			    if (ie != null)
			    {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeType))
                        {
                            type = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeChannelIndex))
                        {
                            channelIndex = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeChannelName))
                        {
                            channelName = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeUpLevel))
                        {
                            upLevel = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTopLevel))
                        {
                            topLevel = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeScope))
                        {
                            scope = EScopeTypeUtils.GetEnumType(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeSince))
                        {
                            since = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value, false);
                        }
                    }
                }

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(pageInfo, contextInfo, type, channelIndex, channelName, upLevel, topLevel, scope, since);
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string type, string channelIndex, string channelName, int upLevel, int topLevel, EScopeType scope, string since)
        {
            var count = 0;

            var sinceDate = DateUtils.SqlMinValue;
            if (!string.IsNullOrEmpty(since))
            {
                sinceDate = DateTime.Now.AddHours(-DateUtils.GetSinceHours(since));
            }

            if (string.IsNullOrEmpty(type) || StringUtils.EqualsIgnoreCase(type, TypeContents))
            {
                var channelId = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelId, upLevel, topLevel);
                channelId = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelId, channelIndex, channelName);

                var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, channelId);

                var nodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeInfo, scope, string.Empty, string.Empty);
                foreach (var nodeId in nodeIdList)
                {
                    var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, nodeId);
                    count += DataProvider.ContentDao.GetCountOfContentAdd(tableName, pageInfo.PublishmentSystemId, nodeId, sinceDate, DateTime.Now.AddDays(1), string.Empty);
                }
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeChannels))
            {
                var channelId = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelId, upLevel, topLevel);
                channelId = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelId, channelIndex, channelName);

                var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, channelId);
                count = nodeInfo.ChildrenCount;
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeComments))
            {
                count = DataProvider.CommentDao.GetCountChecked(pageInfo.PublishmentSystemId, contextInfo.ChannelId, contextInfo.ContentId);
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeDownloads))
            {
                if (contextInfo.ContentId > 0)
                {
                    count = CountManager.GetCount(pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contextInfo.ContentId.ToString(), ECountType.Download);
                }
            }

            return count.ToString();
        }
	}
}
