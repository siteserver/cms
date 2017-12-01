using System;
using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Cache;
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

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeType, StringUtils.SortedListToAttributeValueString("需要获取值的类型", TypeList)},
            {AttributeChannelIndex, "栏目索引"},
            {AttributeChannelName, "栏目名称"},
            {AttributeUpLevel, "上级栏目的级别"},
            {AttributeTopLevel, "从首页向下的栏目级别"},
            {AttributeScope, "内容范围"},
            {AttributeSince, "时间段"}
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

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
		{
		    var type = string.Empty;
            var channelIndex = string.Empty;
            var channelName = string.Empty;
            var upLevel = 0;
            var topLevel = -1;
            var scope = EScopeType.Self;
            var since = string.Empty;

		    foreach (var name in contextInfo.Attributes.Keys)
		    {
		        var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeType))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeChannelIndex))
                {
                    channelIndex = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeChannelName))
                {
                    channelName = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeUpLevel))
                {
                    upLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeTopLevel))
                {
                    topLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeScope))
                {
                    scope = EScopeTypeUtils.GetEnumType(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeSince))
                {
                    since = value;
                }
            }

            return ParseImpl(pageInfo, contextInfo, type, channelIndex, channelName, upLevel, topLevel, scope, since);
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
                channelId = StlDataUtility.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelId, channelIndex, channelName);

                var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, channelId);

                //var nodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeInfo.NodeId, nodeInfo.ChildrenCount, scope, string.Empty, string.Empty);
                var nodeIdList = Node.GetNodeIdListByScopeType(nodeInfo.NodeId, nodeInfo.ChildrenCount, scope, string.Empty, string.Empty);
                foreach (var nodeId in nodeIdList)
                {
                    var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, nodeId);
                    //count += DataProvider.ContentDao.GetCountOfContentAdd(tableName, pageInfo.PublishmentSystemId, nodeId, EScopeType.Self, sinceDate, DateTime.Now.AddDays(1), string.Empty);
                    count += Content.GetCountOfContentAdd(tableName, pageInfo.PublishmentSystemId, nodeId, EScopeType.Self, sinceDate, DateTime.Now.AddDays(1), string.Empty);
                }
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeChannels))
            {
                var channelId = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelId, upLevel, topLevel);
                channelId = StlDataUtility.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelId, channelIndex, channelName);

                var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, channelId);
                count = nodeInfo.ChildrenCount;
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeComments))
            {
                //count = DataProvider.CommentDao.GetCountChecked(pageInfo.PublishmentSystemId, contextInfo.ChannelId, contextInfo.ContentId);
                count = Comment.GetCountChecked(pageInfo.PublishmentSystemId, contextInfo.ChannelId, contextInfo.ContentId);
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
