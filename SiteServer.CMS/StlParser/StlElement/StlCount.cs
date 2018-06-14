using System;
using System.Collections.Generic;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "显示数值", Description = "通过 stl:count 标签在模板中显示统计数字")]
    public class StlCount
	{
        private StlCount() { }
		public const string ElementName = "stl:count";

		private static readonly Attr Type = new Attr("type", "需要获取值的类型");
        private static readonly Attr ChannelIndex = new Attr("channelIndex", "栏目索引");
        private static readonly Attr ChannelName = new Attr("channelName", "栏目名称");
        private static readonly Attr UpLevel = new Attr("upLevel", "上级栏目的级别");
        private static readonly Attr TopLevel = new Attr("topLevel", "从首页向下的栏目级别");
        private static readonly Attr Scope = new Attr("scope", "内容范围");
        private static readonly Attr Since = new Attr("since", "时间段");

        public const string TypeChannels = "Channels";
        public const string TypeContents = "Contents";

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {TypeChannels, "栏目数"},
            {TypeContents, "内容数"}
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

		    foreach (var name in contextInfo.Attributes.AllKeys)
		    {
		        var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Type.Name))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelIndex.Name))
                {
                    channelIndex = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName.Name))
                {
                    channelName = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, UpLevel.Name))
                {
                    upLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, TopLevel.Name))
                {
                    topLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Scope.Name))
                {
                    scope = EScopeTypeUtils.GetEnumType(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Since.Name))
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
                var channelId = StlDataUtility.GetChannelIdByLevel(pageInfo.SiteId, contextInfo.ChannelId, upLevel, topLevel);
                channelId = StlDataUtility.GetChannelIdByChannelIdOrChannelIndexOrChannelName(pageInfo.SiteId, channelId, channelIndex, channelName);

                var nodeInfo = ChannelManager.GetChannelInfo(pageInfo.SiteId, channelId);
                var channelIdList = ChannelManager.GetChannelIdList(nodeInfo, scope, string.Empty, string.Empty, string.Empty);
                foreach (var theChannelId in channelIdList)
                {
                    var tableName = ChannelManager.GetTableName(pageInfo.SiteInfo, theChannelId);
                    count += Content.GetCountOfContentAdd(tableName, pageInfo.SiteId, theChannelId, EScopeType.Self, sinceDate, DateTime.Now.AddDays(1), string.Empty, ETriState.True);
                }
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeChannels))
            {
                var channelId = StlDataUtility.GetChannelIdByLevel(pageInfo.SiteId, contextInfo.ChannelId, upLevel, topLevel);
                channelId = StlDataUtility.GetChannelIdByChannelIdOrChannelIndexOrChannelName(pageInfo.SiteId, channelId, channelIndex, channelName);

                var nodeInfo = ChannelManager.GetChannelInfo(pageInfo.SiteId, channelId);
                count = nodeInfo.ChildrenCount;
            }

            return count.ToString();
        }
	}
}
