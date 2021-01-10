using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "显示数值", Description = "通过 stl:count 标签在模板中显示统计数字")]
    public static class StlCount
	{
		public const string ElementName = "stl:count";

		[StlAttribute(Title = "需要获取值的类型")]
        private const string Type = nameof(Type);

        [StlAttribute(Title = "栏目索引")]
        private const string ChannelIndex = nameof(ChannelIndex);

        [StlAttribute(Title = "栏目索引")]
        private const string Index = nameof(Index);

        [StlAttribute(Title = "栏目名称")]
        private const string ChannelName = nameof(ChannelName);

        [StlAttribute(Title = "上级栏目的级别")]
        private const string UpLevel = nameof(UpLevel);

        [StlAttribute(Title = "从首页向下的栏目级别")]
        private const string TopLevel = nameof(TopLevel);

        [StlAttribute(Title = "内容范围")]
        private const string Scope = nameof(Scope);

        [StlAttribute(Title = "时间段")]
        private const string Since = nameof(Since);


        public const string TypeChannels = "Channels";
        public const string TypeContents = "Contents";

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {TypeChannels, "栏目数"},
            {TypeContents, "内容数"}
        };

        public static async Task<object> ParseAsync(IParseManager parseManager)
		{
		    var type = string.Empty;
            var channelIndex = string.Empty;
            var channelName = string.Empty;
            var upLevel = 0;
            var topLevel = -1;
            var scope = ScopeType.Self;
            var since = string.Empty;

		    foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
		    {
		        var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelIndex) || StringUtils.EqualsIgnoreCase(name, Index))
                {
                    channelIndex = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName))
                {
                    channelName = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, UpLevel))
                {
                    upLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, TopLevel))
                {
                    topLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Scope))
                {
                    scope = TranslateUtils.ToEnum(value, ScopeType.Self);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Since))
                {
                    since = value;
                }
            }

            return await ParseAsync(parseManager, type, channelIndex, channelName, upLevel, topLevel, scope, since);
		}

        private static async Task<string> ParseAsync(IParseManager parseManager, string type, string channelIndex, string channelName, int upLevel, int topLevel, ScopeType scope, string since)
        {
            var databaseManager = parseManager.DatabaseManager;
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            var count = 0;

            var sinceDate = Constants.SqlMinValue;
            if (!string.IsNullOrEmpty(since))
            {
                sinceDate = DateTime.Now.AddHours(-DateUtils.GetSinceHours(since));
            }

            if (string.IsNullOrEmpty(type) || StringUtils.EqualsIgnoreCase(type, TypeContents))
            {
                var dataManager = new StlDataManager(parseManager.DatabaseManager);
                var channelId = await dataManager.GetChannelIdByLevelAsync(pageInfo.SiteId, contextInfo.ChannelId, upLevel, topLevel);
                channelId = await dataManager.GetChannelIdByChannelIdOrChannelIndexOrChannelNameAsync(pageInfo.SiteId, channelId, channelIndex, channelName);

                var channelIdList = await databaseManager.ChannelRepository.GetChannelIdsAsync(pageInfo.SiteId, channelId, scope);
                foreach (var theChannelId in channelIdList)
                {
                    var tableName = await databaseManager.ChannelRepository.GetTableNameAsync(pageInfo.Site, theChannelId);
                    count += await databaseManager.ContentRepository.GetCountOfContentAddAsync(tableName, pageInfo.SiteId, theChannelId, ScopeType.Self, sinceDate, DateTime.Now.AddDays(1), 0, true);
                }
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeChannels))
            {
                var dataManager = new StlDataManager(parseManager.DatabaseManager);
                var channelId = await dataManager.GetChannelIdByLevelAsync(pageInfo.SiteId, contextInfo.ChannelId, upLevel, topLevel);
                channelId = await dataManager.GetChannelIdByChannelIdOrChannelIndexOrChannelNameAsync(pageInfo.SiteId, channelId, channelIndex, channelName);

                var nodeInfo = await databaseManager.ChannelRepository.GetAsync(channelId);
                count = nodeInfo.ChildrenCount;
            }

            return count.ToString();
        }
	}
}
